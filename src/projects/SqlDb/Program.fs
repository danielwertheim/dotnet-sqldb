module SqlDb.Program

open CommandLine
open SqlDb.Logging
open SqlDb.Commands
open Serilog.Events

type ExitCode =
| Success = 0
| Failure = -1

[<EntryPoint>]
let main argv =
    #if DEBUG
    Logging.overrideLogLevelWith LogEventLevel.Debug
    #endif

    argv
    |> Parser.Default.ParseArguments<DbDropCommand.Options, DbEnsureCommand.Options, DbUpCommand.Options>
    |> function
        | :? CommandLine.Parsed<obj> as command ->
            match command.Value with
            | :? DbDropCommand.Options as opts -> Command.tryRun DbDropCommand.name DbDropCommand.run opts
            | :? DbEnsureCommand.Options as opts -> Command.tryRun DbEnsureCommand.name DbEnsureCommand.run opts
            | :? DbUpCommand.Options as opts -> Command.tryRun DbUpCommand.name DbUpCommand.run opts
            | opts ->
                logger.Error("Parsed UnknownOptions={@UnknownOptions}", opts)
                Errors.GenericError (ErrorMessage "Parser error") |> Result.Error
        | :? CommandLine.NotParsed<obj> -> Result.Ok ()
        | _ -> failwith "Parsing ended up in unhandled case."
        |> function
            | Result.Ok _ -> ExitCode.Success
            | Result.Error err ->
                match err with
                | Errors.GenericError msg -> logger.Error(msg |> ErrorMessage.asString)
                | Errors.DbUpEngineError (msg, ex) -> logger.Error(ex, msg |> ErrorMessage.asString)
                | Errors.CommandError (name, ex) -> logger.Error(ex, sprintf "Error while running command '%s'." (name |> CommandName.asString))
                |> fun () -> ExitCode.Failure
            |> fun exitCode ->
                Serilog.Log.CloseAndFlush()
                int exitCode