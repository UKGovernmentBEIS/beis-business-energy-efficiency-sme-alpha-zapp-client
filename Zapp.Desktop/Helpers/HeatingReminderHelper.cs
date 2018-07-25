using System;
using System.Linq;
using System.Threading.Tasks;
using Caliburn.Micro;
using Zapp.Desktop.Configuration;
using Zapp.Desktop.Models;
using Zapp.Desktop.Properties;
using Zapp.Desktop.ViewModels;

namespace Zapp.Desktop.Helpers
{
    public interface IHeatingReminderHelper
    {
        Task<ReminderViewModel> GetWeatherDependentReminder();
    }

    public class HeatingReminderHelper : IHeatingReminderHelper
    {
        private const double MaximumTemperatureForHeating = 16;
        private const double MinimumTemperatureForAirConditioning = 24;

        private readonly ISettings settings;
        private readonly IWeatherApiClient weatherApiClient;

        public HeatingReminderHelper(ISettings settings, IWeatherApiClient weatherApiClient)
        {
            this.settings = settings;
            this.weatherApiClient = weatherApiClient;
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

        public async Task<ReminderViewModel> GetWeatherDependentReminder()
        {
            var forecast = await weatherApiClient.GetWeatherForecastForLocation(settings.Location);
            var peakTemperature = GetPeakTemperatureForToday(forecast);
            return peakTemperature.HasValue
                ? GetReminderForTemperature(peakTemperature.Value)
                : DefaultReminder;
        }

        private static double? GetPeakTemperatureForToday(WeatherForecast weatherForecast)
        {
            var midnightTonight = DateTime.Today.AddDays(1);
            return weatherForecast?.Forecasts
                .Where(forecast => forecast.Time < midnightTonight)
                .Max(forecast => forecast.Measurements.Temperature);
        }

        private static ReminderViewModel GetReminderForTemperature(double temperature)
        {
            var model = IoC.Get<ReminderViewModel>();

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
            else
            {
                model.Icon = NotificationIcon.Thermometer;
                model.Title = Resources.Reminder_WeatherDefault_Title;
                model.Message = string.Format(Resources.Reminder_WeatherDefault_Message, temperature);
            }

            return model;
        }

        private static bool TemperatureRequiresHeating(double temperature)
        {
            return temperature < MaximumTemperatureForHeating;
        }

        private static bool TemperatureRequiresAirConditioning(double temperature)
        {
            return temperature > MinimumTemperatureForAirConditioning;
        }
    }
}
