module ``Sql Db``

open Xunit
open SqlDb

[<Literal>]
let fooDbCnString = "Data Source=localhost;Initial Catalog=foodb;Integrated Security=True"
    
[<Literal>]
let masterDbCnString = "Data Source=localhost;Initial Catalog=master;Integrated Security=True"

module ``Db name`` =
    module ``When constructing`` =
        [<Theory>]
        [<InlineData("MASTER")>]
        [<InlineData("mAsTeR")>]
        [<InlineData("master")>]
        let ``It should throw for master db`` (v) =
            fun () -> MsSql.DbName.create v
            |> Should.throwInvalidArgsNamed "dbName"
            |> ignore
        
        [<Theory>]
        [<InlineData("master1")>]
        [<InlineData("foo")>]
        [<InlineData("0148213dd2ca4cb5b6c6bb7a33d95201")>]
        let ``It should construct for non master db`` (v) =
            v
            |> MsSql.DbName.create
            |> MsSql.DbName.asString
            |> Should.beEqual v

 module ``Db connection string`` =
    module ``When constructing from connection string`` =
        [<Fact>]
        let ``It should throw for master db`` () =
            fun () ->
                masterDbCnString
                |> MsSql.DbConnectionString.create
            |> Should.throwInvalidArgsNamed "dbName"
        
        [<Fact>]
        let ``It should construct for non master db`` () =
            fooDbCnString
            |> MsSql.DbConnectionString.create
            |> fun cnString ->
                cnString
                |> MsSql.DbConnectionString.getDbName
                |> Should.beEqual (MsSql.DbName.create "foodb")

                cnString
                |> MsSql.DbConnectionString.asString
                |> Should.beEqual fooDbCnString

 module ``Master Db connection string`` =
    module ``When constructing from connection string`` =
        [<Theory>]
        [<InlineData(fooDbCnString)>]
        [<InlineData(masterDbCnString)>]
        let ``It should switch to master db for non master db string and preserve master`` (v) =
            v
            |> MsSql.MasterConnectionString.create
            |> MsSql.MasterConnectionString.asString
            |> Should.beEqual masterDbCnString

module ``Connection info`` =

    module ``When constructing from connection string`` =
        [<Fact>]
        let ``It should throw, when master db is specified`` () =
            fun () ->
                masterDbCnString
                |> MsSql.ConnectionInfo.fromConnectionString
            |> Should.throwInvalidArgsNamed "dbName"
        
        [<Fact>]
        let ``It should construct info from non master db connection string`` () =
            fooDbCnString
            |> MsSql.ConnectionInfo.fromConnectionString
            |> Should.beEqual {
                DbConnectionString = MsSql.DbConnectionString.create fooDbCnString
                MasterConnectionString = MsSql.MasterConnectionString.create masterDbCnString
            }