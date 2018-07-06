namespace RemindSME.Desktop.Models
{
    public class WeatherIcon
    {
        private WeatherIcon(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public static WeatherIcon Sunny => new WeatherIcon("\u2600");
        public static WeatherIcon Cold => new WeatherIcon("\u2744");
        public static WeatherIcon Mixed => new WeatherIcon("\u26C5");
        public static WeatherIcon Thermometer => new WeatherIcon(char.ConvertFromUtf32(0x1F321));
    }
}
