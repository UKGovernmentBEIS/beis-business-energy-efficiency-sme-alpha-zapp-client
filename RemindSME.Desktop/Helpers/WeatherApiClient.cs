using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using Flurl;
using Flurl.Http;
using RemindSME.Desktop.Models;

namespace RemindSME.Desktop.Helpers
{
    public interface IWeatherApiClient {
        Task<CurrentWeather> GetCurrentWeatherForLocation(string location);
        Task<WeatherForecast> GetWeatherForecastForLocation(string location);
    }

    public class WeatherApiClient : IWeatherApiClient
    {
        private const string BaseUrl = "http://api.openweathermap.org/data/2.5";
        private const string ApiKey = "api-key";

        private readonly ILog log;

        public WeatherApiClient(ILog log)
        {
            this.log = log;
        }

        public async Task<CurrentWeather> GetCurrentWeatherForLocation(string location)
        {
            return await MakeApiRequest<CurrentWeather>("weather", location);
        }

        public async Task<WeatherForecast> GetWeatherForecastForLocation(string location)
        {
            return await MakeApiRequest<WeatherForecast>("forecast", location);
        }

        private async Task<T> MakeApiRequest<T>(string endpoint, string location)
        {
            var url = BaseUrl
                .AppendPathSegment(endpoint)
                .SetQueryParam("appid", ApiKey)
                .SetQueryParam("units", "metric")
                .SetQueryParam("q", location);

            try
            {
                return await url.GetJsonAsync<T>();
            }
            catch (Exception e)
            {
                log.Error(e);
                return default(T);
            }
        }
    }
}
