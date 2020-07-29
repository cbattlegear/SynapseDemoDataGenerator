using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;
using ProtoBuf;
using System.Globalization;
using Bogus.Distributions.Gaussian;
using ExtensionMethods;

namespace SynapseDemoDataGenerator.Generators
{
    class RentalGenerator : Generator<RetailTypes.Rental>
    {
        private int StartingUserId;
        private int EndingUserId;

        private int StartingKioskId;
        private int EndingKioskId;

        private static Random rand = new Random();
        public RentalGenerator(int numberToGenerate, int startingId, int userIdStart, int userIdEnd, int kioskIdStart, int kioskIdEnd, int splitCount) : base(numberToGenerate, startingId, splitCount)
        {
            //Passing the number to generate and starting ID to the base Generator class
            StartingUserId = userIdStart;
            EndingUserId = userIdEnd;

            StartingKioskId = kioskIdStart;
            EndingKioskId = kioskIdEnd;
        }
        public override void Generate()
        {
            int DiskUseThreshold = Convert.ToInt32(Program.Configuration["UseDiskThreshold"]);
            Console.WriteLine("Generating Rentals, starting with UserID {0}", StartId);

            var mediaTypes = new[] { "BetaMax", "DVD", "Bluray", "VHS", "Laserdisc"};

            //Create our array of movies and give them a good shake
            var movieArray = Enumerable.Range(1, 1000).ToArray();
            rand.Shuffle(movieArray);

            var newRental = new Faker<RetailTypes.Rental>("en")

                .RuleFor(u => u.RentalId, f => StartId++)
                .RuleFor(u => u.UserId, (f, u) => WeightedInteger(StartingUserId, EndingUserId))
                .RuleFor(u => u.RentalDate, (f, u) => f.Date.Past(8))
                .RuleFor(u => u.RentalDuration, (f, u) => f.Random.Number(1, 7))
                .RuleFor(u => u.ScheduledReturnDate, (f, u) => u.RentalDate.AddDays(u.RentalDuration))
                // 80% chance they returned on time, random chance of late or early after the 80%
                .RuleFor(u => u.ActualDuration, (f, u) => f.Random.Number(u.RentalDuration - 3 > 0 ? u.RentalDuration - 3 : 1, u.RentalDuration + 3).OrDefault(f, 0.8f, u.RentalDuration))
                .RuleFor(u => u.ActualReturnDate, (f, u) => u.RentalDate.AddDays(u.ActualDuration))
                .RuleFor(u => u.MediaId, (f, u) => movieArray[WeightedInteger(0, 999)])
                .RuleFor(u => u.MediaType, (f, u) => mediaTypes[WeightedInteger(0, 4)])
                // The M makes sure the sucker stays decimal and we charge $1.25 a day for renting
                .RuleFor(u => u.RentalAmount, (f, u) => u.RentalDuration * 1.25m)
                // $3.10 a day late fee yo! (if actualduration is larger than rental duration, find out by how many days and charge them aggressively)
                .RuleFor(u => u.AdditionalFees, (f, u) => u.ActualDuration > u.RentalDuration ? (u.ActualDuration - u.RentalDuration) * 3.10m : 0.00m)
                .RuleFor(u => u.TotalAmount, (f, u) => u.RentalAmount + u.AdditionalFees)
                .RuleFor(u => u.KioskId, (f, u) => WeightedInteger(StartingKioskId, EndingKioskId));
            
            //Putting this in to deal with memory limits around 10 million records
            if(GenerateCount < DiskUseThreshold)
            {
                Console.WriteLine("Beginning Rental generation in memory...");
                items = newRental.Generate(GenerateCount);
                ItemsCreated = items.Count();
                Console.WriteLine("Rental generation complete.");
            } else
            {
                Console.WriteLine("Generating over {0} items, generating on disk, this may be slow...", DiskUseThreshold.ToString("N0", CultureInfo.InvariantCulture));
                Console.WriteLine("Beginning Rental generation on disk...");
                int filecount = 1;
                int numberLeft = GenerateCount;
                Directory.CreateDirectory("CacheData");

                // Handle large file scenarios with no split
                // Essentially if someone says they want 40mil records, but want them all in one file, we still need to decide where to cache at.
                int splitHold = SplitAmount;
                if (SplitAmount <= 0)
                    splitHold = DiskUseThreshold;

                while(numberLeft > 0)
                {
                    int createAmount = splitHold > numberLeft ? numberLeft : splitHold;
                    items = newRental.Generate(createAmount);
                    using (FileStream fs = new FileStream("CacheData\\RentalCache-" + filecount.ToString() + ".bin", FileMode.Create))
                    {
                        Serializer.Serialize(fs, items);
                    }
                    filecount++;
                    numberLeft = numberLeft - createAmount;
                    ItemsCreated += items.Count();
                }
                Console.WriteLine("Rental generation complete.");
            }
            
        }
    }
}
