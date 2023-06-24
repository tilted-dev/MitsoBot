namespace SharedImp.Tables

open System
open System.ComponentModel.DataAnnotations

type TeacherSchedule = {
    [<Key>] Id : int64
    PeriodStart : DateTime
    PeriodEnd : DateTime
    Surname : string
    Schedule : string
    UpdatedAt : DateTime   
}