namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.MonadModels
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type SetFacultyButtonPayloadHandler(vkApi:IVkApi, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<SetFacultyButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            requestData.User.Faculty <- requestData.Content
            match! usersService.UpdateUserAsync requestData.User with
            | Ok _ ->
               match! mitsoService.GetFormsAsync requestData.User with
               | Ok forms ->
                   let keyboard = Keyboards.CreateVerticalKeyboard(forms, ActionType.SetForm, ActionType.BackToFaculty)
                   do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetForm, keyboard)
               | Error userError -> do! vkApi.SendMessageAsync(requestData.User.Id, MitsoServiceResults.toString userError)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
        }