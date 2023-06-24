namespace VkontakteBot.Models

open Newtonsoft.Json
open Newtonsoft.Json.Linq

type public Updates = {
    [<JsonProperty("type")>]
    Type : string
    [<JsonProperty("object")>]
    JObject : JObject
    [<JsonProperty("group_id")>]
    GroupId : int64
    [<JsonProperty("secret")>]
    Secret : string
}
    