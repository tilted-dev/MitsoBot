namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type SetFacultyButtonPayloadHandler(tgClient:ITelegramBotClient, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetFacultyButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.Faculty <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
               match! mitsoService.GetFormsAsync requestData.User with
               | Ok forms ->
                   let keyboard = Keyboards.CreateVerticalKeyboard(forms, ActionType.SetForm, ActionType.BackToFaculty)
                   do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetForm, replyMarkup=keyboard)
                       |> Async.AwaitTask
                       |> Async.Ignore
               | Error userError ->
                   do! tgClient.SendTextMessageAsync(requestData.User.Id, text=MitsoServiceResults.toString userError)
                       |> Async.AwaitTask
                       |> Async.Ignore
            | Error error ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, text=UsersServiceResults.toString error)
                    |> Async.AwaitTask
                    |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }