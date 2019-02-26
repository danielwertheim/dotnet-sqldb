[<RequireQualifiedAccess>]
module SqlServerDb.SqlDb

open System
open System.Data.SqlClient

type DbName = private DbName of string
type DbConnectionString = private DbConnectionString of DbName*string
type MasterConnectionString = private MasterConnectionString of string

type ConnectionInfo = {   
    DbConnectionString: DbConnectionString
    MasterConnectionString: MasterConnectionString
}

module DbName =
    let private isMasterDb name =
        String.Equals (name, "master", StringComparison.OrdinalIgnoreCase)

    let create (dbName: string) =
        if (dbName |> isMasterDb) then
            invalidArg "dbName" "Invalid db name specified. Can not be against master db."

        DbName dbName
    
    let asString (DbName v) = v

module DbConnectionString =
    let create connectionString =
        let builder = new SqlConnectionStringBuilder(connectionString)
        
        let dbName = DbName.create builder.InitialCatalog

        DbConnectionString (dbName,builder.ToString())
    
    let getDbName (DbConnectionString (dbName, _)) = dbName

    let asString (DbConnectionString (_, cnString)) = cnString

module MasterConnectionString =
    let create connectionString =
        let builder = new SqlConnectionStringBuilder(connectionString)
        
        builder.InitialCatalog <- "master"

        MasterConnectionString <| builder.ToString()
    
    let asString (MasterConnectionString v) = v

module ConnectionInfo =

    let fromConnectionString connectionString = {
        DbConnectionString = connectionString |> DbConnectionString.create
        MasterConnectionString = connectionString |> MasterConnectionString.create
    }