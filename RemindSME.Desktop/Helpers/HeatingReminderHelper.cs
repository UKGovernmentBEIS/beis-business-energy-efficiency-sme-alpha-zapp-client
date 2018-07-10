using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using RemindSME.Desktop.Models;
using RemindSME.Desktop.Properties;
using RemindSME.Desktop.ViewModels;

namespace RemindSME.Desktop.Helpers
{
    public interface IHeatingReminderHelper
    {
        Task<ReminderViewModel> GetWeatherDependentReminder();
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

        public async Task<ReminderViewModel> GetWeatherDependentReminder()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation(Location);
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature.HasValue
                ? GetReminderForTemperature(peakTemperature.Value)
                : DefaultReminder;
        }

        private static ReminderViewModel DefaultReminder
        {
            get
            {
                var model = IoC.Get<ReminderViewModel>();
                model.Icon = NotificationIcon.Thermometer;
                model.Title = Resources.Reminder_HeatingFirstLogin_Title;
                model.Message = Resources.Reminder_HeatingFirstLogin_Message;
                return model;
            }
        }

        private double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            var midnightTonight = DateTime.Today.AddDays(1);
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < midnightTonight)
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private ReminderViewModel GetReminderForTemperature(double temperature)
        {
            var model = IoC.Get<ReminderViewModel>();
            model.Icon = NotificationIcon.Thermometer;
            model.Title = Resources.Reminder_HeatingFirstLogin_Title;
            model.Message = string.Format(Resources.Reminder_WeatherDefault_Message, temperature);

            if (TemperatureRequiresAirConditioning(temperature))
            {
                model.Icon = NotificationIcon.Sunny;
                model.Title = Resources.Reminder_CheckAirCon_Title;
                model.Message = string.Format(Resources.Reminder_CheckAirCon_Message, temperature);
            }
            else if (TemperatureRequiresHeating(temperature))
            {
                model.Icon = NotificationIcon.Cold;
                model.Title = Resources.Reminder_CheckHeating_Title;
                model.Message = string.Format(Resources.Reminder_CheckHeating_Message, temperature);
            }
            else if (TemperatureIsAboveAverage(temperature))
            {
                model.Icon = NotificationIcon.Thermometer;
                model.Title = Resources.Reminder_CheckAirCon_Title;
                model.Message = string.Format(Resources.Reminder_OpenWindows_Message, temperature);
            }

            return model;
        }

        private static bool TemperatureIsAboveAverage(double temperature) => temperature > RoomTemperature;
        private static bool TemperatureRequiresHeating(double temperature) => temperature < MaximumTemperatureForHeating;
        private static bool TemperatureRequiresAirConditioning(double temperature) => temperature > MinimumTemperatureForAirConditioning;
    }
}
