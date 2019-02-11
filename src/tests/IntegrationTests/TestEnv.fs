module TestEnv

open System.Data.SqlClient
open SqlServerDb
open System.Data
open System

type TestDbSession private (id, cnInfo:SqlDb.ConnectionInfo, onComplete: TestDbSession -> unit) =
    member ___.id = id
    member ___.dbName =
        cnInfo.DbConnectionString
        |> SqlDb.DbConnectionString.getDbName
        |> SqlDb.DbName.asString

    member __.connectionInfo = cnInfo

    interface IDisposable with
        member this.Dispose() =
            onComplete(this)

    static member private cleanUp (session: TestDbSession) =
        use cn = new SqlConnection(session.connectionInfo.MasterConnectionString |> SqlDb.MasterConnectionString.asString)
        cn.Open()
        try
            use command = cn.CreateCommand()
            command.CommandType <- CommandType.Text
            command.CommandText <- sprintf """
                IF DB_ID(N'%s') IS NOT NULL BEGIN
                ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE IF EXISTS [%s]
                END""" session.dbName session.dbName session.dbName
            command.ExecuteNonQuery() |> ignore
        finally
            cn.Close()

    static member beginNew () =
        let id = Guid.NewGuid().ToString("N")
        let cnInfo =
            Config.getTestDbConnectionString()
            |> fun cnString ->
                let x = new SqlConnectionStringBuilder(cnString)
                x.InitialCatalog <- sprintf "%s_%s" x.InitialCatalog id
                x.ToString()
            |> SqlDb.ConnectionInfo.fromConnectionString

        new TestDbSession(id, cnInfo, TestDbSession.cleanUp)

module TestDb =
    let create (session: TestDbSession) =
        use cn = new SqlConnection(session.connectionInfo.MasterConnectionString |> SqlDb.MasterConnectionString.asString)
        cn.Open()
        try
            use command = cn.CreateCommand()
            command.CommandType <- CommandType.Text
            command.CommandText <- sprintf """
                CREATE DATABASE %s
                ALTER DATABASE %s SET RECOVERY SIMPLE""" session.dbName session.dbName
            command.ExecuteNonQuery() |> ignore
        finally
            cn.Close()
        
        session

    let createTable name (session: TestDbSession) =
        use cn = new SqlConnection(session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString)
        cn.Open()
        try
            use command = cn.CreateCommand()
            command.CommandType <- CommandType.Text
            command.CommandText <- sprintf "CREATE TABLE dbo.[%s] (Id int PRIMARY KEY) " name
            command.ExecuteNonQuery() |> ignore
        finally
            cn.Close()
        session

    [<RequireQualifiedAccess>]
    module Should =
        open Xunit.Sdk

        let private dbExists (session: TestDbSession) =
            use cn = new SqlConnection(session.connectionInfo.MasterConnectionString |> SqlDb.MasterConnectionString.asString)
            cn.Open()
            try
                use cmd = cn.CreateCommand()
                cmd.CommandType <- CommandType.Text
                cmd.CommandText <- "SELECT COALESCE(DB_ID(@dbName),-1)"
                cmd.Parameters.AddWithValue("dbName", session.dbName) |> ignore
                cmd.ExecuteScalar()
            finally
                cn.Close()
            |> fun r ->
                session.dbName,(r :?> int) <> -1

        let private tableExists tableName (session: TestDbSession) =
            use cn = new SqlConnection(session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString)
            cn.Open()
            try
                use cmd = cn.CreateCommand()
                cmd.CommandType <- CommandType.Text
                cmd.CommandText <- "SELECT COALESCE(OBJECT_ID(@tableName, N'U'),-1)"
                cmd.Parameters.AddWithValue("tableName", tableName) |> ignore
                cmd.ExecuteScalar()
            finally
                cn.Close()
            |> fun r ->
                tableName,(r :?> int) <> -1

        let exist session =
            session
            |> dbExists
            |> function
            | _, true -> session
            | dbName, false -> raise (XunitException (sprintf "Db '%A' was expected to exist." dbName))

        let notExist session =    
            session
            |> dbExists
            |> function
            | dbName, true -> raise (XunitException (sprintf "Db '%A' was not expected to exist." dbName))
            | _, false -> session

        let haveTable name session =
            session
            |> tableExists name
            |> function
            | _, true -> session
            | tableName, false -> raise (XunitException (sprintf "Table '%A' was expected to exist." tableName))