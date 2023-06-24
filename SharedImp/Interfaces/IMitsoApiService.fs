namespace SharedImp.Interfaces

open SharedImp.Models
open SharedImp.MonadModels.MitsoApiResults
open SharedImp.Tables

type IMitsoApiService =
    abstract member GetFacultiesAsync : user:User -> Async<string list FetchResult>
    abstract member GetFormsAsync : user:User -> Async<string list FetchResult>
    abstract member GetCoursesAsync : user:User -> Async<string list FetchResult>
    abstract member GetGroupsAsync : user:User -> Async<string list FetchResult>
    abstract member GetScheduleAsync : user:User -> Async<Schedule FetchResult>
    abstract member GetTeacherScheduleAsync : user:User * surname:string -> Async<Schedule FetchResult>