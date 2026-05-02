using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WeatherApp.UI.ViewModels;

namespace WeatherApp.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(ViewModels.MainWindowViewModel viewModel)
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = viewModel;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
