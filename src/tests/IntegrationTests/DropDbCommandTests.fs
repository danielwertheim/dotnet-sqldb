module ``Drop Db command``

open Xunit

module ``When db does not exist`` =
    open TestEnv
    open SqlDb

    [<Fact>]
    let ``It should not fail`` () =
        use session = TestDbSession.beginNew()
        
        TestDb.Should.notExist session |> ignore

        SqlDb.Program.main [|
            "drop"
            "-c"
            MsSql.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        TestDb.Should.notExist session

module ``When db exist`` =
    open TestEnv
    open SqlDb

    [<Fact>]
    let ``It should drop the database`` () =
        use session = TestDbSession.beginNew()
            
        session
        |> TestDb.create
        |> TestDb.Should.exist
        |> ignore

        SqlDb.Program.main [|
            "drop"
            "-c"
            MsSql.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode            
        
        TestDb.Should.notExist session