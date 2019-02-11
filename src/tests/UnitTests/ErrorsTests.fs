module ``Error message``

open Xunit
open SqlServerDb
open System

module ``When converting to a string`` =
    [<Fact>]
    let ``It should return the string`` () =
        let v = Guid.NewGuid().ToString("N")

        ErrorMessage v
        |> ErrorMessage.asString
        |> Should.beEqual v 
