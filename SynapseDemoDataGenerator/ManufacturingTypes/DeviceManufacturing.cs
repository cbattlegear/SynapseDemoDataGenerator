using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class DeviceManufacturing
    {
        public int DeviceId { get; set; }
        public string SerialNumber { get; set; }
        public int SchematicId { get; set; }
        public int FactoryLine { get; set; }
        public int BatchNumber { get; set; }
        public DateTime BuildStart { get; set; }
        public DateTime BuildEnd { get; set; }
    }
}
