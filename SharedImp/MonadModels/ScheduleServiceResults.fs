module SharedImp.MonadModels.ScheduleServiceResults

type QueryError = NotFound | CacheExpired | UnexpectedScheduleServiceError
type ScheduleServiceResult<'TObj> = Result<'TObj, QueryError>