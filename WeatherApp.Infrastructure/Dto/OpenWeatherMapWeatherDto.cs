using System.Text.Json.Serialization;

namespace WeatherApp.Infrastructure.Dto
{
    public class OpenWeatherMapWeatherDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("main")]
        public MainDto Main { get; set; } = new MainDto();

        [JsonPropertyName("weather")]
        public WeatherEntryDto[] Weather { get; set; } = System.Array.Empty<WeatherEntryDto>();
    }

    public class MainDto
    {
        [JsonPropertyName("temp")]
        public decimal Temp { get; set; }
    }

    public class WeatherEntryDto
    {
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}
