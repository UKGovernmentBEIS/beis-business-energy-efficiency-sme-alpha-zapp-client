using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using RemindSME.Desktop.Models;

namespace RemindSME.Desktop.Helpers
{
    public interface IWeatherDataService {
        Task<WeatherData> GetWeatherData(string location);
    }

    public class WeatherDataService : IWeatherDataService
    {
        private const string BaseUrl = "http://api.openweathermap.org/data/2.5";
        private const string ApiKey = "api-key";

        public async Task<WeatherData> GetWeatherData(string location)
        {
            var url = BaseUrl
                .AppendPathSegment("weather")
                .SetQueryParam("appid", ApiKey)
                .SetQueryParam("units", "metric")
                .SetQueryParam("q", location);

            try
            {
                return await url.GetJsonAsync<WeatherData>();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to fetch weather data: {e.Message}");
                return null;
            }
        }
    }
}
