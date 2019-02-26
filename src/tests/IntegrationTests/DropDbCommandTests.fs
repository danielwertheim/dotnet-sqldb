module ``Drop Db command``

open Xunit
open SqlServerDb.Commands

module ``When db does not exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should not fail`` () =
        use session = TestDbSession.beginNew()

        {
            DbDropCommand.Options.ConnectionString = session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString
        }
        |> DbDropCommand.run
        |> Should.beSuccessfulCommand
            
        TestDb.Should.notExist session

module ``When db exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should drop the database`` () =
        use session = TestDbSession.beginNew()
            
        session
        |> TestDb.create
        |> ignore

        {
            DbDropCommand.Options.ConnectionString = session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString
        }
        |> DbDropCommand.run
        |> Should.beSuccessfulCommand
            
        session
        |> TestDb.Should.notExist