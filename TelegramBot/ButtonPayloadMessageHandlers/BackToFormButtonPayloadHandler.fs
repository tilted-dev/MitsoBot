namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type BackToFormButtonPayloadHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToFormButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetFormsAsync requestData.User with
            | Ok forms ->
                let keyboard = Keyboards.CreateVerticalKeyboard(forms, ActionType.SetForm, ActionType.BackToFaculty)
                do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetForm, replyMarkup=keyboard)
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
