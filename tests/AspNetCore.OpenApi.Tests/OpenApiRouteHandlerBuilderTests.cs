namespace Microsoft.AspNetCore.OpenApi;

using Microsoft.AspNetCore.Http;

public class OpenApiRouteHandlerBuilderTests
{
    [Test]
    public async Task AddScopes()
    {
        var builder = new Builder.RouteHandlerBuilder([]);
        await Assert.That(builder.WithScopes("first", "second")).IsNotNull();
    }

    [Test]
    public async Task AddScopesFromInterface()
    {
        Builder.IEndpointConventionBuilder builder = new Builder.RouteHandlerBuilder([]);
        await Assert.That(builder.WithScopes("first", "second")).IsNotNull();
    }
}