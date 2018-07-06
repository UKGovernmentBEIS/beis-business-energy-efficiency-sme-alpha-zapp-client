using System;
using System.Linq;
using System.Threading.Tasks;
using RemindSME.Desktop.Configuration;
using RemindSME.Desktop.Models;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.Helpers
{
    public interface IHeatingReminderHelper {
        Task<string> GetWeatherDependentMessage();
    }

    public class HeatingReminderHelper : IHeatingReminderHelper
    {
        private const string Location = "London,UK";

        private const double RoomTemperature = 20;
        private const double MaximumTemperatureForHeating = 16;
        private const double MinimumTemperatureForAirConditioning = 24;

        private readonly IWeatherApiClient weatherApiClient;
        private readonly ISettings settings;

        public HeatingReminderHelper(IWeatherApiClient weatherApiClient, ISettings settings)
        {
            this.weatherApiClient = weatherApiClient;
            this.settings = settings;
        }

        public string DefaultMessage => Resources.Reminder_HeatingFirstLogin_Message;

        public async Task<string> GetWeatherDependentMessage()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation(Location);
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature.HasValue
                ? GetRecommendationMessageForTemperature(peakTemperature.Value)
                : DefaultMessage;
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            var midnightTonight = DateTime.Today.AddDays(1);
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < midnightTonight)
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private string GetRecommendationMessageForTemperature(double temperature)
        {
            if (TemperatureRequiresAirConditioning(temperature))
            {
                return $"It's going to be hot today (up to {temperature:F0}°C)! Please set the air conditioning to a sensible temperature and close the windows.";
            }

            if (TemperatureRequiresHeating(temperature))
            {
                return $"It's cold today ({temperature:F0}°C)! Please set the heating to a sensible temperature and close the windows.";
            }

            if (TemperatureIsAboveAverage(temperature))
            {
                return $"It's going to be {temperature:F0}°C today. Rather than use the air conditioning, could you open windows instead?";
            }

            return $"It's going to be {temperature:F0}°C today. Please consider whether you need the heating or air conditioning. Can you open windows instead?";
        }

        private bool ShouldShowAirConditioningMessage(double temperature) => TemperatureRequiresAirConditioning(temperature)
                                                                          && TemperatureIsHotterThanYesterday(temperature);

        private bool ShouldShowHeatingMessage(double temperature) => TemperatureRequiresHeating(temperature)
                                                                  && TemperatureIsColderThanYesterday(temperature);

        private bool TemperatureIsAboveAverage(double temperature) => temperature > RoomTemperature;
        private bool TemperatureRequiresHeating(double temperature) => temperature < MaximumTemperatureForHeating;
        private bool TemperatureRequiresAirConditioning(double temperature) => temperature > MinimumTemperatureForAirConditioning;

        private bool TemperatureIsDifferentFromYesterday(double temperature) => Math.Abs(TemperatureDifference(temperature)) > 2.5;
        private bool TemperatureIsHotterThanYesterday(double temperature) => TemperatureDifference(temperature) > +2.5;
        private bool TemperatureIsColderThanYesterday(double temperature) => TemperatureDifference(temperature) < -2.5;
        private double TemperatureDifference(double temperature) => temperature - settings.MostRecentPeakTemperature;
    }
}
