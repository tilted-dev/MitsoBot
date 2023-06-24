namespace SharedImp.Interfaces

open System
open System.Collections.Generic
open SharedImp.Models
open SharedImp.MonadModels.ScheduleServiceResults
open SharedImp.Tables

type IScheduleService =
    abstract member GetFacultiesAsync : user:User -> Async<string list ScheduleServiceResult>
    abstract member GetFormsAsync : user:User -> Async<string list ScheduleServiceResult>
    abstract member GetCoursesAsync : user:User -> Async<string list ScheduleServiceResult>
    abstract member GetGroupsAsync : user:User -> Async<string list ScheduleServiceResult>
    abstract member GetSpecificRangeInScheduleAsync : user:User * periodStart:DateTime * periodEnd:DateTime -> Async<Dictionary<DateTime, Lesson list> ScheduleServiceResult>
    abstract member GetSpecificDayInScheduleAsync : user:User * date:DateTime -> Async<(DateTime * list<Lesson>) ScheduleServiceResult>
    abstract member FindSpecificDayInSchedule : user:User * date:DateTime * schedule:Schedule -> (DateTime * list<Lesson>) ScheduleServiceResult
    abstract member GetScheduleRangesAsync : user:User -> Async<string list ScheduleServiceResult>
    abstract member FindSpecificRangeInSchedule : user:User * periodStart:DateTime * periodEnd:DateTime * schedule:Schedule -> Dictionary<DateTime, Lesson list> ScheduleServiceResult
    abstract member GetTeacherScheduleAsync : user:User * surname:string -> Async<Schedule ScheduleServiceResult>
    abstract member InsertFacultiesAsync : user:User * faculties:string list -> Async<unit>
    abstract member InsertFormsAsync : user:User * forms:string list -> Async<unit>
    abstract member InsertCoursesAsync : user:User * courses:string list -> Async<unit>
    abstract member InsertGroupsAsync : user:User * groups:string list -> Async<unit>
    abstract member InsertScheduleAsync : user:User * schedule:Schedule -> Async<unit>
    abstract member InsertTeacherScheduleAsync : user:User * surname:string * schedule:Schedule -> Async<unit>