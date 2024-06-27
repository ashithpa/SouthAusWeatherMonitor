using System.Net;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using API.Services;
using Model;
using Moq.Protected;
using System.Text.Json;

namespace API.Tests
{
    public class WeatherServiceTests
    {
        [Fact]
        public async Task GetWeatherObservationsAsync_ReturnsData_WhenApiCallSucceeds()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockOptions = Options.Create(new WeatherApiSettings
            {
                BaseUrl = "https://api.weather.com/data",
                TimeoutInSeconds = 10
            });

            var expectedData = new List<ObservationData>
            {
                new ObservationData { Name = "Station 1", apparent_t = 25.5 },
                new ObservationData { Name = "Station 2", apparent_t = 28.1 }
            };

            var serializedData = JsonSerializer.Serialize(new WeatherData
            {
                Observations = new Observations { Data = expectedData }
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(serializedData, Encoding.UTF8, "application/json")
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://api.weather.com")
            };

            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                                 .Returns(httpClient);

            var weatherService = new WeatherService(httpClient, mockOptions);

            // Act
            var result = await weatherService.GetWeatherObservationsAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedData.Count, result.Count);
            Assert.Equal(expectedData[0].Name, result[0].Name);
            Assert.Equal(expectedData[1].apparent_t, result[1].apparent_t);
        }

        [Fact]
        public async Task GetWeatherObservationsAsync_ReturnsEmptyList_WhenNoData()
        {
            // Arrange
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            var mockOptions = Options.Create(new WeatherApiSettings
            {
                BaseUrl = "https://api.weather.com/data",
                TimeoutInSeconds = 10
            });

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{}", Encoding.UTF8, "application/json")
            };

            var handler = new Mock<HttpMessageHandler>();
            handler.Protected()
                   .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                   .ReturnsAsync(response);

            var httpClient = new HttpClient(handler.Object)
            {
                BaseAddress = new Uri("https://api.weather.com")
            };

            mockHttpClientFactory.Setup(factory => factory.CreateClient(It.IsAny<string>()))
                                 .Returns(httpClient);

            var weatherService = new WeatherService(httpClient, mockOptions);

            // Act
            var result = await weatherService.GetWeatherObservationsAsync("123");

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }
    }
}
