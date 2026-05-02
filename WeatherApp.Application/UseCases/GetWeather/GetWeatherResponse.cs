using WeatherApp.Domain.Entities;

namespace WeatherApp.Application.UseCases.GetWeather
{
    public sealed class GetWeatherResponse
    {
        public GetWeatherResponse(bool success, Weather? weather, string? errorMessage = null)
        {
            Success = success;
            Weather = weather;
            ErrorMessage = errorMessage;
        }

        public bool Success { get; }
        public Weather? Weather { get; }
        public string? ErrorMessage { get; }
    }
}
