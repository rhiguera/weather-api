using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Domain.Interfaces;

namespace WeatherApp.Application.UseCases.GetWeather
{
    public sealed class GetWeatherUseCase : IGetWeatherUseCase
    {
        private readonly IWeatherService _weatherService;

        public GetWeatherUseCase(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        public async Task<GetWeatherResponse> HandleAsync(GetWeatherRequest request, CancellationToken cancellationToken = default)
        {
            var weather = await _weatherService.GetWeatherByCityAsync(request.City, cancellationToken).ConfigureAwait(false);
            if (weather is null)
            {
                return new GetWeatherResponse(false, null, "City not found");
            }

            return new GetWeatherResponse(true, weather);
        }
    }
}
