using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Application.UseCases.GetWeather;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using Xunit;

namespace WeatherApp.Tests
{
    public class GetWeatherUseCaseTests
    {
        private class FakeWeatherService : IWeatherService
        {
            private readonly Weather? _result;

            public FakeWeatherService(Weather? result)
            {
                _result = result;
            }

            public Task<Weather?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_result);
            }
        }

        [Fact]
        public async Task HandleAsync_ReturnsSuccess_WhenWeatherFound()
        {
            var sample = new Weather("TestCity", 15.5m, "sunny");
            var service = new FakeWeatherService(sample);
            var useCase = new GetWeatherUseCase(service);

            var response = await useCase.HandleAsync(new GetWeatherRequest("TestCity"));

            Assert.True(response.Success);
            Assert.NotNull(response.Weather);
            Assert.Equal("TestCity", response.Weather!.City);
        }

        [Fact]
        public async Task HandleAsync_ReturnsFailure_WhenNotFound()
        {
            var service = new FakeWeatherService(null);
            var useCase = new GetWeatherUseCase(service);

            var response = await useCase.HandleAsync(new GetWeatherRequest("UnknownCity"));

            Assert.False(response.Success);
            Assert.Null(response.Weather);
            Assert.Equal("City not found", response.ErrorMessage);
        }
    }
}
