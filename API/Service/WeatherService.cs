using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Model;

namespace API.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherApiSettings _apiSettings;

        public WeatherService(HttpClient httpClient, IOptions<WeatherApiSettings> apiSettings)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiSettings = apiSettings?.Value ?? throw new ArgumentNullException(nameof(apiSettings));

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.Timeout = TimeSpan.FromSeconds(_apiSettings.TimeoutInSeconds);
        }

        public async Task<List<ObservationData>> GetWeatherObservationsAsync(string stationId)
        {
            var observations = new List<ObservationData>();

            try
            {
                var apiUrl = $"{_apiSettings.BaseUrl}.{stationId}.json";
                var response = await _httpClient.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true // Ignore case when matching properties
                    };

                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    var weatherData = await JsonSerializer.DeserializeAsync<WeatherData>(contentStream, options);

                    if (weatherData?.Observations?.Data != null)
                    {
                        observations.AddRange(weatherData.Observations.Data);
                    }
                    else
                    {
                        Console.WriteLine("No valid data found in the response.");
                    }
                }
                else
                {
                    Console.WriteLine($"Weather API request failed with status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request error occurred: {ex.Message}");
                // Optionally throw custom exception or log further details
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"JSON deserialization error occurred: {ex.Message}");
                // Optionally throw custom exception or log further details
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                // Optionally throw or log further details
                throw; // Consider throwing if not handled or logged appropriately
            }

            return observations;
        }
    }

    public class WeatherApiSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutInSeconds { get; set; } = 30; // Default timeout of 30 seconds
    }

}
