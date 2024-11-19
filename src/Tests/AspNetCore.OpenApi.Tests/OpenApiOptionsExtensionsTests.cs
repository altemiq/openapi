namespace Microsoft.AspNetCore.OpenApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Xml.Linq;

public class OpenApiOptionsExtensionsTests
{
    const string DocumentName = "v1";

    [Test]
    public async Task DefaultTitle() => await TestTitle(typeof(OpenApiOptionsExtensionsTests).Assembly.GetName().Name, options => { });

    [Test]
    public async Task SetTitle()
    {
        const string Name = nameof(Name);
        await TestTitle(Name, options => options.SetInfo(Name));
    }

    [Test]
    public async Task AddOpenIdConnect()
    {
        var document = await GetOpenApiDocument(options => options.AddOpenIdConnect("http://localhost"));
        await Assert.That(document.Components.SecuritySchemes).IsNotEmpty();
    }

    private static async Task TestTitle(string? name, Action<OpenApiOptions> configure)
    {
        var document = await GetOpenApiDocument(configure);

        await Assert.That(document.Info.Title).IsEqualTo($"{name} | {DocumentName}");
    }

    private static async Task<Microsoft.OpenApi.Models.OpenApiDocument> GetOpenApiDocument(Action<OpenApiOptions> configure)
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddOpenApi(DocumentName, configure);

        var application = builder.Build();

        return await GetOpenApiDocument(application.Services, DocumentName);

        static async Task<Microsoft.OpenApi.Models.OpenApiDocument> GetOpenApiDocument(IServiceProvider serviceProvider, string documentName)
        {
            // get the keyed service
            var documentServiceType = typeof(OpenApiOptions).Assembly.GetType("Microsoft.AspNetCore.OpenApi.OpenApiDocumentService") ?? throw new InvalidOperationException();
            var service = serviceProvider.GetKeyedServices(documentServiceType, documentName).First();

            var method = documentServiceType.GetMethod("GetOpenApiDocumentAsync")!;
            var task = (Task<Microsoft.OpenApi.Models.OpenApiDocument>)method.Invoke(service, [serviceProvider, CancellationToken.None])!;

            return await task;
        }
    }
}