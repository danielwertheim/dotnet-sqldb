module SqlDb.Commands.DbDropCommand

open CommandLine
open DbUp
open SqlDb
open SqlDb.Logging

let name = CommandName "DropDbCommand"

let private logger = Logging.loggerFor LoggingContexts.DbDropCommand

[<Verb("drop", HelpText = "Drops any existing database.")>]
type Options = {
    [<Option('c', "connectionstring",
        Required = true,
        HelpText = "Connection string for the DB to drop. NOTE! Connection will be made against master DB.")>]
    ConnectionString : string

    [<Option("verbose",
        Default = false,
        Required = false,
        HelpText = "Enables verbose output.")>]
    Verbose: bool
}

let run (opts:Options) =
    logger.Debug("Running with Options={@DbDropOptions}", opts);

    opts.ConnectionString
    |> MsSql.ConnectionInfo.fromConnectionString
    |> fun cnInfo ->
        DeployChanges.To.SqlDatabase(cnInfo.MasterConnectionString |> MsSql.MasterConnectionString.asString)
        |> DbUpExtensions.useScript (DbUpScript.dropDb (MsSql.DbConnectionString.getDbName cnInfo.DbConnectionString))
        |> DbUpExtensions.useSerilog logger
        |> DbUpExtensions.useNullJournal
        |> DbUpExtensions.build
        |> DbUpExtensions.run
        