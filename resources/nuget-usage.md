## Global installation
```
dotnet tool install -g dotnet-sqldb
```

Usage either: `dotnet-sqldb` or `dotnet sqldb`

## Help

```
dotnet-sqldb --help
```

## EnsureDb
Ensures that the specified DB exists. If it does not exist, it gets created.

```
dotnet-sqldb ensure [--connectionstring|-c] mycnstring
```

## DropDb
Drops the specified database if it exists.

```
dotnet-sqldb drop [--connectionstring|-c] mycnstring
```

## UpgradeDb
Upgrades the database by applying SQL-scripts using [DbUp](https://github.com/dbup/dbup)

```
dotnet-sqldb up [--connectionstring|-c] mycnstring [--assembly|-a] myassembly_with_embedded_scripts
```
