module ``Ensure Db exists command``

open Xunit
open SqlServerDb.Commands

module ``When db does not exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should create the database`` () =
        use session = TestDbSession.beginNew()

        {
            DbEnsureCommand.Options.ConnectionString = session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString
        }
        |> DbEnsureCommand.run
        |> Should.beSuccessfulCommand
            
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

        {
            DbEnsureCommand.Options.ConnectionString = session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString
        }
        |> DbEnsureCommand.run
        |> Should.beSuccessfulCommand
            
        session
        |> TestDb.Should.exist
        |> TestDb.Should.haveTable session.id