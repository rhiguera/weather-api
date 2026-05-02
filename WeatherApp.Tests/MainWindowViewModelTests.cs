using System.Threading.Tasks;
using WeatherApp.Application.UseCases.GetWeather;
using WeatherApp.Domain.Entities;
using WeatherApp.UI.ViewModels;
using Xunit;

namespace WeatherApp.Tests
{
    public class MainWindowViewModelTests
    {
        private class FakeUseCase : IGetWeatherUseCase
        {
            private readonly GetWeatherResponse _response;

            public FakeUseCase(GetWeatherResponse response)
            {
                _response = response;
            }

            public Task<GetWeatherResponse> HandleAsync(GetWeatherRequest request, System.Threading.CancellationToken cancellationToken = default)
            {
                return Task.FromResult(_response);
            }
        }

        [Fact]
        public async Task GetWeatherCommand_SetsProperties_OnSuccess()
        {
            var weather = new Weather("TestCity", 10.0m, "clear");
            var response = new GetWeatherResponse(true, weather);
            var useCase = new FakeUseCase(response);
            var vm = new MainWindowViewModel(useCase);

            await vm.GetWeatherCommand.ExecuteAsync(null);

            Assert.Equal("10.0 °C in TestCity", vm.TemperatureDisplay);
            Assert.Equal("clear", vm.Description);
            Assert.True(string.IsNullOrEmpty(vm.ErrorMessage));
        }

        [Fact]
        public async Task GetWeatherCommand_SetsError_OnFailure()
        {
            var response = new GetWeatherResponse(false, null, "network error");
            var useCase = new FakeUseCase(response);
            var vm = new MainWindowViewModel(useCase);

            await vm.GetWeatherCommand.ExecuteAsync(null);

            Assert.Equal("network error", vm.ErrorMessage);
        }
    }
}
