﻿using System;
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

        private static Random rand = new Random();
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
                .RuleFor(u => u.UserId, (f, u) => WeightedInteger(StartingUserId, EndingUserId, 0.55, 0.4, 0.05))
                .RuleFor(u => u.RentalDate, (f, u) => f.Date.Past(8))
                .RuleFor(u => u.RentalDuration, (f, u) => f.Random.Number(1, 7))
                .RuleFor(u => u.ScheduledReturnDate, (f, u) => u.RentalDate.AddDays(u.RentalDuration))
                // 80% chance they returned on time, random chance of late or early after the 80%
                .RuleFor(u => u.ActualDuration, (f, u) => f.Random.Number(u.RentalDuration - 3 > 0 ? u.RentalDuration - 3 : 1, u.RentalDuration + 3).OrDefault(f, 0.8f, u.RentalDuration))
                .RuleFor(u => u.ActualReturnDate, (f, u) => u.RentalDate.AddDays(u.ActualDuration))
                .RuleFor(u => u.MediaId, (f, u) => WeightedInteger(1, 1000, 0.6, 0.3, 0.1))
                .RuleFor(u => u.MediaType, (f, u) => f.PickRandom(mediaTypes))
                // The M makes sure the sucker stays decimal and we charge $1.25 a day for renting
                .RuleFor(u => u.RentalAmount, (f, u) => u.RentalDuration * 1.25m)
                // $3.10 a day late fee yo! (if actualduration is larger than rental duration, find out by how many days and charge them aggressively)
                .RuleFor(u => u.AdditionalFees, (f, u) => u.ActualDuration > u.RentalDuration ? (u.ActualDuration - u.RentalDuration) * 3.10m : 0.00m)
                .RuleFor(u => u.TotalAmount, (f, u) => u.RentalAmount + u.AdditionalFees)
                .RuleFor(u => u.KioskId, (f, u) => WeightedInteger(StartingKioskId, EndingKioskId, 0.6, 0.3, 0.1));

            items = newRental.Generate(GenerateCount);
        }

        public int WeightedInteger(int beginInt, int endInt, double weightHigh, double weightMid, double weightLow)
        {
            double weightedBase = rand.NextDouble();

            int range = endInt - beginInt;
            int highRange = (int)(range * weightLow);
            int midRange = (int)(range * weightMid);
            int lowRange = (int)(range * weightHigh);

            if (weightedBase <= weightHigh)
            {
                return rand.Next(beginInt, beginInt + highRange + 1);
            } else if(weightedBase > weightHigh && weightedBase <= weightHigh + weightMid)
            {
                return rand.Next(beginInt + highRange + 1, beginInt + highRange + midRange + 1);
            } else
            {
                return rand.Next(beginInt + highRange + midRange + 1, endInt + 1);
            }
        }
    }
}
