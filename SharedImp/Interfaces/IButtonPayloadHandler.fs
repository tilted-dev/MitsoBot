namespace SharedImp.Interfaces

open SharedImp.Models
open SharedImp.Tables

type IButtonPayloadHandler =
    abstract member ProcessPayload: requestData:RequestData -> Async<unit>