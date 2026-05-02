using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Infrastructure.Services
{
    /// <summary>
    /// Stub implementation of <see cref="IWeatherService"/> for development and testing.
    /// Returns deterministic sample data for a few cities, and null for unknown.
    /// </summary>
    public class WeatherServiceStub : IWeatherService
    {
        public Task<Weather?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city))
                return Task.FromResult<Weather?>(null);

            var normalized = city.Trim().ToLowerInvariant();

            return normalized switch
            {
                "london" => Task.FromResult<Weather?>(new Weather("London", 12.3m, "broken clouds")),
                "madrid" => Task.FromResult<Weather?>(new Weather("Madrid", 20.1m, "clear sky")),
                "sydney" => Task.FromResult<Weather?>(new Weather("Sydney", 22.5m, "light rain")),
                _ => Task.FromResult<Weather?>(null)
            };
        }
    }
}
