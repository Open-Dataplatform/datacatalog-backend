# Migrator
Running this app will apply any missing migrations to the target database and update seeded data.


## Running locally
To run locally simply navigate to src/DataCatalog.Migrator and run:
```
dotnet run
```

The target database is determined by the *ConnectionStrings:DataCatalog* environment variable which by default is set to target a server on localhost in [appsettings.json](appsettings.json).

## Seeding
[SeedLogic.cs](SeedLogic.cs) contains data which is added to the database when this app runs. Most of this as Energinet specific but can be changed to fit what is needed.