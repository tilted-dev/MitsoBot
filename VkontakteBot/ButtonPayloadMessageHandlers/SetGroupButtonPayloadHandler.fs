namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type SetGroupButtonPayloadHandler(vkApi:IVkApi, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetGroupButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.GroupName <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
                match! mitsoService.GetScheduleRangesAsync requestData.User with
                | Ok ranges -> do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.UseScheduleKeyboard, Keyboards.CreateScheduleKeyboard ranges)
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, MitsoServiceResults.toString error)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
        }