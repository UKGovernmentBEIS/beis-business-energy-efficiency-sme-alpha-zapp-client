using System;
using System.Linq;
using System.Threading.Tasks;
using RemindSME.Desktop.Configuration;
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
        private readonly ISettings settings;

        public HeatingRecommendationHelper(IWeatherApiClient weatherApiClient, ISettings settings)
        {
            this.weatherApiClient = weatherApiClient;
            this.settings = settings;
        }

        public async Task<string> GetWeatherDependentMessage()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation("London,UK");
            var peakTemperature = GetPeakTemperatureForToday(forecast);

            if (!peakTemperature.HasValue)
            {
                return Resources.Notification_HeatingFirstLogin_Message;
            }

            var message = GetRecommendationMessageForTemperature(peakTemperature.Value);
            settings.MostRecentPeakTemperature = peakTemperature.Value;
            return message;
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < DateTime.Today.AddDays(1))
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private string GetRecommendationMessageForTemperature(double temperature)
        {
            if (ShouldShowAirConditioningMessage(temperature))
            {
                return $"Looks like it's going to be hot today ({temperature:F0}°C)! Please make sure the air conditioning is set to a sensible temperature for today's weather. Can you open windows instead?";
            }

            if (ShouldShowHeatingMessage(temperature))
            {
                return $"Looks like it's going to be cold today ({temperature:F0}°C)! Please make sure the heating is set to a sensible temperature for today's weather.";
            }

            return Resources.Notification_HeatingFirstLogin_Message;
        }

        private bool ShouldShowAirConditioningMessage(double temperature)
        {
            return TemperatureRequiresAirConditioning(temperature) && TemperatureIsSignificantlyHotterThanYesterday(temperature);
        }

        private bool ShouldShowHeatingMessage(double temperature)
        {
            return TemperatureRequiresHeating(temperature) && TemperatureIsSignificantlyColderThanYesterday(temperature);
        }

        private bool TemperatureRequiresHeating(double temperature) => temperature < 15;
        private bool TemperatureRequiresAirConditioning(double temperature) => temperature > 25;

        private bool TemperatureIsSignificantlyHotterThanYesterday(double temperature) => TemperatureDifference(temperature) > +5;
        private bool TemperatureIsSignificantlyColderThanYesterday(double temperature) => TemperatureDifference(temperature) < -5;

        private double TemperatureDifference(double temperature) => temperature - settings.MostRecentPeakTemperature;
    }
}
