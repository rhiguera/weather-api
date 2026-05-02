using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using WeatherApp.UI.ViewModels;

namespace WeatherApp.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            DataContext = new MainWindowViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
