namespace SharedImp.Services

open System
open System.Collections.Generic
open Dapper.FSharp.SQLite.Builders
open Dapper.FSharp.SQLite
open Microsoft.Data.Sqlite
open Newtonsoft.Json
open Serilog
open SharedImp.Interfaces
open SharedImp.Models
open SharedImp.MonadModels.ScheduleServiceResults
open SharedImp.Tables

type ScheduleService() =
    let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
    let mutable logger = Log.ForContext<ScheduleService>()
    let scheduleTable = table'<StoredSchedule> "StoredSchedule"
    let teacherScheduleTable = table'<TeacherSchedule> "TeacherSchedule"
    let facultiesTable = table'<Faculty> "Faculties"
    let formsTable = table'<Form> "Forms"
    let coursesTable = table'<Course> "Courses"
    let groupsTable = table'<Group> "Groups"
    interface IScheduleService with
        member this.GetSpecificRangeInScheduleAsync(user, periodStart, periodEnd) = async {
            logger <- logger
             .ForContext("User", user, true)
             .ForContext("PeriodStart", periodStart.ToString "yyyy-MM-dd")
             .ForContext("PeriodEnd", periodEnd.ToString "yyyy-MM-dd")
            try
                let! result = (select {
                    for schedule in scheduleTable do
                        where (schedule.Faculty = user.Faculty
                               && schedule.Form = user.Form
                               && schedule.Course = user.Course
                               && schedule.GroupName = user.GroupName
                               && schedule.PeriodStart = periodStart
                               && schedule.PeriodEnd = periodEnd)
                } |> dbConnection.SelectAsync<StoredSchedule> |> Async.AwaitTask)

                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive schedule from cache with period {PeriodStart}-{PeriodEnd}")
                    return Error NotFound
                else
                    let storedSchedule = result |> Seq.head
                    if (DateTime.Now - storedSchedule.UpdatedAt).TotalMinutes >= 15 then
                        let! _ = this.CleanOldSchedule storedSchedule
                        logger.Information("[CacheExpired] ==> Schedule cache with period {PeriodStart}-{PeriodEnd} expired")
                        return Error CacheExpired
                    else
                        logger.Information("Successfully received schedule from cache with period {PeriodStart}-{PeriodEnd}")
                        return Ok <| JsonConvert.DeserializeObject<Dictionary<DateTime, Lesson list>> storedSchedule.Schedule
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }
        
        member this.GetSpecificDayInScheduleAsync(user, date) = async {
            logger <- logger
             .ForContext("User", user, true)
             .ForContext("Date", date.ToString "yyyy-MM-dd")
            try
                let! result = (select {
                    for schedule in scheduleTable do
                        where (schedule.Faculty = user.Faculty
                               && schedule.Form = user.Form
                               && schedule.Course = user.Course
                               && schedule.GroupName = user.GroupName
                               && schedule.PeriodStart <= date
                               && schedule.PeriodEnd >= date)
                } |> dbConnection.SelectAsync<StoredSchedule> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive schedule from cache for specific date: {Date}")
                    return Error NotFound
                else
                    let storedSchedule = result |> Seq.head
                    if (DateTime.Now - storedSchedule.UpdatedAt).TotalMinutes >= 15 then
                        let! _ = this.CleanOldSchedule storedSchedule
                        logger.Information("[CacheExpired] ==> Schedule cache with date {Date} expired")
                        return Error CacheExpired
                    else
                        let specificRange = JsonConvert.DeserializeObject<Dictionary<DateTime, Lesson list>> storedSchedule.Schedule
                        let specificDay = specificRange |> Seq.tryFind(fun x -> x.Key = date)
                        match specificDay with
                        | Some kv ->
                            logger.Information("Successfully received schedule from cache with date: {Date}")
                            return Ok <| (kv.Key, kv.Value)
                        | None ->
                            logger.Information("[NotFound] ==> Failed to receive schedule from cache for specific date: {Date}")
                            let! _ = this.CleanOldSchedule storedSchedule
                            return Error NotFound
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }
        
        member this.GetTeacherScheduleAsync(user, surname) = async {
            logger <- logger
             .ForContext("User", user, true)
             .ForContext("Surname", surname)
            try
                let currentDate = DateTime.Today
                let! result = (select {
                    for teacherSchedule in teacherScheduleTable do
                        where (teacherSchedule.PeriodStart >= currentDate && teacherSchedule.Surname = surname)
                } |> dbConnection.SelectAsync<TeacherSchedule> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive teacher schedule from cache with surname: {Surname}")
                    return Error NotFound
                else
                    if result |> Seq.exists(fun x -> (DateTime.Now - x.UpdatedAt).TotalMinutes >= 15) then
                        for schedule in result do
                            let! _ = this.CleanOldTeacherSchedule schedule
                            ()
                        logger.Information("[CacheExpired] ==> Teacher schedule cache with date {Date} expired")
                        return Error CacheExpired
                    else
                         logger.Information("Successfully received teacher schedule from cache")
                         let schedule = Schedule()
                         result |> Seq.iter(fun x ->
                             schedule.Add($"С {x.PeriodStart} по {x.PeriodEnd}",
                                          JsonConvert.DeserializeObject<Dictionary<DateTime,Lesson list>> x.Schedule))
                         return Ok schedule
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }
        
        member this.GetFacultiesAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let! result = (select {
                    for faculty in facultiesTable do selectAll
                } |> dbConnection.SelectAsync<Faculty> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive faculties from cache")
                    return Error NotFound
                else
                    logger.Information("Successfully received faculties from cache")
                    return Ok <| (result |> Seq.map(fun x -> x.Name) |> Seq.toList)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }
        
        member this.GetCoursesAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let! result = (select {
                    for course in coursesTable do
                        where(course.Faculty = user.Faculty && course.Form = user.Form)
                } |> dbConnection.SelectAsync<Faculty> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive courses from cache")
                    return Error NotFound
                else
                    logger.Information("Successfully received courses from cache")
                    return Ok <| (result |> Seq.map(fun x -> x.Name) |> Seq.toList)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }

        member this.GetFormsAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let! result = (select {
                    for form in formsTable do
                        where(form.Faculty = user.Faculty)
                } |> dbConnection.SelectAsync<Faculty> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive forms from cache")
                    return Error NotFound
                else
                    logger.Information("Successfully received forms from cache")
                    return Ok <| (result |> Seq.map(fun x -> x.Name) |> Seq.toList)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }

        member this.GetGroupsAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let! result = (select {
                    for group in groupsTable do
                        where(group.Faculty = user.Faculty && group.Form = user.Form && group.Course = user.Course)
                } |> dbConnection.SelectAsync<Faculty> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive groups from cache")
                    return Error NotFound
                else
                    logger.Information("Successfully received groups from cache")
                    return Ok <| (result |> Seq.map(fun x -> x.Name) |> Seq.toList)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }

        member this.InsertFacultiesAsync(user, faculties) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let dbFaculties = faculties |> Seq.map(fun x -> {Name = x}) |> Seq.toList
                let! count = (insert {
                    into facultiesTable
                    values dbFaculties
                } |> dbConnection.InsertAsync |> Async.AwaitTask)
                logger.Information("Successfully inserted {Count} rows into `Faculties` table", count)
            with ex -> logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }

        member this.InsertFormsAsync(user, forms) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let dbForms = forms |> Seq.map(fun x -> {Faculty = user.Faculty; Name = x}) |> Seq.toList
                let! count = (insert {
                    into formsTable
                    values dbForms
                } |> dbConnection.InsertAsync |> Async.AwaitTask)
                logger.Information("Successfully inserted {Count} rows into `Forms` table", count)
            with ex -> logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }

        member this.InsertCoursesAsync(user, courses) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let dbCourses = courses |> Seq.map(fun x -> {Faculty = user.Faculty; Form = user.Form; Name = x}) |> Seq.toList
                let! count = (insert {
                    into coursesTable
                    values dbCourses
                } |> dbConnection.InsertAsync |> Async.AwaitTask)
                logger.Information("Successfully inserted {Count} rows into `Courses` table", count)
            with ex -> logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }

        member this.InsertGroupsAsync(user, groups) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let dbGroups = groups |> Seq.map(fun x -> {Faculty = user.Faculty; Form = user.Form; Course = user.Course; Name = x}) |> Seq.toList
                let! count = (insert {
                    into groupsTable
                    values dbGroups
                } |> dbConnection.InsertAsync |> Async.AwaitTask)
                logger.Information("Successfully inserted {Count} rows into `Groups` table", count)
            with ex -> logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }

        member this.InsertScheduleAsync(user, schedule) = async {
            logger <- logger.ForContext("User", user, true)
            let storedSchedule = schedule |> Seq.map(fun x -> (
                let periods = x.Key.Split([|"С "; " по "|], StringSplitOptions.RemoveEmptyEntries)
                {
                    Id = 0
                    PeriodStart=(periods |> Seq.head |> DateTime.Parse)
                    PeriodEnd=(periods |> Seq.last |> DateTime.Parse)
                    Faculty=user.Faculty
                    Form=user.Form
                    Course=user.Course
                    GroupName=user.GroupName
                    Schedule=(JsonConvert.SerializeObject <| x.Value)
                    UpdatedAt = DateTime.Now
                }
                ))
            try
                let mutable totalRows = 0
                for schToInsert in storedSchedule do
                    let! count = (insert {
                        for scheduleRow in scheduleTable do
                            value schToInsert
                            excludeColumn scheduleRow.Id
                    } |> dbConnection.InsertAsync |> Async.AwaitTask)
                    totalRows <- totalRows + count
                logger.Information("Successfully inserted {Count} rows into `Schedule` table", totalRows)
            with ex -> logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }
        
        member this.InsertTeacherScheduleAsync(user, surname, schedule) = async {
            logger <- logger.ForContext("User", user, true).ForContext("Surname", surname)
            let storedSchedule = schedule |> Seq.map(fun x -> (
                let periods = x.Key.Split([|"С "; " по "|], StringSplitOptions.RemoveEmptyEntries)
                {
                    Id = 0
                    PeriodStart=(periods |> Seq.head |> DateTime.Parse)
                    PeriodEnd=(periods |> Seq.last |> DateTime.Parse)
                    Surname=surname
                    Schedule=(JsonConvert.SerializeObject <| x.Value)
                    UpdatedAt=DateTime.Now
                }
                ))
            try
                let mutable totalRows = 0
                for schToInsert in storedSchedule do
                    let! count = (insert {
                        for scheduleRow in teacherScheduleTable do
                            value schToInsert
                            excludeColumn scheduleRow.Id
                    } |> dbConnection.InsertAsync |> Async.AwaitTask)
                    totalRows <- totalRows + count
                logger.Information("Successfully inserted {Count} rows into `TeacherSchedule` table with Surname: {Surname}", totalRows)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
        }

        member this.GetScheduleRangesAsync(user) = async {
            logger <- logger.ForContext("User", user, true)
            try
                let! result = (select {
                     for schedule in scheduleTable do
                     where (schedule.Faculty = user.Faculty  && schedule.Form = user.Form && schedule.Course = user.Course && schedule.GroupName = user.GroupName)
                } |> dbConnection.SelectAsync<StoredSchedule> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("[NotFound] ==> Failed to receive schedule ranges from cache")
                    return Error NotFound
                else
                    logger.Information("Successfully received schedule ranges from cache")
                    return Ok (result |> Seq.map(fun x -> (
                        let periodStart = x.PeriodStart.ToString "yyyy-MM-dd"
                        let periodEnd = x.PeriodEnd.ToString "yyyy-MM-dd"
                        $"С {periodStart} по {periodEnd}"
                    )) |> Seq.toList)
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                return Error <| UnexpectedScheduleServiceError
        }

        member this.FindSpecificDayInSchedule(user, date, schedule) =
            logger <- logger.ForContext("User", user, true).ForContext("Date", date.ToString "yyyy-MM-dd")
            try
                let foundSchedule =
                 (schedule |> Seq.tryFind(fun x ->
                    let ranges = x.Key.Split([|"С "; " по "|], StringSplitOptions.RemoveEmptyEntries)
                    let periodStart = ranges |> Seq.head |> DateTime.Parse
                    let periodEnd = ranges |> Seq.last |> DateTime.Parse
                                    
                    (date >= periodStart && date <= periodEnd)
                 ))
                 
                match foundSchedule with
                | Some specificSchedule ->
                    match specificSchedule.Value |> Seq.tryFind(fun x -> x.Key = date) with
                    | Some specificDay ->
                        logger.Information("Successfully found specific day {Date} in schedule")
                        Ok <| (specificDay.Key, specificDay.Value)
                    | None ->
                        logger.Warning("[NotFound] ==> Failed to find specific day {Date} in schedule")
                        Error NotFound
                | None ->
                    logger.Warning("[NotFound] ==> Failed to find specific day {Date} in schedule")
                    Error NotFound
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                Error <| UnexpectedScheduleServiceError

        member this.FindSpecificRangeInSchedule(user, startPeriod, endPeriod, schedule) =
            let periodStart = startPeriod.ToString "yyyy-MM-dd"
            let periodEnd = endPeriod.ToString "yyyy-MM-dd"
            logger <- logger
             .ForContext("User", user, true)
             .ForContext("PeriodStart", periodStart)
             .ForContext("PeriodEnd", periodEnd)
            try
                match schedule |> Seq.tryFind(fun x -> x.Key = $"С {periodStart} по {periodEnd}") with
                | Some specificRange ->
                    logger.Information("Successfully found specific range in schedule: {PeriodStart}-{PeriodEnd}")
                    Ok <| specificRange.Value
                | None ->
                    logger.Error("[NotFound] ==> Failed to find specific range in schedule: {PeriodStart}-{PeriodEnd}")
                    Error NotFound
            with ex ->
                logger.Error(ex, "[UnexpectedScheduleServiceError] ==> An unexpected error occurred while executing task")
                Error <| UnexpectedScheduleServiceError

    member this.CleanOldSchedule(storedSchedule:StoredSchedule) = async {
       return delete {
          for schedule in scheduleTable do
              where (schedule.Faculty = storedSchedule.Faculty
                     && schedule.Form = storedSchedule.Form
                     && schedule.Course = storedSchedule.Course
                     && schedule.GroupName = storedSchedule.GroupName)
          } |> dbConnection.DeleteAsync |> Async.AwaitTask
    }
    
    member this.CleanOldTeacherSchedule(storedSchedule:TeacherSchedule) = async {
       return delete {
          for schedule in teacherScheduleTable do
              where (schedule.Surname = storedSchedule.Surname)
          } |> dbConnection.DeleteAsync |> Async.AwaitTask
    }