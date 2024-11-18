// -----------------------------------------------------------------------
// <copyright file="WeatherForecast.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Altemiq.OpenApi.Web;

/// <summary>
/// The weather forecast.
/// </summary>
public class WeatherForecast
{
    private static readonly string[] Summaries = ["Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"];

    /// <summary>
    /// Gets the date.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets the temperature in celcius.
    /// </summary>
    public int TemperatureC { get; init; }

    /// <summary>
    /// Gets the temperature in farenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(this.TemperatureC / 0.5556);

    /// <summary>
    /// Gets the summary.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Gets a list of example forecasts.
    /// </summary>
    /// <returns>The example forecasts.</returns>
    internal static IEnumerable<WeatherForecast> Get() => Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = Summaries[Random.Shared.Next(Summaries.Length)],
    }).ToArray();
}