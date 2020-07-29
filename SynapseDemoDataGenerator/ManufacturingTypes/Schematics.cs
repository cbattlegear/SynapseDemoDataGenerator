using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class Schematics
    {
        public int SchematicId { get; set; }
        public float SchematicVersion { get; set; }
        public string DeviceModel { get; set; }
        public string TemperatureSensor { get; set; }
        public string Processor { get; set; }
        public DateTime SchematicStart { get; set; }
        public DateTime SchematicEnd { get; set; }
    }
}
