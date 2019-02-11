[<RequireQualifiedAccess>]
module Should

open Xunit.Sdk

let beSuccessfulCommand (r: Result<unit,_>) =
    match r with
    | Ok v -> v
    | err -> raise (new XunitException(sprintf "Command was expected to be Ok, but got error: '%A'" err))
