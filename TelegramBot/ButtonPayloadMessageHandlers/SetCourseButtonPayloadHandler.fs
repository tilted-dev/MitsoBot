namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type SetCourseButtonPayloadHandler(tgClient:ITelegramBotClient, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetCourseButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.Course <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
                match! mitsoService.GetGroupsAsync requestData.User with
                | Ok groups ->
                    let keyboard = Keyboards.CreateGroupsKeyboard groups
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SetGroup, replyMarkup=keyboard)
                        |> Async.AwaitTask
                        |> Async.Ignore
                | Error error ->
                    do! tgClient.SendTextMessageAsync(requestData.User.Id, text=MitsoServiceResults.toString error)
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