namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels

type SetCourseButtonPayloadHandler(vkApi:IVkApi, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetCourseButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.Course <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
                match! mitsoService.GetGroupsAsync requestData.User with
                | Ok groups ->
                    let keyboard = Keyboards.CreateGroupsKeyboard groups
                    do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetGroup, keyboard)
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, MitsoServiceResults.toString error)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
        }