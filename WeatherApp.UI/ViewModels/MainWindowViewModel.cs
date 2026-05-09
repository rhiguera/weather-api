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
            ToggleThemeCommand = new RelayCommand(ExecuteToggleTheme);
        }

        [ObservableProperty]
        private string city = string.Empty;

        [ObservableProperty]
        private string temperatureDisplay = string.Empty;

        [ObservableProperty]
        private string description = string.Empty;

        [ObservableProperty]
        private string humidityDisplay = string.Empty;

        [ObservableProperty]
        private string windSpeedDisplay = string.Empty;

        [ObservableProperty]
        private string? iconUrl;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isDarkMode = false;

        public IAsyncRelayCommand GetWeatherCommand { get; }
        public IRelayCommand ToggleThemeCommand { get; }

        private void ExecuteToggleTheme()
        {
            IsDarkMode = !IsDarkMode;
            // The actual theme switching logic will be handled by the View (code-behind or binding)
        }

        private async Task ExecuteGetWeatherAsync(CancellationToken ct = default)
        {
            ErrorMessage = string.Empty;
            TemperatureDisplay = string.Empty;
            Description = string.Empty;
            HumidityDisplay = string.Empty;
            WindSpeedDisplay = string.Empty;
            IconUrl = null;

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
            HumidityDisplay = $"Humidity: {w.Humidity}%";
            WindSpeedDisplay = $"Wind: {w.WindSpeed} m/s";
            IconUrl = w.IconUrl;
        }
    }
}
