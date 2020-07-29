using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class DeviceRegistrations
    {
        public int RegistrationId { get; set; }
        public int DeviceId { get; set; }
        public int AccountId { get; set; }
        public int LocationId { get; set; }
        public DateTime RegistrationDate { get; set; }
    }
}
