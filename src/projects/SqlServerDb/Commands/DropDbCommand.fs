module SqlServerDb.Commands.DbDropCommand

open CommandLine
open DbUp
open SqlServerDb
open SqlServerDb.Logging

let name = CommandName "DropDbCommand"

let private logger = Logging.loggerFor LoggingContexts.DbDropCommand

[<Verb("drop", HelpText = "Drops any existing database.")>]
type Options = {
    [<Option('c', "connectionstring",
        Required = true,
        HelpText = "Connection string for the DB to drop. NOTE! Connection will be made against master DB.")>]
    ConnectionString : string
}

let run (opts:Options) =
    logger.Information("Running with Options={@DbDropOptions}", opts);

    opts.ConnectionString
    |> SqlDb.ConnectionInfo.fromConnectionString
    |> fun cnInfo ->
        DeployChanges.To.SqlDatabase(cnInfo.MasterConnectionString |> SqlDb.MasterConnectionString.asString)
        |> DbUpExtensions.useScript (DbUpScript.dropDb (SqlDb.DbConnectionString.getDbName cnInfo.DbConnectionString))
        |> DbUpExtensions.useSerilog logger
        |> DbUpExtensions.useNullJournal
        |> DbUpExtensions.build
        |> DbUpExtensions.run
        