namespace SharedImp.Tables

open System
open System.ComponentModel.DataAnnotations

type StoredSchedule = {
    [<Key>] Id : int64
    PeriodStart : DateTime
    PeriodEnd : DateTime
    Faculty : string
    Form : string
    Course : string
    GroupName : string
    Schedule : string
    UpdatedAt : DateTime   
}