using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Domain.Interfaces;
using WeatherApp.Infrastructure.Services;

namespace WeatherApp.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            // Register a stubbed weather service for now. Will replace with real HTTP client later.
            services.AddSingleton<IWeatherService, WeatherServiceStub>();
            return services;
        }
    }
}
