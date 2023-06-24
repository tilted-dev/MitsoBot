namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils
open SharedImp.MonadModels.MitsoServiceResults

type BackToFormButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToFormButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetFormsAsync requestData.User with
            | Ok forms ->
                let keyboard = Keyboards.CreateVerticalKeyboard(forms, ActionType.SetForm, ActionType.BackToFaculty)
                do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetForm, keyboard)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }