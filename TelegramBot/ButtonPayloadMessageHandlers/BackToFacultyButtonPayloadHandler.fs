namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type BackToFacultyButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToFacultyButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetFacultiesAsync requestData.User with
            | Ok faculties ->
                do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetFaculty, replyMarkup=Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty))
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