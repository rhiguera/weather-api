using System.Threading;
using System.Threading.Tasks;

namespace WeatherApp.Application.UseCases.GetWeather
{
    public interface IGetWeatherUseCase
    {
        Task<GetWeatherResponse> HandleAsync(GetWeatherRequest request, CancellationToken cancellationToken = default);
    }
}
