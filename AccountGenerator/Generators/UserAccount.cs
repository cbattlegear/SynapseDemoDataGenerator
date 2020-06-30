using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;

namespace SynapseDemoDataGenerator.Generators
{
    public class UserAccount : Generator
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

        public void Generate()
        {
            //'Seed' for User ID starting values
            var userIds = 100001;

            //Counter for full run and files created
            var totalGenerated = 1;
            var filesCreated = 1;
            var intermediaryCount = 0;

            Console.WriteLine("Generating User Accounts, starting with UserID {0}", userIds);

            //create CSV buiilder and empty string
            var csv = new StringBuilder();
            var newLine = "";
            var csvHeader = string.Format("UserID,FirstName,LastName,Email,PhoneNumber,Address,State,ZipCode,CreditCardNumber,CreditCardExpiration,MemberSince");

            //csv.AppendLine(newLine);

            //var faker = new Faker("en");

            var newUser = new Faker<UserAccount>("en")

                .RuleFor(u => u.UserId, f => userIds++)
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(f.Person.Gender))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(f.Person.Gender))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.PhoneNumber, (f, u) => f.Phone.PhoneNumberFormat())
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.State, (f, u) => f.Address.StateAbbr())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")))
                .RuleFor(u => u.CreditCardNumber, (f, u) => f.Finance.CreditCardNumber(CardType.Visa))
                .RuleFor(u => u.CreditCardExpiration, (f, u) => f.Date.Future(4))
                .RuleFor(u => u.MemberSince, (f, u) => f.Date.Past(8));

            var users = newUser.Generate(GenerateCount);

            foreach (UserAccount user in users)
            {
                newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                             user.UserId.ToString()
                             , user.FirstName
                             , user.LastName
                             , user.Email
                             , user.PhoneNumber
                             , user.Address
                             , user.State
                             , user.ZipCode
                             , user.CreditCardNumber
                             , user.CreditCardExpiration.ToString("yyyy-MM")
                             , user.MemberSince.ToString("yyyy-MM-dd"));
                csv.AppendLine(newLine);

                intermediaryCount++;

                //Output Status
                if (totalGenerated % 100000 == 0)
                {

                    WriteCSV("Accounts", csv.ToString(), csvHeader, filesCreated);
                    filesCreated++;
                    Console.WriteLine("Latest User Id: {0}, Total Created: {1}", user.UserId, totalGenerated);
                    csv = new StringBuilder();
                    intermediaryCount = 0;
                }

                totalGenerated++;
            }
            //Flush remaining values generated if any
            if (intermediaryCount > 0)
            {
                WriteCSV("Accounts", csv.ToString(), csvHeader, filesCreated);
            }
        }
    }
}
