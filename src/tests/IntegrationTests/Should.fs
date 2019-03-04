[<RequireQualifiedAccess>]
module Should

open Xunit.Sdk

let beSuccessfulCommand (r: Result<unit,_>) =
    match r with
    | Ok v -> v
    | err -> raise (new XunitException(sprintf "Command was expected to be Ok, but got error: '%A'" err))

let haveSuccessfulExitCode (exitCode: int) =
    exitCode
    |> enum<SqlDb.Program.ExitCode>
    |> function
        | SqlDb.Program.ExitCode.Success -> ()
        | v -> raise (new XunitException(sprintf "Program execution was expected to be Ok, but got exit code: '%A'" v))
