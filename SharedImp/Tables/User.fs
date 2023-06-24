namespace SharedImp.Tables

open Microsoft.FSharp.Core
open Newtonsoft.Json

[<CLIMutable>]
type User = {
    mutable Id : int64
    mutable Faculty : string
    mutable Form : string
    mutable Course : string
    mutable GroupName : string
    [<JsonIgnore>]
    mutable Notification : bool
}