module ``Upgrade Db command``

open Xunit

module ``When clean db exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should apply scripts from specified assembly`` () =
        use session = TestDbSession.beginNew()
        
        session
        |> TestDb.create
        |> TestDb.Should.exist
        |> ignore

        SqlServerDb.Program.main [|
            "up"
            "-c"
            SqlDb.DbConnectionString.asString session.connectionInfo.DbConnectionString
            "-a"
            "IntegrationTests"
        |]
        |> Should.haveSuccessfulExitCode

        session
        |> TestDb.Should.exist
        |> TestDb.Should.haveTable "SchemaVersions"
        |> TestDb.Should.haveTable "Foo"