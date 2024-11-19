# Altemiq.AspNetCore.OpenApi

This contains methods to extend `Microsoft.AspNetCore.OpenApi` with extra functionality

## OpenApi

### `SetInfo`

This sets the information for the document

````csharp
services.AddOpenApi(options => options.SetInfo("API Title"));
````

results in 

```xml
"info": {
    "title": "API Title | v1",
    "version": "1.0.0.0"
}
```

### `UsePathBase`

This sets the path base for the service section

````csharp
services.AddOpenApi(options => options.UsePathBase("/api"));
````

results in 

```xml
"servers": [
    {
        "url": "/api"
    }
]
```

### Add* (SecurityScheme)

This adds and returns the added security scheme

#### `AddApiKey`

```csharp
services.AddOpenApi(options => options.AddApiKey());
```

#### `AddHttp`

```csharp
services.AddOpenApi(options => options.AddHttp());
```

#### `AddOAuth2`

```csharp
services.AddOpenApi(options => 
{
    var scheme = options.AddOAuth2(
        new Microsoft.OpenApi.Models.OpenApiOAuthFlows
        {
            Password = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
            {
                Scopes =
                {
                    ["first"] = "First Scope",
                    ["second"] = "Second Scope",
                    ["third"] = "Third Scope",
                    ["forth"] = "Forth Scope",
                },
            },
        });
});
```

#### `AddOpenIdConnect`

```csharp
services.AddOpenApi(options => options.AddOpenIdConnect("http://auth.local.dev/.well-known/openid-configuration"));
```

### `WithAuthorizeCheck`

This adds a check to see if the end point has any `IAuthorizeData` metadata, and if so, adds the standard responses. This also adds the specified security scheme to the end point, and also configures the requirements for the scheme.

```csharp
services.AddOpenApi(options => options.WithAuthorizeCheck(securityScheme));
```

This takes into account the scopes specified if the scheme is either `OAuth2` or `OpenIdConnect`

## HTTP

This extends `IEndpointConventionBuilder` to include extra metadata

### `WithScopes` / `ScopesAttribute`

Adds scopes to the authorization.

```csharp
app
    .MapGet("requires-scopes", () => "Requires Scopes")
    .RequireAuthorization()
    .WithScopes("first", "second");
```

```csharp
[ApiController]
[Route("controllers/[controller]")]
public class WeatherForecastController : ControllerBase
{
    [HttpGet(Name = "GetWeatherForecast")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Scopes("third", "forth")]
    public IEnumerable<WeatherForecast> Get() => WeatherForecast.Get();
}
```

This will only be taken into account if the end point requires authorization.