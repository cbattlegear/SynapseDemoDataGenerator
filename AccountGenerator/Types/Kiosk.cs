using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SynapseDemoDataGenerator.Types
{
    [ProtoContract]
    public class Kiosk
    {
        [ProtoMember(1)]
        public int KioskId { get; set; }
        [ProtoMember(2)]
        public string Address { get; set; }
        [ProtoMember(3)]
        public string State { get; set; }
        [ProtoMember(4)]
        public string ZipCode { get; set; }
        [ProtoMember(5)]
        public DateTime InstallDate { get; set; }
    }

}