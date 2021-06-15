# Migrator
Running this app will apply any missing migrations to the target database and update seeded data.


## Running locally
To run locally simply navigate to src/DataCatalog.Migrator and run:
```
dotnet run
```

The target database is determined by the ConnectionStrings:DataCatalog environment variable which by default is set to target LocalDB in [appsettings.json](appsettings.json).
