module ``Sql Db``

open Xunit
open SqlServerDb

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
            fun () -> SqlDb.DbName.create v
            |> Should.throwInvalidArgsNamed "dbName"
            |> ignore
        
        [<Theory>]
        [<InlineData("master1")>]
        [<InlineData("foo")>]
        [<InlineData("0148213dd2ca4cb5b6c6bb7a33d95201")>]
        let ``It should construct for non master db`` (v) =
            v
            |> SqlDb.DbName.create
            |> SqlDb.DbName.asString
            |> Should.beEqual v

 module ``Db connection string`` =
    module ``When constructing from connection string`` =
        [<Fact>]
        let ``It should throw for master db`` () =
            fun () ->
                masterDbCnString
                |> SqlDb.DbConnectionString.create
            |> Should.throwInvalidArgsNamed "dbName"
        
        [<Fact>]
        let ``It should construct for non master db`` () =
            fooDbCnString
            |> SqlDb.DbConnectionString.create
            |> fun cnString ->
                cnString
                |> SqlDb.DbConnectionString.getDbName
                |> Should.beEqual (SqlDb.DbName.create "foodb")

                cnString
                |> SqlDb.DbConnectionString.asString
                |> Should.beEqual fooDbCnString

 module ``Master Db connection string`` =
    module ``When constructing from connection string`` =
        [<Theory>]
        [<InlineData(fooDbCnString)>]
        [<InlineData(masterDbCnString)>]
        let ``It should switch to master db for non master db string and preserve master`` (v) =
            v
            |> SqlDb.MasterConnectionString.create
            |> SqlDb.MasterConnectionString.asString
            |> Should.beEqual masterDbCnString

module ``Connection info`` =

    module ``When constructing from connection string`` =
        [<Fact>]
        let ``It should throw, when master db is specified`` () =
            fun () ->
                masterDbCnString
                |> SqlDb.ConnectionInfo.fromConnectionString
            |> Should.throwInvalidArgsNamed "dbName"
        
        [<Fact>]
        let ``It should construct info from non master db connection string`` () =
            fooDbCnString
            |> SqlDb.ConnectionInfo.fromConnectionString
            |> Should.beEqual {
                DbConnectionString = SqlDb.DbConnectionString.create fooDbCnString
                MasterConnectionString = SqlDb.MasterConnectionString.create masterDbCnString
            }