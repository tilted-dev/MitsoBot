namespace SharedImp

open Microsoft.Data.Sqlite
open Quartz
open Serilog

type CleanDbJob() =
    interface IJob with
        member this.Execute _ = task {
            let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
            try
                try
                    dbConnection.Open()
                    let command = dbConnection.CreateCommand()
                    command.CommandText <-
                        """
                           DELETE FROM Faculties;
                           DELETE FROM Forms;
                           DELETE FROM Courses;
                           DELETE FROM Groups;
                           DELETE FROM StoredSchedule;
                           DELETE FROM TeacherSchedule;
                        """
                    do! command.ExecuteNonQueryAsync() |> Async.AwaitIAsyncResult |> Async.Ignore
                    Log.ForContext<CleanDbJob>().Information("Successfully clean cached data")
                with
                | ex -> Log.ForContext<CleanDbJob>().Error(ex, "Quartz task finished with error")
            finally
                dbConnection.Close()
        }
