module ``Logging context``

open Xunit
open SqlDb.Logging

module ``When converting to a string`` =
    [<Fact>]
    let ``It should return the string`` () =
        LoggingContexts.DbDropCommand
        |> LoggingContext.asString
        |> Should.beEqual "DbDropCommand"

        LoggingContexts.DbEnsureCommand
        |> LoggingContext.asString
        |> Should.beEqual "DbEnsureCommand"
