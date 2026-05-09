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
            vm.City = "TestCity";

            await vm.GetWeatherCommand.ExecuteAsync(null);

            string expectedTemp = $"{10.0m} °C in TestCity";
            Assert.Equal(expectedTemp, vm.TemperatureDisplay);
            Assert.Equal("clear", vm.Description);
            Assert.True(string.IsNullOrEmpty(vm.ErrorMessage));
        }

        [Fact]
        public async Task GetWeatherCommand_SetsError_OnFailure()
        {
            var response = new GetWeatherResponse(false, null, "network error");
            var useCase = new FakeUseCase(response);
            var vm = new MainWindowViewModel(useCase);
            vm.City = "TestCity";

            await vm.GetWeatherCommand.ExecuteAsync(null);

            Assert.Equal("network error", vm.ErrorMessage);
        }

        [Fact]
        public async Task GetWeatherCommand_SetsError_WhenCityIsEmpty()
        {
            var response = new GetWeatherResponse(true, new Weather("TestCity", 10.0m, "clear"));
            var useCase = new FakeUseCase(response);
            var vm = new MainWindowViewModel(useCase);
            vm.City = "";

            await vm.GetWeatherCommand.ExecuteAsync(null);

            Assert.Equal("Please enter a city.", vm.ErrorMessage);
        }
    }
}
