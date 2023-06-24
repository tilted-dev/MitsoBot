module SharedImp.MonadModels.MitsoServiceResults

type ServiceError = ServiceError of string
type ServiceResult<'TObj> = Result<'TObj, ServiceError>
let toString (ServiceError v) = v