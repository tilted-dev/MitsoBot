namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type RefreshScheduleButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<RefreshScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetScheduleRangesAsync requestData.User with
            | Ok ranges -> do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.ScheduleRefreshed, Keyboards.CreateScheduleKeyboard ranges)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }