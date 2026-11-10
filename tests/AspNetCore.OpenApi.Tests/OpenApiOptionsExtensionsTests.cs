namespace Microsoft.AspNetCore.OpenApi;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

public class OpenApiOptionsExtensionsTests
{
    private const string DocumentName = "v1";

    [Test]
    public async Task DefaultTitle()
    {
        await TestTitle(typeof(OpenApiOptionsExtensionsTests).Assembly.GetName().Name, _ => { });
    }

    [Test]
    public async Task SetTitle()
    {
        const string Name = nameof(Name);
        await TestTitle(Name, options => options.SetInfo(Name));
    }

    [Test]
    public async Task SetTitleAndVersion()
    {
        const string Name = nameof(Name);
        await TestTitle(Name, options => options.SetInfo(Name, DocumentName));
    }

    [Test]
    public async Task AddOpenIdConnect()
    {
        Microsoft.OpenApi.OpenApiDocument document = await GetOpenApiDocument(options => _ = options.AddOpenIdConnect("http://localhost"));
        _ = await Assert.That(document.Components).IsNotNull()
            .And.Member(x => x.SecuritySchemes, x => x.IsNotNull());
    }

    [Test]
    public async Task AddSchema()
    {
        string name = default;
        Microsoft.OpenApi.OpenApiDocument document = await GetOpenApiDocument(options =>
        {
            var schema = new Microsoft.OpenApi.OpenApiSecurityScheme { Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey };
            _ = options.AddSecurityScheme(schema, out name);
        });

        _ = await Assert.That(document.Components).IsNotNull()
            .And.Member(x => x.SecuritySchemes, x => x.IsNotNull());
        _ = await Assert.That(name).IsNotNull();
    }

    [Test]
    public async Task AddSchemaWithName()
    {
        Microsoft.OpenApi.OpenApiDocument document = await GetOpenApiDocument(options =>
        {
            var schema = new Microsoft.OpenApi.OpenApiSecurityScheme { Type = Microsoft.OpenApi.SecuritySchemeType.ApiKey };
            _ = options.AddSecurityScheme("SchemaName", schema);
        });

        _ = await Assert.That(document.Components).IsNotNull()
            .And.Member(x => x.SecuritySchemes, x => x.IsNotNull());
    }

    private static async Task TestTitle(string name, Action<OpenApiOptions> configure)
    {
        Microsoft.OpenApi.OpenApiDocument document = await GetOpenApiDocument(configure);

        _ = await Assert.That(document.Info.Title).IsEqualTo($"{name} | {DocumentName}");
    }

    private static async Task<Microsoft.OpenApi.OpenApiDocument> GetOpenApiDocument(Action<OpenApiOptions> configure)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();
        _ = builder.Services.AddOpenApi(DocumentName, configure);

        WebApplication application = builder.Build();

        return await GetOpenApiDocumentCore(application.Services, DocumentName);

        static async Task<Microsoft.OpenApi.OpenApiDocument> GetOpenApiDocumentCore(IServiceProvider serviceProvider, string documentName)
        {
            // get the keyed service
            Type documentServiceType = typeof(OpenApiOptions).Assembly.GetType("Microsoft.AspNetCore.OpenApi.OpenApiDocumentService") ?? throw new InvalidOperationException();
            System.Reflection.MethodInfo method = documentServiceType.GetMethod("GetOpenApiDocumentAsync", [typeof(CancellationToken)]);
            Task<Microsoft.OpenApi.OpenApiDocument> task = (Task<Microsoft.OpenApi.OpenApiDocument>)method.Invoke(serviceProvider.GetKeyedService(documentServiceType, documentName), [CancellationToken.None]);
            return await task;
        }
    }
}