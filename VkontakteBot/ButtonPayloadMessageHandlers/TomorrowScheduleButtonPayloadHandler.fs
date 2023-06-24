namespace VkontakteBot.ButtonPayloadMessageHandlers

open System
open SharedImp.Interfaces
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults

type TomorrowScheduleButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<TomorrowScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetSpecificDayInScheduleAsync(requestData.User, DateTime.Today.AddDays 1) with
            | Ok schedule -> do! vkApi.SendMessageAsync(requestData.User.Id, schedule.PrettifyData())
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }