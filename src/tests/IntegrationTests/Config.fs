module Config

open Microsoft.Extensions.Configuration

let private readConfig () =
    let builder = new ConfigurationBuilder()
    builder
        .AddIniFile("config.ini", optional=true, reloadOnChange=false)
        .AddIniFile("config.local.ini", optional=true, reloadOnChange=false)
        .AddEnvironmentVariables("DNSQLSERVERDB_")
        .Build()

let private config = readConfig()

let getTestDbConnectionString () =
    config.GetConnectionString "TestDb"