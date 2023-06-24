namespace TelegramBot.ButtonPayloadMessageHandlers

open Serilog
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type RefreshScheduleButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<RefreshScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetScheduleRangesAsync requestData.User with
            | Ok ranges ->
                try
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.UseScheduleKeyboard, replyMarkup=Keyboards.CreateScheduleKeyboard ranges)
                        |> Async.AwaitTask
                        |> Async.Ignore
                with ex ->
                    do! tgClient.SendTextMessageAsync(requestData.User.Id, TextMessages.ActualSchedule, replyToMessageId=int requestData.CurrentMessageId)
                        |> Async.AwaitTask
                        |> Async.Ignore
                    Log.ForContext("SourceContext", "RefreshScheduleButtonPayloadHandler")
                       .Warning(ex, "Api request exception occurred while updating message")
            | Error error ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, toString error)
                    |> Async.AwaitTask
                    |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }
