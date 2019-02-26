# dotnet-sqlserverdb
Uses [DbUp](https://github.com/dbup/dbup) and [Command Line Parser](https://github.com/commandlineparser/commandline) to offer a simple [DotNet Global Tool](https://docs.microsoft.com/en-us/dotnet/core/tools/global-tools) for applying migration scripts etc.

## Integration tests
The `./.env` file and `./src/tests/IntegrationTests/config.local.ini` files are `.gitignored`. In order to create sample files of these, you can run:

```
./init-local-config.sh
```

### Docker-Compose
There's a `docker-compose.yml` file, that defines usage of an SQL Server instance over port `1401`. The `SA_PASSWORD` is configured via environment key `DNSQLSERVERDB_SA_PWD`, which can either be specified via:

- Environment variable: `DNSQLSERVERDB_SA_PWD`, e.g.:
```
DNSQLSERVERDB_SA_PWD=MyFooPassword
```

- Docker Environment file `./.env` (`.gitignored`), e.g.:
```
DNSQLSERVERDB_SA_PWD=MyFooPassword
```

### Test configuration
A connection string needs to be provided, either via:

- Local-INI-file (`.gitignored`): `./src/tests/IntegrationTests/config.local.ini` containing a connection string section with a `TestDb` node, e.g.:
```
[ConnectionStrings]
TestDb="Server=.,1401;Database=foodb;User ID=sa;Password=MyFooPassword"
```

- Environment variable: `DNSQLSERVERDB_ConnectionStrings__TestDb`, e.g.:

```
DNSQLSERVERDB_ConnectionStrings__TestDb=.,1401;Database=foodb;User ID=sa;Password=MyFooPassword`
```