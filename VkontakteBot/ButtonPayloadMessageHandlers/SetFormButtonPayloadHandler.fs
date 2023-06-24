namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels

type SetFormButtonPayloadHandler(vkApi:IVkApi, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetFormButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.Form <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
                match! mitsoService.GetCoursesAsync requestData.User with
                | Ok courses ->
                    let keyboard = Keyboards.CreateVerticalKeyboard(courses, ActionType.SetCourse, ActionType.BackToForm)
                    do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetCourse, keyboard)
                | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, MitsoServiceResults.toString error)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
        }