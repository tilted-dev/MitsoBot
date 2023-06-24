namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type StartButtonPayloadHandler(tgClient:ITelegramBotClient) =
    interface IGenericButtonPayloadHandler<StartButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.StartMessage, replyMarkup=Keyboards.CreateDefaultKeyboard())
                |> Async.AwaitTask
                |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }