namespace TelegramBot.Utils

open SharedImp.Enums
open Telegram.Bot.Types.ReplyMarkups

type Keyboards =
    static member CreateDefaultKeyboard() =
        seq {
            seq {
                InlineKeyboardButton.WithCallbackData("🔎 Поиск расписания", $"{ActionType.SearchSchedule}")
            }
            seq {
                InlineKeyboardButton.WithCallbackData("🎓 Поиск расписания преподавателя", $"{ActionType.SearchTeacherSchedule}")
            }
            seq {
                InlineKeyboardButton.WithCallbackData("🛠 Настройки", $"{ActionType.Settings}")
            }
        } |> InlineKeyboardMarkup

    static member CreateStaticKeyboard() =
        ReplyKeyboardMarkup("Меню", ResizeKeyboard=true)
    
    static member CreateSettingsKeyboard() =
        seq {
            seq {
                InlineKeyboardButton.WithCallbackData("🆕 Выбрать группу", $"{ActionType.ResetUser}")
            }
            seq {
                InlineKeyboardButton.WithCallbackData("⌨ На главную", $"{ActionType.Start}")
            }
        } |> InlineKeyboardMarkup
    
    static member CreateVerticalKeyboard(array, prefix, ?backPayload: 'a) =
        let result = ResizeArray()
        for value in array do
            seq {
                InlineKeyboardButton.WithCallbackData(value, $"{prefix}.{value}")
            } |> result.Add
        if backPayload.IsNone then
            seq {
                InlineKeyboardButton.WithCallbackData("⌨ На главную", $"{ActionType.Start}")
            } |> result.Add
        else
            seq {
                InlineKeyboardButton.WithCallbackData("↩ Назад", $"{backPayload.Value}")
            } |> result.Add
            seq {
                InlineKeyboardButton.WithCallbackData("⌨ На главную", $"{ActionType.Start}")
            } |> result.Add
        InlineKeyboardMarkup result

    static member CreateGroupsKeyboard(groups) =
        let groupChunk = groups |> Seq.chunkBySize 3
        let result = ResizeArray()
        for chunks in groupChunk do
            chunks
            |> Seq.map(fun x -> InlineKeyboardButton.WithCallbackData(x, $"{ActionType.SetGroup}.{x}"))
            |> result.Add
        seq {
            InlineKeyboardButton.WithCallbackData("↩ Назад", $"{ActionType.BackToCourse}")
        } |> result.Add
        seq {
            InlineKeyboardButton.WithCallbackData("⌨ На главную", $"{ActionType.Start}")
        } |> result.Add
        InlineKeyboardMarkup result
    
    static member CreateScheduleKeyboard(ranges) =
        let result = ResizeArray()
        seq {
            InlineKeyboardButton.WithCallbackData("Сегодня", $"{ActionType.TodaySchedule}")
            InlineKeyboardButton.WithCallbackData("Завтра", $"{ActionType.TomorrowSchedule}")
        } |> result.Add
        for range in ranges do
            seq {
                InlineKeyboardButton.WithCallbackData(range, $"{ActionType.SpecificScheduleRange}.{range}")
            } |> result.Add
        seq {
            InlineKeyboardButton.WithCallbackData("🔄 Обновить", $"{ActionType.RefreshSchedule}")
        } |> result.Add
        seq {
            InlineKeyboardButton.WithCallbackData("⌨ На главную", $"{ActionType.Start}")
        } |> result.Add
        InlineKeyboardMarkup result