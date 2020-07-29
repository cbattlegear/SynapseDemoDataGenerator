using System;
using System.Collections.Generic;
using System.Text;

namespace SynapseDemoDataGenerator.ManufacturingTypes
{
    class UserAccounts
    {
        public int AccountId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
