using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using System.IO;
using ProtoBuf;
using System.Globalization;

namespace SynapseDemoDataGenerator.Generators
{
    class UserAccountGenerator : Generator<Types.UserAccount>
    {
        public UserAccountGenerator(int numberToGenerate, int startingId, int splitCount) : base(numberToGenerate, startingId, splitCount)
        {
            //Passing the number to generate and starting ID to the base Generator class
        }
        public override void Generate()
        {
            Console.WriteLine("Generating User Accounts, starting with UserID {0}", StartId);

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

            //Putting this in to deal with memory limits around 10 million records
            if (GenerateCount < Properties.Settings.Default.UseDiskThreshold)
            {
                Console.WriteLine("Beginning User Account generation in memory...");
                items = newUser.Generate(GenerateCount);
                ItemsCreated = items.Count();
                Console.WriteLine("User Account generation complete.");
            }
            else
            {
                Console.WriteLine("Generating over {0} items, generating on disk, this may be slow...", Properties.Settings.Default.UseDiskThreshold.ToString("N1", CultureInfo.InvariantCulture));
                Console.WriteLine("Beginning User Account generation on disk...");
                int filecount = 1;
                int numberLeft = GenerateCount;
                Directory.CreateDirectory("CacheData");

                // Handle large file scenarios with no split
                // Essentially if someone says they want 40mil records, but want them all in one file, we still need to decide where to cache at.
                int splitHold = SplitAmount;
                if (SplitAmount <= 0)
                    splitHold = Properties.Settings.Default.UseDiskThreshold;

                while (numberLeft > 0)
                {
                    int createAmount = splitHold > numberLeft ? numberLeft : splitHold;
                    items = newUser.Generate(createAmount);
                    using (FileStream fs = new FileStream("CacheData\\UserCache-" + filecount.ToString() + ".bin", FileMode.Create))
                    {
                        Serializer.Serialize(fs, items);
                    }
                    filecount++;
                    numberLeft = numberLeft - createAmount;
                    ItemsCreated += items.Count();
                }
                Console.WriteLine("User Account generation complete.");
            }
        }
    }
}
