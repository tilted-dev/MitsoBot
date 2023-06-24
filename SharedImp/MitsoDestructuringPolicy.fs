namespace SharedImp

open System.Net.Http
open Newtonsoft.Json

open Serilog.Core
open SharedImp.Tables

type MitsoDestructuringPolicy() =
    static let mutable serializingSettings = JsonSerializerSettings(NullValueHandling = NullValueHandling.Ignore)  
    interface IDestructuringPolicy with
        member this.TryDestructure(value, propertyValueFactory, result) =
            match value with
            | :? User as user ->
                result <- propertyValueFactory.CreatePropertyValue <| JsonConvert.SerializeObject(user, serializingSettings)
                true
            | :? HttpRequestMessage as requestMessage ->
                result <- propertyValueFactory.CreatePropertyValue <| JsonConvert.SerializeObject(requestMessage.RequestUri.ToString(), serializingSettings)
                true
            | _ -> false