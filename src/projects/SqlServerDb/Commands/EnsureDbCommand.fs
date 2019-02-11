module SqlServerDb.Commands.DbEnsureCommand

open CommandLine
open DbUp
open SqlServerDb
open SqlServerDb.Logging

let name = CommandName "EnsureDbCommand"

let private logger = Logging.loggerFor LoggingContexts.DbEnsureCommand

[<Verb("ensure", HelpText = "Ensures a database with the provided name exists.")>]
type Options = {
    [<Option('c', "connectionstring",
        Required = true,
        HelpText = "Connection string for the DB to create if it does not already exist.")>]
    ConnectionString : string
}

let run (opts:Options) =
    logger.Information("Running with Options={@DbEnsureOptions}", opts);

    opts.ConnectionString
    |> SqlDb.ConnectionInfo.fromConnectionString
    |> fun cnInfo ->
        DeployChanges.To.SqlDatabase(cnInfo.MasterConnectionString |> SqlDb.MasterConnectionString.asString)
        |> DbUpExtensions.useScript (DbUpScript.ensureDbExists (SqlDb.DbConnectionString.getDbName cnInfo.DbConnectionString))
        |> DbUpExtensions.useSerilog logger
        |> DbUpExtensions.useNullJournal
        |> DbUpExtensions.build
        |> DbUpExtensions.run