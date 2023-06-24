namespace SharedImp.Services

open System.IO
open Microsoft.Data.Sqlite
open Serilog

type DbSetup =
    static member CreateUsersTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists Users
                        (
                            Id           INTEGER not null,
                            Faculty      TEXT,
                            Form         TEXT,
                            Course       TEXT,
                            GroupName    TEXT,
                            Notification INTEGER,
                            primary key (Id),
                            unique (Id)
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Users` table")
        finally
            dbConnection.Close()
            
    static member CreateScheduleTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists StoredSchedule
                        (
                            Id          integer not null,
                            PeriodStart integer,
                            PeriodEnd   integer,
                            Faculty     TEXT,
                            Form        TEXT,
                            Course      TEXT,
                            GroupName   TEXT,
                            Schedule    TEXT,
                            UpdatedAt   integer,
                            constraint Schedule_pk primary key (Id autoincrement)
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Schedule` table")
        finally
            dbConnection.Close()
    
    static member CreateTeacherScheduleTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists TeacherSchedule
                        (
                            Id          integer not null,
                            PeriodStart integer,
                            PeriodEnd   integer,
                            Surname     TEXT,
                            Schedule    TEXT,
                            UpdatedAt   integer,
                            constraint TeacherSchedule_pk primary key (Id autoincrement)
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Schedule` table")
        finally
            dbConnection.Close()

    static member CreateFacultiesTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists Faculties
                        (
                            Name TEXT
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Faculties` table")
        finally
            dbConnection.Close()

    static member CreateFormsTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists Forms
                        (
                            Faculty TEXT,
                            Name    TEXT
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Forms` table")
        finally
            dbConnection.Close()

    static member CreateCoursesTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists Courses
                        (
                            Faculty TEXT,
                            Form    TEXT,
                            Name    TEXT
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Courses` table")
        finally
            dbConnection.Close()

    static member CreateGroupsTable() =
        let dbConnection = new SqliteConnection "DataSource=db/storage.db;"
        try
            try
                Directory.CreateDirectory "db" |> ignore
                dbConnection.Open()
                let command = dbConnection.CreateCommand()
                command.CommandText <-
                    """create table if not exists Groups
                        (
                            Faculty TEXT,
                            Form    TEXT,
                            Course  TEXT,
                            Name    TEXT
                        );
                    """
                command.ExecuteNonQuery() |> ignore
            with
            | ex -> Log.ForContext<DbSetup>().Error(ex, "Failed to create `Groups` table")
        finally
            dbConnection.Close()