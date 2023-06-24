namespace SharedImp.Utils

open System.Runtime.CompilerServices
open Microsoft.Extensions.DependencyInjection
open SharedImp.Services

[<Extension>]
type DiExtensions() =
    [<Extension>]
    static member RegisterHandlersForRequestAdapter(serviceCollection:IServiceCollection, requestAdapter:BaseRequestAdapter) =
        requestAdapter.RegisterHandlersForRequestAdapter(serviceCollection)