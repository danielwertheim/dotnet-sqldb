module SqlDb.Commands.DbUpCommand

open System.Reflection
open CommandLine
open DbUp
open SqlDb
open SqlDb.Logging

let name = CommandName "UpgradeDbCommand"

let private logger = Logging.loggerFor LoggingContexts.DbUpCommand

[<Verb("up", HelpText = "Upgrades specified DB with migration scripts.")>]
type Options = {
    [<Option('c', "connectionstring",
        Required = true,
        HelpText = "Connection string for the DB to upgrade.")>]
    ConnectionString : string

    [<Option('a', "assembly",
        Required = true,
        HelpText = "Assembly with embedded SQL scripts to be applied.")>]
    Assembly : string
    
    [<Option("verbose",
        Default = false,
        Required = false,
        HelpText = "Enables verbose output.")>]
    Verbose: bool
}

let run (opts:Options) =
    logger.Debug("Running with Options={@DbUpOptions}", opts);

    opts.ConnectionString
    |> MsSql.ConnectionInfo.fromConnectionString
    |> fun cnInfo ->
        DeployChanges.To.SqlDatabase(cnInfo.DbConnectionString |> MsSql.DbConnectionString.asString)
        |> DbUp'.useScriptsInAssembly (Assembly.LoadFrom(opts.Assembly))
        |> DbUp'.useSerilog logger
        |> DbUp'.logScriptOutput
        |> DbUp'.build
        |> DbUp'.run