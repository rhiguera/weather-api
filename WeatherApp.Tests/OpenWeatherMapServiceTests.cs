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
                .Respond("application/json", "{ \"name\": \"London\", \"main\": { \"temp\": 15.5 }, \"weather\": [ { \"description\": \"cloudy\" } ] }");

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
            // OpenWeatherMapService uses GetFromJsonAsync which throws on non-success by default? 
            // Actually HttpClient.GetFromJsonAsync throws if status code is not success.
            // And Polly policy handles HttpRequestException.
            // After 3 retries, it should finally throw.
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetWeatherByCityAsync("London"));
        }

        [Fact]
        public async Task GetWeatherByCityAsync_Retries_OnTransientError()
        {
            // Arrange
            var mockHttp = new MockHttpMessageHandler();
            
            // Fail twice, then succeed
            var request = mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond(HttpStatusCode.InternalServerError);
            mockHttp.Expect($"{BaseUrl}/data/2.5/weather*")
                .Respond("application/json", "{ \"name\": \"London\", \"main\": { \"temp\": 15.5 }, \"weather\": [ { \"description\": \"cloudy\" } ] }");

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
