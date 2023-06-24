namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open SharedImp.Tables
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels

type ResetUserButtonPayloadHandler(vkApi:IVkApi, usersService:IUsersService, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<ResetUserButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! usersService.ResetUserAsync requestData.User with
            | Ok _ ->
                let resetUser : User = { Id=requestData.User.Id; Faculty = null; Form = null; Course = null; GroupName = null; Notification = false }
                match! mitsoService.GetFacultiesAsync(resetUser) with
                | Ok faculties ->
                    let keyboard = Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty)
                    do! vkApi.SendMessageAsync(resetUser.Id, TextMessages.SetFaculty, keyboard)
                | Error error -> do! vkApi.SendMessageAsync(resetUser.Id, MitsoServiceResults.toString error)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, UsersServiceResults.toString error)
        }