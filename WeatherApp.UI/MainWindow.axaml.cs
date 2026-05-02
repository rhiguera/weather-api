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
            Console.WriteLine("MainWindow: parameterless constructor called");
            this.Opened += (_, __) => Console.WriteLine($"MainWindow Opened. ContentType={this.Content?.GetType().FullName}");
        }

        // Constructor used by DI to inject the ViewModel
        public MainWindow(ViewModels.MainWindowViewModel viewModel) : this()
        {
            DataContext = viewModel;
            Console.WriteLine($"MainWindow: DI constructor called. DataContext={(DataContext!=null?DataContext.GetType().FullName:"null")}");
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
