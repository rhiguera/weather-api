using System.Threading.Tasks;
using System.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WeatherApp.Application.UseCases.GetWeather;

namespace WeatherApp.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IGetWeatherUseCase _getWeatherUseCase;

        public MainWindowViewModel(IGetWeatherUseCase getWeatherUseCase)
        {
            _getWeatherUseCase = getWeatherUseCase;
            GetWeatherCommand = new AsyncRelayCommand(ExecuteGetWeatherAsync);
        }

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private string temperatureDisplay = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        public IAsyncRelayCommand GetWeatherCommand { get; }

        private async Task ExecuteGetWeatherAsync(CancellationToken ct = default)
        {
            ErrorMessage = string.Empty;
            TemperatureDisplay = string.Empty;
            Description = string.Empty;

            if (string.IsNullOrWhiteSpace(City))
            {
                ErrorMessage = "Please enter a city.";
                return;
            }

            var request = new GetWeatherRequest(City.Trim());
            var response = await _getWeatherUseCase.HandleAsync(request, ct).ConfigureAwait(false);

            if (!response.Success)
            {
                ErrorMessage = response.ErrorMessage ?? "Unknown error";
                return;
            }

            var w = response.Weather!;
            TemperatureDisplay = $"{w.TemperatureCelsius} °C in {w.City}";
            Description = w.Description;
        }
    }
}
