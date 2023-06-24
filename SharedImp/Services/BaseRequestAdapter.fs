namespace SharedImp.Services

open System
open System.Collections.Generic
open Microsoft.Extensions.DependencyInjection
open Serilog
open SharedImp.Enums
open SharedImp.Interfaces
open SharedImp.Models
open SharedImp.Tables

type BaseRequestAdapter() =
    [<DefaultValue>] val mutable serviceProvider : IServiceProvider
    let textMessagesHandlers = Dictionary<string list, Type>()
    let buttonPayloadHandlers = Dictionary<ActionType, Type>()
    interface IRequestAdapter with
        member this.ProcessRequest requestData = (async {
            match requestData.MessageType with
            | MessageType.Text -> do! this.TryProcessTextMessageHandler requestData
            | MessageType.Payload -> do! this.TryProcessButtonPayloadHandler requestData
            | _ -> Log.ForContext<BaseRequestAdapter>().Error("Unknown message type. Check `MessageType` value")
        } |> Async.StartAsTask |> ignore)

    member private this.TryProcessTextMessageHandler(requestData:RequestData) = async {
        try
            let commandParts = requestData.Content.Split(" ")
            let name = (commandParts |> Seq.head).Trim().ToLower()
            let argument = (commandParts |> Seq.skip 1) |> String.concat " "
            match textMessagesHandlers |> Seq.tryFind(fun x -> x.Key |> Seq.contains name) with
            | Some kv ->
                Log.ForContext("SourceContext", kv.Value.GenericTypeArguments |> Seq.head).ForContext("User", requestData.User, true).Information("Processing text message handler")
                requestData.Content <- argument
                do! (this.serviceProvider.GetService(kv.Value) :?> ITextCommandHandler).ProcessCommand requestData
            | None -> Log.ForContext<BaseRequestAdapter>().Warning("Unknown text command: {Command}", requestData.Content)
        with ex -> Log.ForContext<BaseRequestAdapter>().Error(ex, "Unexpected error occurred while processing text message handler")
    }

    member private this.TryProcessButtonPayloadHandler(requestData:RequestData) = async {
        try
            let payloadParts = requestData.Content.Split(".")
            let actionType = Enum.Parse<ActionType>(payloadParts |> Seq.head, true)
            let data = payloadParts |> Seq.last
            match buttonPayloadHandlers.TryGetValue actionType with
            | true, handler ->
                Log.ForContext("SourceContext", handler.GenericTypeArguments |> Seq.head).ForContext("User", requestData.User, true).Information("Processing button payload handler")
                requestData.Content <- data
                do! (this.serviceProvider.GetService(handler) :?> IButtonPayloadHandler).ProcessPayload requestData
            | false, _ ->
                do! this.HandleUnknownButtonPayload requestData.User
                Log.ForContext<BaseRequestAdapter>().Warning("Unknown payload: {Content}", requestData.Content)
        with ex -> Log.ForContext<BaseRequestAdapter>().Error(ex, "Unexpected error occurred while processing button payload handler")
    }

    member this.RegisterTextMessageHandler<'THandler when 'THandler :> 'THandler IGenericTextCommandHandler>([<ParamArray>] keys: string array) =
        textMessagesHandlers.Add(keys |> Seq.toList, typeof<'THandler IGenericTextCommandHandler>)

    member this.RegisterButtonPayloadHandler<'THandler when 'THandler :> 'THandler IGenericButtonPayloadHandler>(key:ActionType) =
        buttonPayloadHandlers.Add(key, typeof<'THandler IGenericButtonPayloadHandler>)
    
    member this.RegisterHandlersForRequestAdapter(serviceCollection:IServiceCollection) =
        for textHandler in textMessagesHandlers.Values do
            let implementation = textHandler.GetGenericArguments() |> Seq.head
            serviceCollection.AddScoped(textHandler, implementation) |> ignore
        for buttonHandler in buttonPayloadHandlers.Values do
            let implementation = buttonHandler.GetGenericArguments() |> Seq.head
            serviceCollection.AddScoped(buttonHandler, implementation) |> ignore
        
        this.serviceProvider <- serviceCollection.BuildServiceProvider()
    
    abstract member HandleUnknownButtonPayload: user:User -> Async<unit>
    default this.HandleUnknownButtonPayload(_:User) = async {
        ()
    }