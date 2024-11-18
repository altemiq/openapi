// -----------------------------------------------------------------------
// <copyright file="WeatherForecastController.cs" company="Altemiq">
// Copyright (c) Altemiq. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace Altemiq.OpenApi.Web.Controllers;

using Microsoft.AspNetCore.Mvc;

/// <summary>
/// The <see cref="WeatherForecast"/> controller.
/// </summary>
[ApiController]
[Route("controllers/[controller]")]
public class WeatherForecastController : ControllerBase
{
    /// <summary>
    /// Gets the weather forecasts.
    /// </summary>
    /// <returns>The weather forecasts.</returns>
    [HttpGet(Name = "GetWeatherForecast")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    [Scopes("third", "forth")]
    public IEnumerable<WeatherForecast> Get() => WeatherForecast.Get();
}