using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using RemindSME.Desktop.Models;

namespace RemindSME.Desktop.Helpers
{
    public interface IWeatherDataService {
        Task<CurrentWeather> GetCurrentWeatherForLocation(string location);
        Task<WeatherForecast> GetWeatherForecastForLocation(string location);
    }

    public class WeatherDataService : IWeatherDataService
    {
        private const string BaseUrl = "http://api.openweathermap.org/data/2.5";
        private const string ApiKey = "api-key";

        public async Task<CurrentWeather> GetCurrentWeatherForLocation(string location)
        {
            var url = BaseUrl
                .AppendPathSegment("weather")
                .SetQueryParam("appid", ApiKey)
                .SetQueryParam("units", "metric")
                .SetQueryParam("q", location);

            try
            {
                return await url.GetJsonAsync<CurrentWeather>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to fetch current weather data: {e.Message}");
                return null;
            }
        }

        public async Task<WeatherForecast> GetWeatherForecastForLocation(string location)
        {
            var url = BaseUrl
                .AppendPathSegment("forecast")
                .SetQueryParam("appid", ApiKey)
                .SetQueryParam("units", "metric")
                .SetQueryParam("q", location);

            try
            {
                return await url.GetJsonAsync<WeatherForecast>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to fetch weather forecast data: {e.Message}");
                return null;
            }
        }
    }
}
