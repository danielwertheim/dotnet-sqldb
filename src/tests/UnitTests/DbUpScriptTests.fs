module ``DbUp script``

open Xunit
open SqlDb

let fooDb = MsSql.DbName.create "fooDb"

module ``When creating drop Db script`` =

    [<Fact>]
    let ``It should return a script with corresponding name`` () =
        DbUpScript.dropDb fooDb
        |> fun (n, _) -> n |> Should.beEqual "DbDropScript"

module ``When creating ensure Db script`` =
    [<Fact>]
    let ``It should return a script with corresponding name`` () =
        DbUpScript.ensureDbExists fooDb
        |> fun (n, _) -> n |> Should.beEqual "DbEnsureScript"
