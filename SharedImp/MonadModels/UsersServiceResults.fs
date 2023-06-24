module SharedImp.MonadModels.UsersServiceResults

open SharedImp.Resources

type UsersServiceError = UsersServiceError
type UsersServiceResult<'TObj> = Result<'TObj, UsersServiceError>
let toString(_error:UsersServiceError) = TextMessages.UserServiceError
