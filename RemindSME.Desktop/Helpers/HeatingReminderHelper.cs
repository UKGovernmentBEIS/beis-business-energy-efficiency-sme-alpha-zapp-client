using System;
using System.Linq;
using System.Threading.Tasks;
using RemindSME.Desktop.Models;
using RemindSME.Desktop.Properties;

namespace RemindSME.Desktop.Helpers
{
    public interface IHeatingReminderHelper
    {
        Task<string> GetWeatherDependentMessage();
    }

    public class HeatingReminderHelper : IHeatingReminderHelper
    {
        private const string Location = "London,UK";

        private const double RoomTemperature = 20;
        private const double MaximumTemperatureForHeating = 16;
        private const double MinimumTemperatureForAirConditioning = 24;

        private readonly IWeatherApiClient weatherApiClient;

        public HeatingReminderHelper(IWeatherApiClient weatherApiClient)
        {
            this.weatherApiClient = weatherApiClient;
        }

        public string DefaultMessage => Resources.Reminder_HeatingFirstLogin_Message;

        public async Task<string> GetWeatherDependentMessage()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation(Location);
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature.HasValue
                ? GetReminderMessageForTemperature(peakTemperature.Value)
                : DefaultMessage;
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            var midnightTonight = DateTime.Today.AddDays(1);
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < midnightTonight)
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private string GetReminderMessageForTemperature(double temperature)
        {
            if (TemperatureRequiresAirConditioning(temperature))
            {
                return string.Format(Resources.Reminder_CheckAirCon_Message, temperature);
            }

            if (TemperatureRequiresHeating(temperature))
            {
                return string.Format(Resources.Reminder_CheckHeating_Message, temperature);
            }

            if (TemperatureIsAboveAverage(temperature))
            {
                return string.Format(Resources.Reminder_OpenWindows_Message, temperature);
            }

            return string.Format(Resources.Reminder_WeatherDefault_Message, temperature);
        }

        private static bool TemperatureIsAboveAverage(double temperature) => temperature > RoomTemperature;
        private static bool TemperatureRequiresHeating(double temperature) => temperature < MaximumTemperatureForHeating;
        private static bool TemperatureRequiresAirConditioning(double temperature) => temperature > MinimumTemperatureForAirConditioning;
    }
}
