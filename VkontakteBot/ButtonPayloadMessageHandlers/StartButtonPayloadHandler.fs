namespace VkontakteBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type StartButtonPayloadHandler(vkApi:IVkApi) =
    interface IGenericButtonPayloadHandler<StartButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.StartMessage, Keyboards.CreateDefaultKeyboard())
        }