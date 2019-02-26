module ``Upgrade Db command``

open Xunit
open SqlServerDb.Commands

module ``When clean db exist`` =
    open TestEnv
    open SqlServerDb

    [<Fact>]
    let ``It should apply scripts from specified assembly`` () =
        use session = TestDbSession.beginNew()
        
        TestDb.create session |> ignore

        {
            DbUpCommand.Options.ConnectionString = session.connectionInfo.DbConnectionString |> SqlDb.DbConnectionString.asString
            DbUpCommand.Options.Assembly = "IntegrationTests"
        }
        |> DbUpCommand.run
        |> Should.beSuccessfulCommand

        session
        |> TestDb.Should.haveTable "SchemaVersions"
        |> TestDb.Should.haveTable "Foo"