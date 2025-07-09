# Altemiq.OpenApi

This contains methods to extend `Microsoft.OpenApi` with extra functionality

## OpenApi

### `OpenApiExtensibleDictionary`

Allows for updating of values in the dictionary

````csharp
operation.Responses.AddOrUpdate(
    UnauthorizedKey,
    _ => new() { Description = UnauthorizedValue },
    (_, r) =>
    {
        r.Description = UnauthorizedValue;
        return r;
    });
````