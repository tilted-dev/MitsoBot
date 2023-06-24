namespace SharedImp.Models

open SharedImp.Enums
open SharedImp.Tables

type RequestData = {
    MessageType: MessageType
    User: User
    CurrentMessageId: int64
    CallbackQueryId: string
    mutable Content: string
}