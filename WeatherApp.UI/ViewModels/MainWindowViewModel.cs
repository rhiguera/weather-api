using CommunityToolkit.Mvvm.ComponentModel;

namespace WeatherApp.UI.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        public MainWindowViewModel()
        {
            Title = "WeatherApp";
        }

        [ObservableProperty]
        private string title;
    }
}
