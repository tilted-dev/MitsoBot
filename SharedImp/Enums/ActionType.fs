namespace SharedImp.Enums

open System

[<Flags>]
type ActionType =
    | Start  = 0
    | BackToMenu = 1
    | SearchSchedule = 2
    | Settings = 3
    | SetFaculty = 4
    | BackToFaculty = 5
    | SetForm = 6
    | BackToForm = 7
    | SetCourse = 8
    | BackToCourse = 9
    | SetGroup = 10
    | GetScheduleRanges = 11
    | SearchTeacherSchedule = 12
    | SpecificScheduleRange = 13
    | TodaySchedule = 14
    | TomorrowSchedule = 15
    | ResetUser = 16
    | RefreshSchedule = 17
