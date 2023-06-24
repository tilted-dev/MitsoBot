namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot

type SearchTeacherScheduleButtonPayloadHandler(tgClient:ITelegramBotClient) =
    interface IGenericButtonPayloadHandler<SearchTeacherScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            do! tgClient.SendTextMessageAsync(requestData.User.Id, TextMessages.SearchTeacherSchedule)
                |> Async.AwaitTask
                |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }
