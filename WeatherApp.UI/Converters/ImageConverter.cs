using System;
using System.Globalization;
using System.Net.Http;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;

namespace WeatherApp.UI.Converters
{
    public class ImageConverter : IValueConverter
    {
        public static ImageConverter Instance { get; } = new ImageConverter();

        private static readonly HttpClient _httpClient = new HttpClient();

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string url && !string.IsNullOrWhiteSpace(url))
            {
                try
                {
                    // For a real production app, you might want to cache these bitmaps
                    var response = _httpClient.GetAsync(url).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        using var stream = response.Content.ReadAsStreamAsync().Result;
                        return new Bitmap(stream);
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
