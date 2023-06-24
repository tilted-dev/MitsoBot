namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type SearchTeacherScheduleButtonPayloadHandler(vkApi:IVkApi) =
    interface IGenericButtonPayloadHandler<SearchTeacherScheduleButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SearchTeacherSchedule)
        }