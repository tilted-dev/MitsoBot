namespace TelegramBot.ButtonPayloadMessageHandlers

open SharedImp.Interfaces
open SharedImp.Resources
open Telegram.Bot
open Telegram.Bot.Types
open TelegramBot.Utils

type SettingsButtonPayloadHandler(tgClient:ITelegramBotClient) =
    interface IGenericButtonPayloadHandler<SettingsButtonPayloadHandler> with
        member this.ProcessPayload requestData = async {
            do! tgClient.EditMessageTextAsync(chatId=ChatId.op_Implicit requestData.User.Id, messageId=int requestData.CurrentMessageId, text=TextMessages.SettingsMessage, replyMarkup=Keyboards.CreateSettingsKeyboard())
                |> Async.AwaitTask
                |> Async.Ignore
            do! tgClient.AnswerCallbackQueryAsync(requestData.CallbackQueryId)
                |> Async.AwaitTask
                |> Async.Ignore
        }