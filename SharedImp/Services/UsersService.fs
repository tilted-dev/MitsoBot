namespace SharedImp.Services

open Dapper.FSharp.SQLite
open Microsoft.Data.Sqlite
open Serilog
open SharedImp.Interfaces
open SharedImp.MonadModels.UsersServiceResults
open SharedImp.Tables

type UsersService() =
    let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
    let mutable logger = Log.ForContext<UsersService>()
    let usersTable = table'<User> "Users"
    interface IUsersService with
        member this.GetOrAddUserAsync(id) = async {
            logger <- logger.ForContext("User", id, true)
            try
                let! result = (select {
                    for user in usersTable do
                        where (user.Id = id)
                } |> dbConnection.SelectAsync<User> |> Async.AwaitTask)
                
                if result |> Seq.isEmpty then
                    logger.Information("User not exists. New one will be created")
                    let user = {
                        Id = id
                        Faculty = null
                        Form = null
                        Course = null
                        GroupName = null
                        Notification = false
                    }
                    let! _ = (insert {
                        for userRow in usersTable do
                        value user
                    } |> dbConnection.InsertAsync |> Async.AwaitTask)
                    return Ok user
                else
                    logger.Information("Successfully received user from DB")
                    return Ok (result |> Seq.head)
            with ex ->
                logger.Error(ex, "[UsersServiceError] ==> An unexpected error occurred while executing task")
                return Error UsersServiceError
        }
        
        member this.ResetUserAsync(sharedUser) = async {
            logger <- logger.ForContext("User", sharedUser, true)
            try
                let! _ = (update {
                    for user in usersTable do
                        setColumn user.GroupName null
                        setColumn user.Faculty null
                        setColumn user.Form null
                        setColumn user.Course null
                        setColumn user.Notification false
                        where (user.Id = sharedUser.Id)
                } |> dbConnection.UpdateAsync<User> |> Async.AwaitTask)
                logger.Information("Successfully reset user")
                return Ok ()
            with ex ->
                logger.Error(ex, "[UsersServiceError] ==> An unexpected error occurred while executing task")
                return Error UsersServiceError
        }
        
        member this.UpdateUserAsync(sharedUser) = async {
            logger <- logger.ForContext("User", sharedUser, true)
            try
                let! _ = (update {
                    for user in usersTable do
                        set sharedUser
                        excludeColumn user.Id
                        where (user.Id = sharedUser.Id)
                } |> dbConnection.UpdateAsync<User> |> Async.AwaitTask)
                logger.Information("Successfully updated user")
                return Ok ()
            with ex ->
                logger.Error(ex, "[UsersServiceError] ==> An unexpected error occurred while executing task")
                return Error UsersServiceError
        }