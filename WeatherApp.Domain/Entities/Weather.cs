using System;

namespace WeatherApp.Domain.Entities
{
    public sealed class Weather
    {
        public Weather(string city, decimal temperatureCelsius, string description)
        {
            City = city ?? throw new ArgumentNullException(nameof(city));
            TemperatureCelsius = temperatureCelsius;
            Description = description ?? string.Empty;
        }

        public string City { get; }
        public decimal TemperatureCelsius { get; }
        public string Description { get; }
    }
}
