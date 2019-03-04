module SqlDb.Commands.Command

open SqlDb

let tryRun<'opts> commandName opts (f: 'opts -> Result<unit, Errors>) =
    try
        f opts
    with ex ->
        Errors.CommandError (commandName, ex) |> Error