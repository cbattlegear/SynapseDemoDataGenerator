using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class UserLocations
    {
        public int LocationId { get; set; }
        public int AccountId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
