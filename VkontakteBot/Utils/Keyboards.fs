namespace VkontakteBot.Utils

open SharedImp.Enums
open VkNet.Enums.SafetyEnums
open VkNet.Model.Keyboard

type Keyboards() =
    static member CreateDefaultKeyboard() =
        KeyboardBuilder()
            .AddButton("🔎 Поиск расписания", ActionType.SearchSchedule.ToString(), KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton(
                "🎓 Поиск расписания преподавателя",
                ActionType.SearchTeacherSchedule.ToString(),
                KeyboardButtonColor.Primary
            )
            .AddLine()
            .AddButton("🛠 Настройки", ActionType.Settings.ToString(), KeyboardButtonColor.Positive)
            .Build()

    static member CreateSettingsKeyboard() =
        KeyboardBuilder()
            .AddButton("🆕 Выбрать группу", ActionType.ResetUser.ToString(), KeyboardButtonColor.Primary)
            .AddLine()
            .AddButton("⌨ На главную", ActionType.Start.ToString(), KeyboardButtonColor.Positive)
            .Build()

    static member CreateVerticalKeyboard(array, prefix, ?backPayload: 'a) =
        let builder = KeyboardBuilder()

        for value in array do
            builder
                .AddButton(value, $"{prefix}.{value}", KeyboardButtonColor.Default)
                .AddLine()
            |> ignore

        if backPayload.IsNone then
            builder
                .AddButton("Ⓜ На главную", ActionType.Start.ToString(), KeyboardButtonColor.Positive)
                .Build()
        else
            builder
                .AddButton("↩ Назад", backPayload.Value.ToString(), KeyboardButtonColor.Primary)
                .AddLine()
            |> ignore

            builder
                .AddButton("⌨ На главную", ActionType.Start.ToString(), KeyboardButtonColor.Positive)
                .Build()

    static member CreateGroupsKeyboard(groups) =
        let builder = KeyboardBuilder()
        let groupChunk = groups |> Seq.chunkBySize 3

        for chunks in groupChunk do
            for group in chunks do
                builder.AddButton(group, $"{ActionType.SetGroup}.{group}", KeyboardButtonColor.Default)
                |> ignore

            builder.AddLine() |> ignore

        builder
            .AddButton("↩ Назад", ActionType.BackToCourse.ToString(), KeyboardButtonColor.Primary)
            .AddLine()
        |> ignore

        builder
            .AddButton("⌨ На главную", ActionType.Start.ToString(), KeyboardButtonColor.Positive)
            .Build()

    static member CreateScheduleKeyboard(ranges) =
        let builder = KeyboardBuilder()

        builder
            .AddButton("Сегодня", ActionType.TodaySchedule.ToString(), KeyboardButtonColor.Primary)
            .AddButton("Завтра", ActionType.TomorrowSchedule.ToString(), KeyboardButtonColor.Primary)
            .AddLine()
        |> ignore

        for range in ranges do
            builder
                .AddButton(range, $"{ActionType.SpecificScheduleRange}.{range}", KeyboardButtonColor.Default)
                .AddLine()
            |> ignore

        builder.AddButton("🔄 Обновить", ActionType.RefreshSchedule.ToString(), KeyboardButtonColor.Primary)
        |> ignore

        builder
            .AddButton("⌨ На главную", ActionType.Start.ToString(), KeyboardButtonColor.Positive)
            .Build()
