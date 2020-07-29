using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class QualityControl
    {
        public int DeviceId { get; set; }
        public bool PowerCycle { get; set; }
        public bool SoftwareOperation { get; set; }
        public bool ClimateReadingTest { get; set; }
        public bool NetworkConnectivity { get; set; }
        public bool QcPass { get; set; }
    }
}
