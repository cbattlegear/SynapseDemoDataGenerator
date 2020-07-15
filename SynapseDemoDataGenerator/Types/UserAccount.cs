using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SynapseDemoDataGenerator.Types
{
    [ProtoContract]
    public class UserAccount
    {
        [ProtoMember(1)]
        public int UserId { get; set; }
        [ProtoMember(2)]
        public string FirstName { get; set; }
        [ProtoMember(3)]
        public string LastName { get; set; }
        [ProtoMember(4)]
        public string Email { get; set; }
        [ProtoMember(5)]
        public string PhoneNumber { get; set; }
        [ProtoMember(6)]
        public string Address { get; set; }
        [ProtoMember(7)]
        public string State { get; set; }
        [ProtoMember(8)]
        public string ZipCode { get; set; }
        [ProtoMember(9)]
        public string CreditCardNumber { get; set; }
        [ProtoMember(10)]
        public DateTime CreditCardExpiration { get; set; }
        [ProtoMember(11)]
        public DateTime MemberSince { get; set; }
    }
}
