namespace TelegramBot.TextMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open TelegramBot.Utils

type StartTextMessageHandler(tgClient:ITelegramBotClient) =
    interface IGenericTextCommandHandler<StartTextMessageHandler> with
        member this.ProcessCommand requestData = async {
            do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=TextMessages.UseStaticKeyboard, replyMarkup=Keyboards.CreateStaticKeyboard())
                |> Async.AwaitTask
                |> Async.Ignore
            do! tgClient.SendTextMessageAsync(chatId=requestData.User.Id, text=TextMessages.StartMessage, replyMarkup=Keyboards.CreateDefaultKeyboard())
                |> Async.AwaitTask
                |> Async.Ignore
        }