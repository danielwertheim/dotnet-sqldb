module SqlServerDb.Program

open System
open CommandLine
open SqlServerDb.Logging
open SqlServerDb.Commands

type private ExitCode =
| Success = 0
| Failure = -1

[<EntryPoint>]
let main argv =
    argv
    |> Parser.Default.ParseArguments<DbDropCommand.Options, DbEnsureCommand.Options, DbUpCommand.Options>
    |> function
        | :? CommandLine.Parsed<obj> as command ->
            match command.Value with
            | :? DbDropCommand.Options as opts -> Command.tryRun DbDropCommand.name opts DbDropCommand.run
            | :? DbEnsureCommand.Options as opts -> Command.tryRun DbEnsureCommand.name opts DbEnsureCommand.run
            | :? DbUpCommand.Options as opts -> Command.tryRun DbUpCommand.name opts DbUpCommand.run
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
                if System.Diagnostics.Debugger.IsAttached then
                    Console.WriteLine("<<Hit key to end>>")
                    Console.ReadKey() |> ignore
                
                Serilog.Log.CloseAndFlush()
                int exitCode