namespace SharedImp.Utils

open System
open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Text
open SharedImp.Models

[<Extension>]
type TextMessagesUtil() =
    
    [<Extension>]
    static member ToDateString(date:DateTime) =
        let builder = StringBuilder()
        let month =
            match date.Month with
            | 1 -> " Января."
            | 2 -> " Февраля."
            | 3 -> " Марта."
            | 4 -> " Апреля." 
            | 5 -> " Мая."
            | 6 -> " Июня."
            | 7 -> " Июля."
            | 8 -> " Августа."
            | 9 -> " Сентября."
            | 10 -> " Октября."
            | 11 -> " Ноября."
            | 12 -> " Декабря."
            | _ -> String.Empty
        
        builder.Append(date.Day).Append month |> ignore
        
        let dayOfWeek =
            match date.DayOfWeek with
            | DayOfWeek.Monday -> " Понедельник."
            | DayOfWeek.Tuesday -> " Вторник."
            | DayOfWeek.Wednesday -> " Среда."
            | DayOfWeek.Thursday -> " Четверг."
            | DayOfWeek.Friday -> " Пятница."
            | DayOfWeek.Saturday -> " Суббота."
            | DayOfWeek.Sunday -> " Воскресенье."
            | _ -> String.Empty
        
        builder.Append(dayOfWeek).ToString()
        
    [<Extension>]
    static member PrettifyData(week: Dictionary<DateTime, Lesson list>, period) =
        let builder = StringBuilder()
        builder.AppendLine $"↘ Период: {period}" |> ignore
        
        for kv in week do
            builder.AppendLine($"📅 Дата: {kv.Key.ToDateString()}").AppendLine() |> ignore
            for lesson in kv.Value do
                builder
                    .AppendLine($"⏰ {lesson.Time} 📌 Ауд. {lesson.Auditorium}")
                    .AppendLine($"📚 {lesson.Subject}") |> ignore
            builder.AppendLine() |> ignore
        builder.AppendLine().ToString()
    
    [<Extension>]
    static member PrettifyData(specificDay: DateTime * Lesson list) =
        let day, lessons = specificDay
        let builder = StringBuilder()
        builder.AppendLine($"📅 Дата: {day.ToDateString()}").AppendLine() |> ignore
        for lesson in lessons do
             builder
                 .AppendLine($"⏰ {lesson.Time} 📌 Ауд. {lesson.Auditorium}")
                 .AppendLine($"📚 {lesson.Subject}") |> ignore
        builder.AppendLine().ToString()
    
    [<Extension>]
    static member PrettifyData(schedule: Schedule) =
        let builder = StringBuilder()
        for week in schedule do
            builder.AppendLine $"↘ Период: {week.Key}" |> ignore
            for kv in week.Value do
                builder.AppendLine($"📅 Дата: {kv.Key.ToDateString()}").AppendLine() |> ignore
                for lesson in kv.Value do
                    builder
                        .AppendLine($"⏰ {lesson.Time} 📌 Ауд. {lesson.Auditorium}")
                        .AppendLine($"📚 {lesson.Subject}") |> ignore
                builder.AppendLine() |> ignore
        builder.ToString()