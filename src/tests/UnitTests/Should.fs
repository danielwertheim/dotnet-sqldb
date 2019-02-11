[<RequireQualifiedAccess>]
module Should

open Xunit
open System

let beEqual<'a> (expected:'a) (actual:'a) =
    Assert.Equal(expected, actual)

let beTrue (actual:bool) =
    Assert.True(actual)

let beFalse (actual:bool) =
    Assert.False(actual)

let throwInvalidArgs (f: unit -> 'a) =
    Assert.Throws<ArgumentException>((fun () -> f() |> ignore))

let throwInvalidArgsNamed paramName (f: unit -> 'a) =
    Assert.Throws<ArgumentException>(paramName, f >> ignore)
