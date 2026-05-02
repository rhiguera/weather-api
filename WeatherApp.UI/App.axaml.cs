using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using WeatherApp.Infrastructure.DependencyInjection;

namespace WeatherApp.UI
{
    public partial class App : Avalonia.Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // Configure DI
                var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                services.AddInfrastructure();
                // Application use cases
                services.AddTransient<WeatherApp.Application.UseCases.GetWeather.IGetWeatherUseCase, WeatherApp.Application.UseCases.GetWeather.GetWeatherUseCase>();
                // ViewModels and Views
                services.AddTransient<ViewModels.MainWindowViewModel>();
                services.AddTransient<MainWindow>();

                var provider = services.BuildServiceProvider();

                desktop.MainWindow = provider.GetRequiredService<MainWindow>();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
