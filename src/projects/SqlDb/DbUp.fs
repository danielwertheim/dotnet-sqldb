module SqlDb.DbUp'

open DbUp.Engine.Output
open DbUp.Builder
open DbUp.Engine
open DbUp.Helpers
open Serilog

type private DbUpLogger(logger:ILogger) =
    interface IUpgradeLog with
        member __.WriteInformation (format:string, [<System.ParamArray>]args:obj[]) =
            logger.Information(format, args)
        member __.WriteWarning (format:string, [<System.ParamArray>]args:obj[]) =
            logger.Warning(format, args)
        member __.WriteError (format:string, [<System.ParamArray>]args:obj[]) =
            logger.Error(format, args)

let useSerilog (logger:ILogger) (builder: UpgradeEngineBuilder) =
    builder.LogTo (new DbUpLogger(logger))

let useNullJournal (builder:UpgradeEngineBuilder) =
    builder.JournalTo(new NullJournal())

let useScript ((name, script):DbUpScript) (builder: UpgradeEngineBuilder) =
    builder.WithScript(name, script)

let useScriptsInAssembly assembly (builder: UpgradeEngineBuilder) =
    builder.WithScriptsEmbeddedInAssembly(assembly)

let run (engine: UpgradeEngine) : Result<_, Errors> =
    let result = engine.PerformUpgrade()

    match result.Successful with
    | true -> Ok()
    | _ -> Errors.DbUpEngineError (
            result.Error.Message |> ErrorMessage,
            result.Error) |> Error

let build (builder:UpgradeEngineBuilder) =
    builder.Build()