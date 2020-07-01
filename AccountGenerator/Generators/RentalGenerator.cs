using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Bogus.Extensions;

namespace SynapseDemoDataGenerator.Generators
{
    class RentalGenerator : Generator<Types.Rental>
    {
        private int StartingUserId;
        private int EndingUserId;

        private int StartingKioskId;
        private int EndingKioskId;
        public RentalGenerator(int numberToGenerate, int startingId, int userIdStart, int userIdEnd, int kioskIdStart, int kioskIdEnd) : base(numberToGenerate, startingId)
        {
            //Passing the number to generate and starting ID to the base Generator class
            StartingUserId = userIdStart;
            EndingUserId = userIdEnd;

            StartingKioskId = kioskIdStart;
            EndingKioskId = kioskIdEnd;
        }
        public override void Generate()
        {
            Console.WriteLine("Generating Rentals, starting with UserID {0}", StartId);

            var mediaTypes = new[] { "Bluray", "DVD", "VHS", "BetaMax", "Laserdisc" };

            var newRental = new Faker<Types.Rental>("en")

                .RuleFor(u => u.RentalId, f => StartId++)
                .RuleFor(u => u.UserId, (f, u) => f.Random.Number(StartingUserId, EndingUserId))
                .RuleFor(u => u.RentalDate, (f, u) => f.Date.Past(8))
                .RuleFor(u => u.RentalDuration, (f, u) => f.Random.Number(1, 7))
                .RuleFor(u => u.ScheduledReturnDate, (f, u) => u.RentalDate.AddDays(u.RentalDuration))
                // 80% chance they returned on time, random chance of late or early after the 80%
                .RuleFor(u => u.ActualDuration, (f, u) => f.Random.Number(u.RentalDuration - 3 > 0 ? u.RentalDuration - 3 : 1, u.RentalDuration + 3).OrDefault(f, 0.8f, u.RentalDuration))
                .RuleFor(u => u.ActualReturnDate, (f, u) => u.RentalDate.AddDays(u.ActualDuration))
                .RuleFor(u => u.MediaId, (f, u) => f.Random.Number(1, 1000))
                .RuleFor(u => u.MediaType, (f, u) => f.PickRandom(mediaTypes))
                // The M makes sure the sucker stays decimal and we charge 2 dollars a day for renting
                .RuleFor(u => u.RentalAmount, (f, u) => u.RentalDuration * 2.00m)
                // 5 bucks a day late fee yo! (if actualduration is larger than rental duration, find out by how many days and charge them aggressively
                .RuleFor(u => u.AdditionalFees, (f, u) => u.ActualDuration > u.RentalDuration ? (u.ActualDuration - u.RentalDuration) * 5.00m : 0.00m)
                .RuleFor(u => u.TotalAmount, (f, u) => u.RentalAmount + u.AdditionalFees)
                .RuleFor(u => u.KioskId, (f, u) => f.Random.Number(StartingKioskId, EndingKioskId));

            items = newRental.Generate(GenerateCount);
        }
    }
}
