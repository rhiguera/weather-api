using System;

namespace WeatherApp.Domain.Entities
{
    public sealed class Weather
    {
        public Weather(string city, decimal temperatureCelsius, string description, int humidity, decimal windSpeed, string iconCode)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            TemperatureCelsius = temperatureCelsius;
            Description = description ?? string.Empty;
            Humidity = humidity;
            WindSpeed = windSpeed;
            IconCode = iconCode ?? "01d";
        }

        public string City { get; }
        public decimal TemperatureCelsius { get; }
        public string Description { get; }
        public int Humidity { get; }
        public decimal WindSpeed { get; }
        public string IconCode { get; }

        public string IconUrl => $"https://openweathermap.org/img/wn/{IconCode}@2x.png";
    }
}
