namespace SqlDb

open System
open System.Data
open DbUp.Engine

type private DbDropScript(dbName:MsSql.DbName) =
    interface IScript with
        member __.ProvideScript (commandFactory:Func<IDbCommand>) =
            let dbNameString = dbName |> MsSql.DbName.asString

            use command = commandFactory.Invoke()
            command.CommandType <- CommandType.Text
            command.CommandText <- sprintf """
                IF DB_ID(N'%s') IS NOT NULL BEGIN
                ALTER DATABASE [%s] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
                DROP DATABASE IF EXISTS [%s]
                END""" dbNameString dbNameString dbNameString
            command.ExecuteNonQuery()
            |> ignore
            
            String.Empty

type private DbEnsureScript(dbName:MsSql.DbName) =
    interface IScript with
        member __.ProvideScript (commandFactory:Func<IDbCommand>) =
            let dbNameString = dbName |> MsSql.DbName.asString
            use command = commandFactory.Invoke()
            command.CommandType <- CommandType.Text
            command.CommandText <- sprintf "IF DB_ID (N'%s') IS NULL CREATE DATABASE [%s]" dbNameString dbNameString
            command.ExecuteNonQuery()
            |> ignore
            
            String.Empty

type DbUpScript = string * IScript

module DbUpScript =

    let dropDb dbName : DbUpScript = "DbDropScript", new DbDropScript(dbName) :> IScript

    let ensureDbExists dbName : DbUpScript = "DbEnsureScript", new DbEnsureScript(dbName) :> IScript