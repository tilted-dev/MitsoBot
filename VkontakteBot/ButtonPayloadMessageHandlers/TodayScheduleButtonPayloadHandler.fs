namespace VkontakteBot.ButtonPayloadMessageHandlers

open System
open SharedImp.Interfaces
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults

type TodayScheduleButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<TodayScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetSpecificDayInScheduleAsync(requestData.User, DateTime.Today) with
            | Ok schedule -> do! vkApi.SendMessageAsync(requestData.User.Id, schedule.PrettifyData())
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }