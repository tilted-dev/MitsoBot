namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.MonadModels.MitsoServiceResults
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type BackToCourseButtonPayloadHandler(vkApi:IVkApi, mitsoService:IMitsoService) =
    interface IGenericButtonPayloadHandler<BackToCourseButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            match! mitsoService.GetCoursesAsync requestData.User with
            | Ok courses ->
                let keyboard = Keyboards.CreateVerticalKeyboard(courses, ActionType.SetCourse, ActionType.BackToForm)
                do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SetCourse, keyboard)
            | Error error -> do! vkApi.SendMessageAsync(requestData.User.Id, toString error)
        }