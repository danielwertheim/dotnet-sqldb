module SqlDb.Commands.Command

open SqlDb
open Serilog.Events

let inline tryRun<'opts when 'opts : (member Verbose : bool)> commandName (f: 'opts -> Result<unit, Errors>)  (opts: 'opts) =
    try
        let verbose = ((^opts) : (member Verbose : bool) (opts))
        
        if verbose = true then Logging.overrideLogLevelWith LogEventLevel.Verbose

        f opts
    with ex ->
        Errors.CommandError (commandName, ex) |> Error