namespace VkontakteBot.TextMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open VkNet.Abstractions
open VkontakteBot.Utils

type StartTextMessageHandler(vkApi:IVkApi) =
    interface IGenericTextCommandHandler<StartTextMessageHandler> with
        member this.ProcessCommand requestData = async {
            do! vkApi.SendMessageAsync(requestData.User.Id, TextMessages.StartMessage, Keyboards.CreateDefaultKeyboard())
        }