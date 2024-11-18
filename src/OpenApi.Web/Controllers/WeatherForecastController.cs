using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OpenApi.Web.Controllers;

[ApiController]
[Route("controllers/[controller]")]
public class WeatherForecastController : ControllerBase
{
    private readonly ILogger<WeatherForecastController> _logger;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    [Authorize]
    [Scopes("third", "forth")]
    public IEnumerable<WeatherForecast> Get() => WeatherForecast.Get();
}
