namespace TelegramBot.ButtonPayloadMessageHandlers

open System
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults
open Telegram.Bot.Types
open TelegramBot.Utils

type SpecificRangeButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SpecificRangeButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            let rangeParts = requestData.Content.Split([|"С "; " по "|], StringSplitOptions.RemoveEmptyEntries)
            let periodStart = rangeParts |> Seq.head |> DateTime.Parse
            let periodEnd = rangeParts |> Seq.last |> DateTime.Parse
            match! mitsoService.GetSpecificRangeInScheduleAsync(requestData.User, periodStart, periodEnd) with
            | Ok schedule ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, text=schedule.PrettifyData requestData.Content)
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
                do! tgClient.SendTextMessageAsync(requestData.User.Id, text=toString error)
                    |> Async.AwaitTask
                    |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }