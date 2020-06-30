using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseDemoDataGenerator.Generators
{
    public class Kiosk : Generator
    {
        public int KioskId { get; set; }
        public string Address { get; set; }
        public string ZipCode { get; set; }
        public DateTime InstallDate { get; set; }
    }

}