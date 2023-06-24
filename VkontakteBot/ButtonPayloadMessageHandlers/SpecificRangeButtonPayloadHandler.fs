namespace VkontakteBot.ButtonPayloadMessageHandlers

open System
open SharedImp.Interfaces
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.Utils
open SharedImp.MonadModels.MitsoServiceResults

type SpecificRangeButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SpecificRangeButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            let rangeParts = requestData.Content.Split([|"С "; " по "|], StringSplitOptions.RemoveEmptyEntries)
            let periodStart = rangeParts |> Seq.head |> DateTime.Parse
            let periodEnd = rangeParts |> Seq.last |> DateTime.Parse
            match! mitsoService.GetSpecificRangeInScheduleAsync(requestData.User, periodStart, periodEnd) with
            | Ok schedule -> do! vkApi.SendMessageAsync(requestData.User.Id, schedule.PrettifyData requestData.Content)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }