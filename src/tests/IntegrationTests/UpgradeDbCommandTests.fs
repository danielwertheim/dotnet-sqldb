module ``Upgrade Db command``

open Xunit

module ``When clean db exist`` =
    open TestEnv
    open SqlDb

    [<Fact>]
    let ``It should apply scripts from specified assembly`` () =
        use session = TestDbSession.beginNew()
        
        session
        |> TestDb.create
        |> TestDb.Should.exist
        |> ignore

        SqlDb.Program.main [|
            "up"
            "-c"
            MsSql.DbConnectionString.asString session.connectionInfo.DbConnectionString
            "-a"
            "IntegrationTests.dll"
        |]
        |> Should.haveSuccessfulExitCode

        session
        |> TestDb.Should.exist
        |> TestDb.Should.haveTable "SchemaVersions"
        |> TestDb.Should.haveTable "Foo"