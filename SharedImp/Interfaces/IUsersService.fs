namespace SharedImp.Interfaces

open SharedImp.MonadModels.UsersServiceResults
open SharedImp.Tables

type IUsersService =
    abstract member GetOrAddUserAsync : id:int64 -> Async<User UsersServiceResult>
    abstract member UpdateUserAsync : user:User -> Async<unit UsersServiceResult>
    abstract member ResetUserAsync : user:User -> Async<unit UsersServiceResult>