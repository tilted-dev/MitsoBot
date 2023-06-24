namespace VkontakteBot.Controllers

open System
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Newtonsoft.Json.Linq
open SharedImp.Enums
open SharedImp.Interfaces
open VkNet.Model.GroupUpdate
open VkNet.Utils
open VkontakteBot.Models

[<Route("api/[action]")>]
type CallbackController(config: IConfiguration, usersService:IUsersService, requestAdapter:IRequestAdapter) =
    inherit ControllerBase()
    
    [<HttpPost>]
    member this.CallBack([<FromBody>] updates: Updates) : Async<IActionResult> = async {
        if not (updates.Secret = config["Secret"]) then
            return this.Unauthorized()
        else
            match updates.Type with
            | "confirmation" ->
                return this.Ok config["Confirm"]
            | "message_new" ->
                let newMessage = MessageNew.FromJson <| VkResponse updates.JObject
                let vk = newMessage.Message.FromId.GetValueOrDefault()
                match! usersService.GetOrAddUserAsync vk with
                | Ok user ->
                    if (String.IsNullOrWhiteSpace newMessage.Message.Payload) then
                        requestAdapter.ProcessRequest {
                            User = user
                            MessageType = MessageType.Text
                            Content = newMessage.Message.Text
                            CurrentMessageId = newMessage.Message.Id.GetValueOrDefault()
                            CallbackQueryId = null
                        }
                    else
                        let mutable payload = String.Empty
                        let jObject = JObject.Parse(newMessage.Message.Payload)
                        let command = jObject.["command"]
                        let button = jObject.["button"]
                        if not (command = null) then
                            payload <- command.Value<string>()
                        else
                            payload <- button.Value<string>()
                        requestAdapter.ProcessRequest {
                            User = user
                            MessageType = MessageType.Payload
                            Content = payload
                            CurrentMessageId = newMessage.Message.Id.GetValueOrDefault()
                            CallbackQueryId = null
                        }
                    return this.Ok "Ok"
                | _ -> return this.Ok "Ok"
            | _ -> return this.Ok "Ok"
    }