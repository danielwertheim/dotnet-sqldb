module SqlDb.Commands.DbEnsureCommand

open CommandLine
open DbUp
open SqlDb
open SqlDb.Logging

let name = CommandName "EnsureDbCommand"

let private logger = Logging.loggerFor LoggingContexts.DbEnsureCommand

[<Verb("ensure", HelpText = "Ensures a database with the provided name exists.")>]
type Options = {
    [<Option('c', "connectionstring",
        Required = true,
        HelpText = "Connection string for the DB to create if it does not already exist.")>]
    ConnectionString : string

    [<Option("verbose",
        Default = false,
        Required = false,
        HelpText = "Enables verbose output.")>]
    Verbose: bool
}

let run (opts:Options) =
    logger.Debug("Running with Options={@DbEnsureOptions}", opts);

    opts.ConnectionString
    |> MsSql.ConnectionInfo.fromConnectionString
    |> fun cnInfo ->
        DeployChanges.To.SqlDatabase(cnInfo.MasterConnectionString |> MsSql.MasterConnectionString.asString)
        |> DbUpExtensions.useScript (DbUpScript.ensureDbExists (MsSql.DbConnectionString.getDbName cnInfo.DbConnectionString))
        |> DbUpExtensions.useSerilog logger
        |> DbUpExtensions.useNullJournal
        |> DbUpExtensions.build
        |> DbUpExtensions.run