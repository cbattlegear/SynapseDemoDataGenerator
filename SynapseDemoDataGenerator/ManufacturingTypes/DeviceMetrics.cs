using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class DeviceMetrics
    {
        public int DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public decimal Temperature { get; set; }
        public int TemperatureSetPointCool { get; set; }
        public int TemperatureSetPointHeat { get; set; }
        public decimal Humidity { get; set; }
        public decimal LocationTemperature { get; set; }
        public decimal LocationHumidity { get; set; }
    }
}
