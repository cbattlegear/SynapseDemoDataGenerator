using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace SynapseDemoDataGenerator.Types
{
    
    [ProtoContract]
    public class Rental
    {
        [ProtoMember(1)]
        public int RentalId { get; set; }
        [ProtoMember(2)]
        public int UserId { get; set; }
        [ProtoMember(3)]
        public DateTime RentalDate { get; set; }
        [ProtoMember(4)]
        public int RentalDuration { get; set; }
        [ProtoMember(5)]
        public DateTime ScheduledReturnDate { get; set; }
        [ProtoMember(6)]
        public int ActualDuration { get; set; }
        [ProtoMember(7)]
        public DateTime ActualReturnDate { get; set; }
        [ProtoMember(8)]
        public int MediaId { get; set; }
        [ProtoMember(9)]
        public string MediaType { get; set; }
        [ProtoMember(10)]
        public decimal RentalAmount { get; set; }
        [ProtoMember(11)]
        public decimal AdditionalFees { get; set; }
        [ProtoMember(12)]
        public decimal TotalAmount { get; set; }
        [ProtoMember(13)]
        public int KioskId { get; set; }
    }

}