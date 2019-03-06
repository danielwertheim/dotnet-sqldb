namespace SqlDb

type CommandName = CommandName of string

module CommandName =
    let asString (CommandName cn) = cn