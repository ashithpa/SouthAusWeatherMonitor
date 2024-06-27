using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/weather")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet("{stationId}")]
        public async Task<IActionResult> GetWeatherDataAsync(string stationId)
        {
            var weatherData = await _weatherService.GetWeatherObservationsAsync(stationId);
            if (weatherData != null && weatherData.Count > 0)
            {
                return Ok(weatherData);
            }
            return NotFound();
        }
    }
}
