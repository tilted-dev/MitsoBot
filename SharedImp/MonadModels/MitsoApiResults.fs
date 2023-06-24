module SharedImp.MonadModels.MitsoApiResults

open Microsoft.FSharp.Core

type FetchError = ApiError | NoContent | UnexpectedMitsoApiServiceError
type FetchResult<'TObj> = Result<'TObj, FetchError>