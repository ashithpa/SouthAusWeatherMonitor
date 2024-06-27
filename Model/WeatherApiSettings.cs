using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model
{
    public class WeatherApiSettings
    {
        public string BaseUrl { get; set; }
        public int TimeoutInSeconds { get; set; } = 30; // Default timeout of 30 seconds
    }
}