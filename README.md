# Datacatalog API
For documentation of API endpoints go to http://[DOMAIN]/swagger
OpenAPI json can be found at http://[DOMAIN]/swagger/v[API-VERSION]/swagger.json

# Build, Test and Run
Build command:
```powershell
 dotnet build
```

Run command
```powershell
dotnet run --project .\DataCatalog.Api\DataCatalog.Api.csproj
```
The solution will start the api and show the swagger documentation in a browser instanse.

# Http Status Codes
The API is intented to be RESTfull and statuscodes should fullfill the following diagram:

![HttpStausCodes](https://dev.azure.com/energinet/716a4329-d8fc-4ed1-aa6a-b131bfeb639f/_apis/git/repositories/a0d4dc27-c2a1-4cbb-9f3f-129414a2b2dd/items?path=%2FStatusCodeStateDiagram.png&versionDescriptor%5BversionOptions%5D=0&versionDescriptor%5BversionType%5D=0&versionDescriptor%5Bversion%5D=master&resolveLfs=true&%24format=octetStream&api-version=5.0 "HttpStausCodes")


# API Versioning
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

# Deprecation a API Version
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

Resulting in:

![deprecated-versions](https://dev.azure.com/energinet/716a4329-d8fc-4ed1-aa6a-b131bfeb639f/_apis/git/repositories/a0d4dc27-c2a1-4cbb-9f3f-129414a2b2dd/items?path=%2Fapi-deprecated-versions.png&versionDescriptor%5BversionOptions%5D=0&versionDescriptor%5BversionType%5D=0&versionDescriptor%5Bversion%5D=master&resolveLfs=true&%24format=octetStream&api-version=5.0 "deprecated-versions")

# API documentation by swagger 
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

# TODO:
