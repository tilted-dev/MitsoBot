namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type SetGroupButtonPayloadHandler(tgClient:ITelegramBotClient, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetGroupButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.GroupName <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
                match! mitsoService.GetScheduleRangesAsync requestData.User with
                | Ok ranges ->
                    do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.UseScheduleKeyboard, replyMarkup=Keyboards.CreateScheduleKeyboard ranges)
                        |> Async.AwaitTask
                        |> Async.Ignore
                | Error error ->
                    do! tgClient.SendTextMessageAsync(requestData.User.Id, MitsoServiceResults.toString error)
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