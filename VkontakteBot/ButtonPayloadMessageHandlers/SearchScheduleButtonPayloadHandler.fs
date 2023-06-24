namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type SearchScheduleButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SearchScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            if requestData.User.Faculty = null || requestData.User.Form = null || requestData.User.Course = null || requestData.User.GroupName = null then
                match! mitsoService.GetFacultiesAsync(requestData.User) with
                | Ok faculties -> do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetFaculty, Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty))
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
            else
                match! mitsoService.GetScheduleRangesAsync(requestData.User) with
                | Ok ranges -> do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.UseScheduleKeyboard, Keyboards.CreateScheduleKeyboard ranges)
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }