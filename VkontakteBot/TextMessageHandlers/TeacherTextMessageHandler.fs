namespace VkontakteBot.TestMessageHandlers

open System
open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults

type TeacherTextMessageHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericTextCommandHandler<TeacherTextMessageHandler> with
        member this.ProcessCommand requestData = async {
            if String.IsNullOrWhiteSpace requestData.Content || requestData.Content.Length < 3 then
                do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.NoTeacherScheduleContent)
            else
                match! mitsoService.GetTeacherScheduleAsync(requestData.User, requestData.Content) with
                | Ok schedule ->
                    for week in schedule do
                        do! vkApi.SendMessageAsync(requestData.User.Id, week.Value.PrettifyData week.Key)
                        do! Async.Sleep 300
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }