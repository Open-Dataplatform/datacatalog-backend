# Datacatalog API <!-- omit in toc --><!-- omit in toc -->
- [API Documentation](#api-documentation)
- [Development](#development)
	- [Running locally](#running-locally)
	- [Migrations and Seeding](#migrations-and-seeding)
	- [Tests](#tests)
- [API Versioning](#api-versioning)
	- [Deprecation a API Version](#deprecation-a-api-version)
	- [Adding Documentation for a new API Version](#adding-documentation-for-a-new-api-version)

## API Documentation
For documentation of API endpoints go to http://[DOMAIN]/swagger  
OpenAPI json can be found at http://[DOMAIN]/swagger/v[API-VERSION]/swagger.json

## Development

### Running locally
To launch the API from the command line run:
```powershell
dotnet run -p .\src\DataCatalog.Api\
```

Alternatively all C# compatible IDE's should be able to load the project using the .sln file.

### Migrations and Seeding
Generating and applying database migrations are handled by EF Core. The history is located at [src/DataCatalog.Data/](src/DataCatalog.Data/Migrations).

New migrations can be added using the [Entity Framework Core tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) by running the following command from the root folder:

```powershell
dotnet ef migrations add NameOfMigration -p .\src\DataCatalog.Data\ -s .\src\DataCatalog.Migrator\
```

Applying migrations can be done either using the Entity Framework Core tools or by running the [DataCatalog.Migrator console app](src/DataCatalog.Migrator/README.md). In a production environment the migrator app is expected to run before the API is launched for every new release.


### Tests
Two test projects projects are present
- [tests/integration/DataCatalog.Api.IntegrationTests](tests/integration/DataCatalog.Api.IntegrationTests/)
- [tests/unit/DataCatalog.Api.UnitTests/](tests/unit/DataCatalog.Api.UnitTests/)

These can be run using:
```powerchell
dotnet test .\tests\integration\DataCatalog.Api.IntegrationTests\
dotnet test .\tests\unit\DataCatalog.Api.UnitTests\
```

## API Versioning
Api versioning is done with [Microsoft.AspNetCore.Mvc.Versioning](https://github.com/microsoft/aspnet-api-versioning/wiki) as Http header based versioning.
Specifying a version via an Http Header using Api Versioning, allows urls to stay clean without cluttering them with version information.

| Http header   | Value |
|---------------|-------|
| x-api-version | 1.0   |

```
// This controller takes requests on version 1.0 and 1.1
[ApiVersion("1.0")]
[ApiVersion("1.1")]
public class DummyController : ControllerBase
{
	// this Action only replies on version 1.0
	[Route("")]
	[HttpGet]
	public async Task<IActionResult> GetV1_0()
	{
		return StatusCode(200, "Version 1.0");
	}
	
	// this action only replies on version 1.1
	[Route("Extra")]
	[HttpGet, MapToApiVersion("1.1")]
	public async Task<IActionResult> GetV1_1()
	{
		return StatusCode(200, "Version 1.1");
	}	
}
```
### Deprecation a API Version
To advertise that one or more API versions have been deprecated, simply decorate your controller with the deprecated API versions. A deprecated API version does not mean the API version is not supported. A deprecated API version means that the version will become unsupported after six months or more.


```
[ApiVersion( "2.0" )]
[ApiVersion( "1.0", Deprecated = true )]
[ApiController]
[Route( "api/[controller]" )]
public class HelloWorldController : ControllerBase
{
    [HttpGet]
    public string Get() => "Hello world!"

    [HttpGet, MapToApiVersion( "2.0" )]
    public string GetV2() => "Hello world v2.0!";
}
```

### Adding Documentation for a new API Version
Appending Swagger and openApi with new version is done by adding a new SwaggerDoc to options in the SwaggerExtensions class.

```
options.SwaggerDoc("v2.0", new OpenApiInfo
                {
                    Title = "DataCatalog.Api endpoint",
                    Version = "v2.0",
                    Contact = new OpenApiContact()
                    {
                        Name = "Dataplatform team",
                        Email = "dataplatform@energinet.dk"
                    }
                });
```

And adding a new swagger endpoint in the same SwaggerExtensions class.

```
	c.SwaggerEndpoint("/swagger/v2.0/swagger.json", "API v1.1");
```

