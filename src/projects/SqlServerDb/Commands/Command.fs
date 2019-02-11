module SqlServerDb.Commands.Command

open SqlServerDb

let tryRun<'opts> commandName opts (f: 'opts -> Result<unit, Errors>) =
    try
        f opts
    with ex ->
        Errors.CommandError (commandName, ex) |> Error