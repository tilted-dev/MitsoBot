namespace TelegramBot.ButtonPayloadMessageHandlers

open System
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults
open Telegram.Bot.Types
open TelegramBot.Utils

type TomorrowScheduleButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<TomorrowScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetSpecificDayInScheduleAsync(requestData.User, DateTime.Today.AddDays 1) with
            | Ok schedule ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, schedule.PrettifyData())
                    |> Async.AwaitTask
                    |> Async.Ignore
                match! mitsoService.GetScheduleRangesAsync requestData.User with
                | Ok ranges ->
                    do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=TextMessages.UseScheduleKeyboard, replyMarkup=Keyboards.CreateScheduleKeyboard ranges)
                        |> Async.AwaitTask
                        |> Async.Ignore
                | Error _ -> ()
                do! tgClient.DeleteMessageAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId)
                    |> Async.AwaitTask
                    |> Async.Ignore
            | Error error ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, toString error)
                    |> Async.AwaitTask
                    |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }
