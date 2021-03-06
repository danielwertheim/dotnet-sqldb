module ``Command name``

open Xunit
open SqlDb    

module ``When converting to a string`` =
    [<Theory>]
    [<InlineData("SomeFooCommand")>]
    let ``It should return the string for`` (v) =
        CommandName v
        |> CommandName.asString
        |> Should.beEqual v
