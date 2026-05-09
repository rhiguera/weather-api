using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WeatherApp.UI.ViewModels;

namespace WeatherApp.UI
{
    public partial class MainWindow : Window
    {
        // Parameterless constructor required by Avalonia XAML loader at runtime
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            this.Opened += (s, e) => 
            {
                var textBox = this.FindControl<TextBox>("CityTextBox");
                textBox?.Focus();
            };
        }

        // Constructor used by DI to inject the ViewModel
        public MainWindow(ViewModels.MainWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
            
            // Handle Theme Switching
            viewModel.PropertyChanged += (s, e) => 
            {
                if (e.PropertyName == nameof(viewModel.IsDarkMode))
                {
                    var app = Avalonia.Application.Current;
                    if (app != null)
                    {
                        app.RequestedThemeVariant = viewModel.IsDarkMode 
                            ? Avalonia.Styling.ThemeVariant.Dark 
                            : Avalonia.Styling.ThemeVariant.Light;
                    }
                }
            };
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
