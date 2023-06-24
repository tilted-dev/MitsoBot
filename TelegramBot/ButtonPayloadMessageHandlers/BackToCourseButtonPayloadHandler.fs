namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type BackToCourseButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToCourseButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetCoursesAsync requestData.User with
            | Ok courses ->
                let keyboard = Keyboards.CreateVerticalKeyboard(courses, ActionType.SetCourse, ActionType.BackToForm)
                do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetCourse, replyMarkup=keyboard)
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