namespace TelegramBot.TextMessageHandlers

open System
open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults

type TeacherTextMessageHandler(tgClient:ITelegramBotClient, mitsoService:IMitsoService) =
    interface IGenericTextCommandHandler<TeacherTextMessageHandler> with
        member this.ProcessCommand requestData = async {
            if String.IsNullOrWhiteSpace requestData.Content || requestData.Content.Length < 3 then
                do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=TextMessages.NoTeacherScheduleContent, replyToMessageId=int requestData.CurrentMessageId)
                    |> Async.AwaitTask
                    |> Async.Ignore
            else
                match! mitsoService.GetTeacherScheduleAsync(requestData.User, requestData.Content) with
                | Ok schedule ->
                    for week in schedule do
                        do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=week.Value.PrettifyData week.Key) |> Async.AwaitTask |> Async.Ignore
                        do! Async.Sleep 300
                | Error error -> do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=toString error) |> Async.AwaitTask |> Async.Ignore
        }