module Config

open Microsoft.Extensions.Configuration

let private readConfig () =
    let builder = new ConfigurationBuilder()
    builder
        .AddIniFile("integrationtests.local.ini", optional=true, reloadOnChange=false)
        .AddEnvironmentVariables("DNSQLDB_")
        .Build()

let private config = readConfig()

let getTestDbConnectionString () =
    config.GetConnectionString "TestDb"