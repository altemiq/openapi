using Microsoft.AspNetCore.OpenApi;

const string PathBase = "/api"; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.SetInfo("API");
    var scheme = options.AddOAuth2(
        new Microsoft.OpenApi.Models.OpenApiOAuthFlows
        {
            Password = new Microsoft.OpenApi.Models.OpenApiOAuthFlow
            {
                Scopes =
                {
                    ["first"] = "First Scope",
                    ["second"]= "Second Scope",
                    ["third"]= "Third Scope",
                    ["forth"]= "Forth Scope"
                }
            }
        });
    options.WithAuthorizeCheck(scheme);
    options.UsePathBase(PathBase);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UsePathBase(PathBase);

app
    .MapGet($"minimal/{nameof(OpenApi.Web.WeatherForecast)}", OpenApi.Web.WeatherForecast.Get)
    .RequireAuthorization()
    .WithScopes("first", "second");

app.UseSwaggerUI(options => options.SwaggerEndpoint("../openapi/v1.json", "Example"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
