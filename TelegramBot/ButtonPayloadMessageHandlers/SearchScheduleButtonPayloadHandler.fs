namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type SearchScheduleButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SearchScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            if requestData.User.Faculty = null || requestData.User.Form = null || requestData.User.Course = null || requestData.User.GroupName = null then
                match! mitsoService.GetFacultiesAsync(requestData.User) with
                | Ok faculties ->
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetFaculty, replyMarkup=Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty))
                        |> Async.AwaitTask
                        |> Async.Ignore
                | Error error ->
                    do! tgClient.SendTextMessageAsync(requestData.User.Id, text=toString error)
                        |> Async.AwaitTask
                        |> Async.Ignore
            else
                match! mitsoService.GetScheduleRangesAsync(requestData.User) with
                | Ok ranges ->
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.UseScheduleKeyboard, replyMarkup=Keyboards.CreateScheduleKeyboard ranges)
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