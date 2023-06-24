namespace VkontakteBot.Utils

open System
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open VkNet.Abstractions
open VkNet.Model.Keyboard
open VkNet.Model.RequestParams

[<Extension>]
type VkApiUtil =
    [<Extension>]
    static member SendMessageAsync(api: IVkApi, vk: int64, message: string, [<Optional; DefaultParameterValue(null:MessageKeyboard)>] keyboard: MessageKeyboard) = async {
     let! _ = (api.Messages.SendAsync(MessagesSendParams(UserId = vk, RandomId = Random.Shared.Next(), Message = message, Keyboard = keyboard)) |> Async.AwaitTask)
     ()
    }