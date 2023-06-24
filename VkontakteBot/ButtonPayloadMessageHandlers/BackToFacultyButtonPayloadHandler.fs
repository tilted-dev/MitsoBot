namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type BackToFacultyButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToFacultyButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetFacultiesAsync requestData.User with
            | Ok faculties -> do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetFaculty, Keyboards.CreateVerticalKeyboard(faculties, ActionType.SetFaculty))
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }