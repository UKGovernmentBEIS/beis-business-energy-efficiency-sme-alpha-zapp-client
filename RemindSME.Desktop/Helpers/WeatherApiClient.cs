using System;
using System.Configuration;
using System.Threading.Tasks;
using Caliburn.Micro;
using Flurl;
using Flurl.Http;
using RemindSME.Desktop.Models;

namespace RemindSME.Desktop.Helpers
{
    public interface IWeatherApiClient
    {
        Task<WeatherForecast> GetWeatherForecastForLocation(string location);
    }

    public class WeatherApiClient : IWeatherApiClient
    {
        private static readonly string ServerUrl = ConfigurationManager.AppSettings["ServerUrl"];

        private readonly ILog log;

        public WeatherApiClient(ILog log)
        {
            this.log = log;
        }

        public async Task<WeatherForecast> GetWeatherForecastForLocation(string location)
        {
            var url = ServerUrl
                .AppendPathSegment("weather/forecast")
                .SetQueryParam("location", location);

            try
            {
                return await url.GetJsonAsync<WeatherForecast>();
            }
            catch (Exception e)
            {
                log.Error(e);
                return null;
            }
        }
    }
}
