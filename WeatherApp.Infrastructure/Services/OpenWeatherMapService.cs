using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using WeatherApp.Domain.Entities;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Infrastructure.Dto;
using Polly;

namespace WeatherApp.Infrastructure.Services
{
    public class OpenWeatherMapService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly Polly.Retry.AsyncRetryPolicy _retryPolicy;

        public OpenWeatherMapService(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            _retryPolicy = Polly.Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }

        public async Task<Weather?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(city)) return null;

            var url = $"/data/2.5/weather?q={Uri.EscapeDataString(city)}&units=metric&appid={_apiKey}";

            try
            {
                var dto = await _retryPolicy.ExecuteAsync(() => _httpClient.GetFromJsonAsync<OpenWeatherMapWeatherDto>(url, cancellationToken)).ConfigureAwait(false);
                if (dto is null) return null;

                var desc = dto.Weather.Length > 0 ? dto.Weather[0].Description : string.Empty;
                return new Weather(dto.Name, dto.Main.Temp, desc);
            }
            catch (OperationCanceledException)
            {
                // includes timeout
                throw;
            }
            catch (Exception)
            {
                // bubble up to be handled by use case
                throw;
            }
        }
    }
}
