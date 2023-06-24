namespace SharedImp.Interfaces

open System
open System.Collections.Generic
open SharedImp.Models
open SharedImp.MonadModels.MitsoServiceResults
open SharedImp.Tables

type IMitsoService =
    abstract member GetFacultiesAsync : user:User -> Async<string list ServiceResult>
    abstract member GetFormsAsync : user:User -> Async<string list ServiceResult>
    abstract member GetCoursesAsync : user:User -> Async<string list ServiceResult>
    abstract member GetGroupsAsync : user:User -> Async<string list ServiceResult>
    abstract member GetSpecificRangeInScheduleAsync : user:User * periodStart:DateTime * periodEnd:DateTime -> Async<Dictionary<DateTime, Lesson list> ServiceResult>
    abstract member GetSpecificDayInScheduleAsync : user:User * date:DateTime -> Async<(DateTime * list<Lesson>) ServiceResult>
    abstract member GetScheduleRangesAsync : user:User -> Async<string list ServiceResult>
    abstract member GetTeacherScheduleAsync : user:User * surname:string -> Async<Schedule ServiceResult>