namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type SettingsButtonPayloadHandler(vkApi:IVkApi) =
    interface IGenericButtonPayloadHandler<SettingsButtonPayloadHandler>
    interface IButtonPayloadHandler with
        member this.ProcessPayload requestData = async {
            do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.SettingsMessage, Keyboards.CreateSettingsKeyboard())
        }