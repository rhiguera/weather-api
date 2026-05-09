using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using RichardSzalay.MockHttp;
using WeatherApp.Infrastructure.Services;
using Xunit;

namespace WeatherApp.Tests
{
    public class OpenWeatherMapServiceTests
    {
        private const string BaseUrl = "https://api.openweathermap.org";
        private const string ApiKey = "test-api-key";

        [Fact]
        public async Task GetWeatherByCityAsync_ReturnsWeather_OnSuccess()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/data/2.5/weather*")
                .Respond("application/json", "{ \"name\": \"London\", \"main\": { \"temp\": 15.5, \"humidity\": 80 }, \"weather\": [ { \"description\": \"cloudy\", \"icon\": \"03d\" } ], \"wind\": { \"speed\": 5.5 } }");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            var service = new OpenWeatherMapService(client, ApiKey);

            // Act
            var result = await service.GetWeatherByCityAsync("London");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("London", result!.City);
            Assert.Equal(15.5m, result.TemperatureCelsius);
            Assert.Equal("cloudy", result.Description);
            Assert.Equal(80, result.Humidity);
            Assert.Equal(5.5m, result.WindSpeed);
            Assert.Equal("03d", result.IconCode);
        }

        [Fact]
        public async Task GetWeatherByCityAsync_ReturnsNull_WhenCityIsEmpty()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            var service = new OpenWeatherMapService(mockHttp.ToHttpClient(), ApiKey);

            // Act
            var result = await service.GetWeatherByCityAsync("");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetWeatherByCityAsync_Throws_OnApiError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"{BaseUrl}/data/2.5/weather*")
                .Respond(HttpStatusCode.InternalServerError);

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            var service = new OpenWeatherMapService(client, ApiKey);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeatherByCityAsync("London"));
        }

        [Fact]
        public async Task GetWeatherByCityAsync_Retries_OnTransientError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            
            // Fail twice, then succeed
            mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond("application/json", "{ \"name\": \"London\", \"main\": { \"temp\": 15.5, \"humidity\": 80 }, \"weather\": [ { \"description\": \"cloudy\", \"icon\": \"03d\" } ], \"wind\": { \"speed\": 5.5 } }");

            var client = mockHttp.ToHttpClient();
            client.BaseAddress = new Uri(BaseUrl);
            var service = new OpenWeatherMapService(client, ApiKey);

            // Act
            var result = await service.GetWeatherByCityAsync("London");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("London", result!.City);
            mockHttp.VerifyNoOutstandingExpectation();
        }
    }
}
