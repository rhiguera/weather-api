namespace WeatherApp.Domain.Entities
{
    public sealed class Location
    {
        public Location(string city, string country)
        {
            City = city;
            Country = country;
        }

        public string City { get; }
        public string Country { get; }
    }
}
