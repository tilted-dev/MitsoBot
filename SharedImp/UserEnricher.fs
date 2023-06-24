namespace SharedImp

open Serilog.Core

type UserEnricher() =
    interface ILogEventEnricher with
        member this.Enrich(logEvent, propertyFactory) =
            logEvent.AddPropertyIfAbsent <| propertyFactory.CreateProperty("User", "Bot")
            
            