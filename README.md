# dotnet-sqldb
Uses [DbUp](https://github.com/dbup/dbup) and [Command Line Parser](https://github.com/commandlineparser/commandline) to offer a simple [DotNet Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for applying migration scripts etc. against a SQL-Server database.

[![Build status](https://dev.azure.com/danielwertheim/dotnet-sqldb/_apis/build/status/dotnet-sqldb-CI-Ubuntu)](https://dev.azure.com/danielwertheim/dotnet-sqldb/_build/latest?definitionId=5) Ubuntu

**Note:** It's your data. Use at your own risk. As [the license](https://github.com/danielwertheim/dotnet-sqldb/blob/master/LICENSE) (MIT) says: *"THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND..."*

## Installation
It's a DotNet Global Tool, distributed via NuGet.

### Global installation
To install it globally (machine), you use the [dotnet tool install command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) with the `-g` switch:

```
dotnet tool install -g dotnet-sqldb
```

After that you can start using it (you might need to restart your prompt of choice):

```
dotnet sqldb --help
```

### Local installation
To install it locally, you use the [dotnet tool install command](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-tool-install) with the `--tool-path` switch:

```
dotnet tool install dotnet-sqldb --tool-path /path/for/tool
```

To use it you will need to include it in the current environment path.

## Commands
The following commands are supported:

- **Ensure:** Ensures that the specified DB exists. If it does not exist, it gets created.
```
dotnet sqldb ensure [--connectionstring|-c]=mycnstring
```

- **Drop:** Drops the specified database if it exists
```
dotnet sqldb drop [--connectionstring|-c]=mycnstring
```

- **Up:** Upgrades the database by applying SQL-scripts using [DbUp](https://github.com/dbup/dbup)
```
dotnet sqldb up [--connectionstring|-c]=mycnstring [--assembly|-a]=myassembly_with_embedded_scripts
```

## Integration tests
The `./.env` file and `./src/tests/IntegrationTests/integrationtests.local.ini` files are `.gitignored`. In order to create sample files of these, you can run:

```
. init-local-config.sh
```

### Docker-Compose
There's a `docker-compose.yml` file, that defines usage of an SQL Server instance over port `1401`. The `SA_PASSWORD` is configured via environment key `DNSQLDB_SA_PWD`, which can either be specified via:

- Environment variable: `DNSQLDB_SA_PWD`, e.g.:
```
DNSQLDB_SA_PWD=MyFooPassword
```

- Docker Environment file `./.env` (`.gitignored`), e.g.:
```
DNSQLDB_SA_PWD=MyFooPassword
```

### Test configuration
A connection string needs to be provided, either via:

- Local-INI-file (`.gitignored`): `./src/tests/IntegrationTests/integrationtests.local.ini` containing a connection string section with a `TestDb` node, e.g.:
```
[ConnectionStrings]
TestDb="Server=.,1401;Database=foodb;User ID=sa;Password=MyFooPassword"
```

- Environment variable: `DNSQLDB_ConnectionStrings__TestDb`, e.g.:

```
DNSQLDB_ConnectionStrings__TestDb=Server=.,1401;Database=foodb;User ID=sa;Password=MyFooPassword
```
