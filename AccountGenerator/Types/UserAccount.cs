using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseDemoDataGenerator.Types
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CreditCardNumber { get; set; }
        public DateTime CreditCardExpiration { get; set; }
        public DateTime MemberSince { get; set; }
    }
}
