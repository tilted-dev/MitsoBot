namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open SharedImp.Tables
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type ResetUserButtonPayloadHandler(tgClient:ITelegramBotClient, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<ResetUserButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! usersService.ResetUserAsync requestData.User with
            | Ok _ ->
                let resetUser : SharedImp.Tables.User = { Id = requestData.User.Id; Faculty = null; Form = null; Course = null; GroupName = null; Notification = false }
                match! mitsoService.GetFacultiesAsync(resetUser) with
                | Ok faculties ->
                    let keyboard = Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty)
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetFaculty, replyMarkup=keyboard)
                        |> Async.AwaitTask
                        |> Async.Ignore
                | Error error ->
                    do! tgClient.SendTextMessageAsync(resetUser.Id, MitsoServiceResults.toString error)
                        |> Async.AwaitTask
                        |> Async.Ignore
            | Error error ->
                do! tgClient.SendTextMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
                    |> Async.AwaitTask
                    |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }