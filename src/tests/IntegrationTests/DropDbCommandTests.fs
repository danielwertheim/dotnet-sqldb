module ``Drop Db command``

open Xunit

module ``When db does not exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should not fail`` () =
        use session = TestDbSession.beginNew()
        
        TestDb.Should.notExist session |> ignore

        SqlServerDb.Program.main [|
            "drop"
            "-c"
            SqlDb.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode
            
        TestDb.Should.notExist session

module ``When db exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should drop the database`` () =
        use session = TestDbSession.beginNew()
            
        session
        |> TestDb.create
        |> TestDb.Should.exist
        |> ignore

        SqlServerDb.Program.main [|
            "drop"
            "-c"
            SqlDb.DbConnectionString.asString session.connectionInfo.DbConnectionString
        |]
        |> Should.haveSuccessfulExitCode            
        
        TestDb.Should.notExist session