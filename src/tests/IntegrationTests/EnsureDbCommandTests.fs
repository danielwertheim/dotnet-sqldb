module ``Ensure Db exists command``

open Xunit

module ``When db does not exist`` =
    open TestEnv
    open SqlDb

    [<Fact>]
    let ``It should create the database`` () =
        use session = TestDbSession.beginNew()

        TestDb.Should.notExist session |> ignore

        SqlDb.Program.main [|
            "ensure"
            "-c"
            MsSql.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        TestDb.Should.exist session

module ``When db exist`` =
    open TestEnv
    open SqlDb

    [<Fact>]
    let ``It should not re-create the database`` () =
        use session = TestDbSession.beginNew()
            
        session
        |> TestDb.create
        |> TestDb.createTable session.id
        |> ignore

        SqlDb.Program.main [|
            "ensure"
            "-c"
            MsSql.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        session
        |> TestDb.Should.exist
        |> TestDb.Should.haveTable session.id