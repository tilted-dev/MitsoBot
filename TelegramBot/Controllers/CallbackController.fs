namespace TelegramBot.Controllers

open Microsoft.AspNetCore.Mvc
open SharedImp.Interfaces
open Telegram.Bot.Types
open Telegram.Bot.Types.Enums

[<Route("api/[action]")>]
type CallbackController(requestAdapter:IRequestAdapter, usersService:IUsersService) =
    inherit ControllerBase()
    
    [<HttpPost>]
    member this.CallBack([<FromBody>] update: Update) : Async<IActionResult> = async {
        match update.Type with
        | UpdateType.Message ->
            match! usersService.GetOrAddUserAsync update.Message.From.Id with
            | Ok user ->
                requestAdapter.ProcessRequest {
                  MessageType = SharedImp.Enums.MessageType.Text
                  User = user
                  Content = update.Message.Text
                  CurrentMessageId = update.Message.MessageId
                  CallbackQueryId = null
                }
                return this.Ok()
            | _ -> return this.Ok()
        | UpdateType.CallbackQuery ->
            match! usersService.GetOrAddUserAsync update.CallbackQuery.From.Id with
            | Ok user ->
                requestAdapter.ProcessRequest {
                  MessageType = SharedImp.Enums.MessageType.Payload
                  User = user
                  Content = update.CallbackQuery.Data
                  CurrentMessageId = update.CallbackQuery.Message.MessageId
                  CallbackQueryId = update.CallbackQuery.Id
                }
                return this.Ok()
            | _ -> return this.Ok()
        | _ -> return this.Ok()
    }