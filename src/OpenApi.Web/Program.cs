// -----------------------------------------------------------------------
// <copyright file="Program.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Microsoft.AspNetCore.OpenApi;

const string PathBase = "/api";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    _ = options.SetInfo("API");
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
    _ = options.WithAuthorizeCheck(scheme);
    _ = options.UsePathBase(PathBase);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    _ = app.MapOpenApi();
}

app.UsePathBase(PathBase);

app
    .MapGet($"minimal/{nameof(Altemiq.OpenApi.Web.WeatherForecast)}", Altemiq.OpenApi.Web.WeatherForecast.Get)
    .RequireAuthorization()
    .WithScopes("first", "second");

app.UseSwaggerUI(options => options.SwaggerEndpoint("../openapi/v1.json", "Example"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync().ConfigureAwait(false);