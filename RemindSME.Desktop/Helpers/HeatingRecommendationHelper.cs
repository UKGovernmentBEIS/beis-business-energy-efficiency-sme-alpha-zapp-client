using System;
using System.Linq;
using System.Threading.Tasks;
using RemindSME.Desktop.Models;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.Helpers
{
    public interface IHeatingRecommendationHelper {
        Task<string> GetWeatherDependentMessage();
    }

    public class HeatingRecommendationHelper : IHeatingRecommendationHelper
    {
        private readonly IWeatherApiClient weatherApiClient;

        public HeatingRecommendationHelper(IWeatherApiClient weatherApiClient)
        {
            this.weatherApiClient = weatherApiClient;
        }

        public async Task<string> GetWeatherDependentMessage()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation("London,UK");
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature != null
                ? $"Looks like it's going to be hot today ({peakTemperature:F0}°C)! Please make sure the air conditioning is set to a sensible temperature for today's weather. Can you open windows instead?"
                : Resources.Notification_HeatingFirstLogin_Message;
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < DateTime.Today.AddDays(1))
                .Max(forecast => forecast.Measurements.Temperature);
        }
    }
}
