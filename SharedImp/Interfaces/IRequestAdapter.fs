namespace SharedImp.Interfaces

open SharedImp.Models

type IRequestAdapter =
    abstract member ProcessRequest: requestData:RequestData -> unit