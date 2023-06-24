namespace TelegramBot.Middlewares

open Microsoft.AspNetCore.Http
open Serilog

type ErrorHandlerMiddleware(next:RequestDelegate) =
    member _.Invoke (ctx : HttpContext) = task {
        try
            let! result = next.Invoke ctx
            result
        with ex ->
            Log.ForContext<ErrorHandlerMiddleware>().Error(ex, "An unexpected error occurred while processing request")
            ctx.Response.StatusCode <- 200
            do! ctx.Response.WriteAsync("Ok")
    }