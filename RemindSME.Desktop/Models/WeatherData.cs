using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RemindSME.Desktop.Models
{
    public class WeatherForecast
    {
        public City City { get; set; }
        [JsonProperty("list")]
        public List<Forecast> Forecasts { get; set; }
    }

    public class CurrentWeather : WeatherData
    {
        [JsonProperty("name")]
        public string Location { get; set; }
        public Coord Coord { get; set; }
    }

    public class Forecast : WeatherData
    {
        [JsonProperty("dt_txt")]
        public DateTime Time { get; set; }
    }

    public class WeatherData
    {
        [JsonProperty("main")]
        public Measurements Measurements { get; set; }
        public List<Weather> Weather { get; set; }
    }

    public class City
    {
        public string Name { get; set; }
        public Coord Coord { get; set; }
    }

    public class Coord
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }
        [JsonProperty("lon")]
        public double Longitude { get; set; }
    }

    public class Weather
    {
        public string Main { get; set; }
        public string Description { get; set; }
    }

    public class Measurements
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }
        public double Pressure { get; set; }
        public double Humidity { get; set; }
    }
}
