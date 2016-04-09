using System;

namespace Looksfamiliar.d2c2d.MessageModels
{
    public class Climate : MessageBase
    {
        private const double C1 = -42.379;
        private const double C2 = 2.04901523;
        private const double C3 = 10.14333127;
        private const double C4 = -0.22475541;
        private const double C5 = -6.83783 * (10^-3);
        private const double C6 = -5.481717 * (10^-2);
        private const double C7 = 1.22874 * (10^-3);
        private const double C8 = 8.5282 * (10^-4);
        private const double C9 = -1.99 * (10^-6);

        public Climate()
        {
            Humidity = 0;
            Temperature = 0;
            MessageType = MessageTypeEnum.Climate;
        }

        public double Humidity { get; set; }
        public double Temperature { get; set; }

        // source; https://en.wikipedia.org/wiki/Heat_index
        // The formula below approximates the heat index in degrees Fahrenheit, to within ±1.3 °F. 
        // It is the result of a multivariate fit (temperature equal to or greater than 80 °F and 
        // relative humidity equal to or greater than 40%) to a model of the human body.[7][8] 
        // This equation reproduces the  NOAA National Weather Service table (except the values 
        // at 90 °F & 45%/70% relative humidity vary unrounded by less than -1/+1, respectively).
        public double HeatIndex
        {
            get
            {
                var r = Convert.ToInt32(Humidity);
                var t = Convert.ToInt32(Temperature);
                return C1 + (C2*t) + (C3*r) + (C4*t*r) + (C5*(t ^ 2)) +(C6*(r ^ 2)) + (C7*(t ^ 2)*r) + (C8*t*(r ^ 2)) + (C9*(t ^ 2)*(r ^ 2));
            }
        }
    }

    public class ClimateSettings
    {
        public ClimateSettings()
        {
            MinHumidity = 0;
            MaxHumiditiy = 0;
            MinTemperature = 0;
            MaxTemperature = 0;
        }

        public double MinHumidity { get; set; }
        public double MaxHumiditiy { get; set; }
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
    }
}
