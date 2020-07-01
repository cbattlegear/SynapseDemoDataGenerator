using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;

namespace SynapseDemoDataGenerator.Generators
{
    class UserAccountGenerator : Generator<Types.UserAccount>
    {
        public UserAccountGenerator(int numberToGenerate, int startingId) : base(numberToGenerate, startingId)
        {
            //Passing the number to generate and starting ID to the base Generator class
        }
        public override void Generate()
        {
            Console.WriteLine("Generating User Accounts, starting with UserID {0}", StartId);

            //csv.AppendLine(newLine);

            //var faker = new Faker("en");

            var newUser = new Faker<Types.UserAccount>("en")

                .RuleFor(u => u.UserId, f => StartId++)
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

            items = newUser.Generate(GenerateCount);
        }
    }
}
