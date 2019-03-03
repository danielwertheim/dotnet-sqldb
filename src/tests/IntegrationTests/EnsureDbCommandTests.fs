module ``Ensure Db exists command``

open Xunit

module ``When db does not exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should create the database`` () =
        use session = TestDbSession.beginNew()

        TestDb.Should.notExist session |> ignore

        SqlServerDb.Program.main [|
            "ensure"
            "-c"
            SqlDb.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        TestDb.Should.exist session

module ``When db exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should not re-create the database`` () =
        use session = TestDbSession.beginNew()
            
        session
        |> TestDb.create
        |> TestDb.createTable session.id
        |> ignore

        SqlServerDb.Program.main [|
            "ensure"
            "-c"
            SqlDb.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        session
        |> TestDb.Should.exist
        |> TestDb.Should.haveTable session.id