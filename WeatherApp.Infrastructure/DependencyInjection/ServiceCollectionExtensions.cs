using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Infrastructure.Services;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Net.Http;

namespace WeatherApp.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // If OPENWEATHER_API_KEY is present, register the real OpenWeatherMap client, otherwise use the stub.
            var apiKey = System.Environment.GetEnvironmentVariable("OPENWEATHER_API_KEY");
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                // Configure HttpClient base address and a conservative timeout; retry is applied inside the typed client using Polly.
                services.AddHttpClient<OpenWeatherMapService>(client =>
                {
                    client.BaseAddress = new Uri("https://api.openweathermap.org");
                    client.Timeout = TimeSpan.FromSeconds(15);
                })
                .ConfigureHttpClient(client =>
                {
                    client.BaseAddress = new Uri("https://api.openweathermap.org");
                    client.Timeout = TimeSpan.FromSeconds(15);
                });

                // Register OpenWeatherMapService as IWeatherService with apiKey
                services.AddTransient<IWeatherService>(serviceProvider =>
                {
                    var httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
                    var httpClient = httpClientFactory.CreateClient(nameof(OpenWeatherMapService));
                    return new OpenWeatherMapService(httpClient, apiKey);
                });
            }
            else
            {
                services.AddSingleton<IWeatherService, WeatherServiceStub>();
            }

            return services;
        }
    }
}
