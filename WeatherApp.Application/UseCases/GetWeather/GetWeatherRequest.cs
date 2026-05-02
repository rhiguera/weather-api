namespace WeatherApp.Application.UseCases.GetWeather
{
    public sealed class GetWeatherRequest
    {
        public GetWeatherRequest(string city)
        {
            City = city;
        }

        public string City { get; }
    }
}
