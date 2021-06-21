# Datacatalog API
For documentation of API endpoints go to http://[DOMAIN]/swagger
OpenAPI json can be found at http://[DOMAIN]/swagger/v[API-VERSION]/swagger.json

# Purpose
The Data catalog api is a backend service for the data catalog frontend which can show various datasets and categories of those. 
A dataset is conceptually a set of data points (duh!) which has a number of properties attached to it. Among these we find an owner, 
a category and a list of people which have some type of access to the data.

This project provides the skeleton for managing the datasets, but the data itself is not stored within the data catalog, 
but rather is fetched and stored elsewhere (e.g. within Azure).

The backend service api can be used to define new datasets and categories of datasets along with multiple other metadata. 
Furthermore it is responsible for controlling access to those datasets.

# Environment
Which environment is used on runtime is controlled by the environment variable _ASPNETCORE_ENVIRONMENT_.

The environment determines which particular appsettings.{_environment_}.json is used. Any values within this will override those within appsettings.json. 
Furthermore the environment is included in the category, data contract and data source meta data.

# Local
To run the Api locally, you need to set the environment variable _ASPNETCORE_ENVIRONMENT_ to "Local". This will do the following:

- Disable all security requirements and allow anonymous access to the entire Api with all roles granted.
- Use a dummy implementation of IStorageService which always finds the path required and sets reader and write capabilities for that path.
- Use a dummy implementation of IGroupService which always returns the same local dummy user.
- Use a dummy implementation of IMemberService which always returns the same dummy member

# Project structure

There are four projects within the repository: 
- DataCatalog.Api: The actual Api responsible for running the logic of managing datasets
- DataCatalog.Migrator: A command line tool able to run migrations when needed. Uses the Entity Framework as the migration tool.
- DataCatalog.Common: Shared code between the other projects
- DataCatalog.Data: Initial data required for the skeleton to function. Contains Energinet specific data. 

# Build, Test and Run
Build command:
```powershell
 dotnet build
```

Run command
```powershell
dotnet run --project .\DataCatalog.Api\DataCatalog.Api.csproj
```
The solution will start the api and show the swagger documentation in a browser instance.

# Implementation Details

There are three projects

## Storage
The only current implementation of a storage provider is using the Azure storage provider for storing data.
It uses Azure's own DataLakeServiceClient to access the data.

Creating a new implementation should be fairly straightforward. Just make a new implementation of the interface IStorageService
and configure it in Startup.cs instead of the azure version. Remember to disable azure setup by setting the AzureAD:Enabled to
false in the appsettings.json.

## Identity Provider
Since we use the standardized OAuth2.0/OpenId Connect flow, you just have to configure the OAuth section in the appsettings.json
to your choice of Identity Provider. The only requirement is that the identity provider is able to provide a role claim which you
should configure the name of within the Roles section in appsettings.json.

# Http Status Codes
The API is intended to be RESTfull and status codes should fulfill the following diagram:

![HttpStatusCodes](https://camo.githubusercontent.com/d09839bf7ae593fa403793326a9af335e9392d622f89ea3ee13b889c02ece2fc/68747470733a2f2f7261776769746875622e636f6d2f666f722d4745542f687474702d6465636973696f6e2d6469616772616d2f6d61737465722f6874747064642e706e67 "HttpStausCodes")


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

Resulting in a header named "api-deprecated-versions: 1.0" being sent in the response.

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

# Kubernetes
Helm charts have been created to easily deploy to kubernetes. These are found within the chart folder. 
The dockerfile in the root of the repository needs to be build with the following command:
`docker build -t datacatalog-backend .`

Update the image repository within the chart/values.yaml file to datacatalog-backend.
To then deploy a release to kubernetes run:

``` bash
helm install datacatalog-backend chart/
```