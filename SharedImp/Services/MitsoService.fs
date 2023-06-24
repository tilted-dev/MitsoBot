namespace SharedImp.Services

open SharedImp.Interfaces
open SharedImp.MonadModels.MitsoApiResults
open SharedImp.MonadModels.MitsoServiceResults
open SharedImp.MonadModels.ScheduleServiceResults
open SharedImp.Resources

type MitsoService(mitsoApiService : IMitsoApiService, scheduleService : IScheduleService) =
    interface IMitsoService with
        member this.GetFacultiesAsync(user) = async {
            match! scheduleService.GetFacultiesAsync(user) with
            | Ok faculties -> return Ok faculties
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetFacultiesAsync(user) with
                    | Ok faculties ->
                        do! scheduleService.InsertFacultiesAsync(user, faculties)
                        return Ok faculties
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoFacultiesContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }

        member this.GetCoursesAsync(user) = async {
            match! scheduleService.GetCoursesAsync(user) with
            | Ok courses -> return Ok courses
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetCoursesAsync(user) with
                    | Ok courses ->
                        do! scheduleService.InsertCoursesAsync(user, courses)
                        return Ok <| courses
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoCoursesContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }
        
        member this.GetFormsAsync(user) = async {
            match! scheduleService.GetFormsAsync(user) with
            | Ok forms -> return Ok forms
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetFormsAsync(user) with
                    | Ok forms ->
                        do! scheduleService.InsertFormsAsync(user, forms)
                        return Ok forms
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoFormsContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }

        member this.GetGroupsAsync(user) = async {
            match! scheduleService.GetGroupsAsync(user) with
            | Ok groups -> return Ok groups
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetGroupsAsync(user) with
                    | Ok groups ->
                        do! scheduleService.InsertGroupsAsync(user, groups)
                        return Ok groups
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoGroupsContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }

        member this.GetSpecificRangeInScheduleAsync(user, periodStart, periodEnd) = async {
            match! scheduleService.GetSpecificRangeInScheduleAsync(user, periodStart, periodEnd) with
            | Ok schedule -> return Ok schedule
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetScheduleAsync(user) with
                    | Ok schedule ->
                        match scheduleService.FindSpecificRangeInSchedule(user, periodStart, periodEnd, schedule) with
                        | Ok result ->
                            do! scheduleService.InsertScheduleAsync(user, schedule)
                            return Ok <| result
                        | Error error ->
                            match error with
                            | NotFound | CacheExpired -> return Error <| ServiceError TextMessages.NoScheduleContent
                            | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoScheduleContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }
        
        member this.GetSpecificDayInScheduleAsync(user, date) = async {
            match! scheduleService.GetSpecificDayInScheduleAsync(user, date) with
            | Ok schedule -> return Ok schedule
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetScheduleAsync(user) with
                    | Ok schedule ->
                        match scheduleService.FindSpecificDayInSchedule(user, date, schedule) with
                        | Ok result ->
                            do! scheduleService.InsertScheduleAsync(user, schedule)
                            return Ok <| result
                        | Error error ->
                            match error with
                            | NotFound | CacheExpired -> return Error <| ServiceError TextMessages.NoScheduleContent
                            | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoScheduleContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }

        member this.GetTeacherScheduleAsync(user, surname) = async {
            match! scheduleService.GetTeacherScheduleAsync(user, surname) with
            | Ok schedule -> return Ok schedule
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetTeacherScheduleAsync(user, surname) with
                    | Ok schedule ->
                        do! scheduleService.InsertTeacherScheduleAsync(user, surname, schedule)
                        return Ok schedule
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoScheduleContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }

        member this.GetScheduleRangesAsync(user) = async {
            match! scheduleService.GetScheduleRangesAsync(user) with
            | Ok ranges -> return Ok ranges
            | Error error ->
                match error with
                | NotFound | CacheExpired  ->
                    match! mitsoApiService.GetScheduleAsync(user) with
                    | Ok schedule ->
                        do! scheduleService.InsertScheduleAsync(user, schedule)
                        return Ok (schedule |> Seq.map(fun x -> x.Key) |> Seq.toList)
                    | Error error ->
                        match error with
                        | ApiError -> return Error <| ServiceError TextMessages.ApiProblem
                        | NoContent -> return Error <| ServiceError TextMessages.NoScheduleContent
                        | UnexpectedMitsoApiServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
                | UnexpectedScheduleServiceError -> return Error <| ServiceError TextMessages.UnexpectedException
        }