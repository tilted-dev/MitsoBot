namespace SharedImp.Interfaces

open SharedImp.Models
open SharedImp.Tables

type ITextCommandHandler =
    abstract member ProcessCommand: requestData:RequestData -> Async<unit>