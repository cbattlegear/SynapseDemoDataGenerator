using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseDemoDataGenerator.Types
{
    public class Rental
    {
        public int RentalId { get; set; }
        public int UserId { get; set; }
        public DateTime RentalDate { get; set; }
        public int RentalDuration { get; set; }
        public DateTime ScheduledReturnDate { get; set; }
        public int ActualDuration { get; set; }
        public int ActualReturnDate { get; set; }
        public int MediaId { get; set; }
        public string MediaType { get; set; }
        public int RentalAmount { get; set; }
        public int AdditionalFees { get; set; }
        public int TotalAmount { get; set; }
        public int KioskId { get; set; }
    }

}