using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Domain.Entities;

namespace WeatherApp.Domain.Interfaces
{
    public interface IWeatherService
    {
        /// <summary>
        /// Obtiene el clima para la ciudad indicada. Retorna null si no se encuentra.
        /// </summary>
        Task<Weather?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default);
    }
}
