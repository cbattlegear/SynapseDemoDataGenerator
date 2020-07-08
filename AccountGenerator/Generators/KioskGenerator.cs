using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using System.IO;
using ProtoBuf;
using System.Globalization;

namespace SynapseDemoDataGenerator.Generators
{
    class KioskGenerator : Generator<Types.Kiosk>
    {
        public KioskGenerator(int numberToGenerate, int startingId, int splitCount) : base(numberToGenerate, startingId, splitCount)
        {
            //Passing the number to generate and starting ID to the base Generator class
        }
        public override void Generate()
        {
            Console.WriteLine("Generating Kiosks, starting with kiosk ID {0}", StartId);

            var newKiosk = new Faker<Types.Kiosk>("en")

                .RuleFor(u => u.KioskId, f => StartId++)
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.State, (f, u) => f.Address.StateAbbr())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")))
                .RuleFor(u => u.InstallDate, (f, u) => f.Date.Past(4));

            //Putting this in to deal with memory limits around 10 million records
            if (GenerateCount < Properties.Settings.Default.UseDiskThreshold)
            {
                Console.WriteLine("Beginning Kiosk generation in memory...");
                items = newKiosk.Generate(GenerateCount);
                ItemsCreated = items.Count();
                Console.WriteLine("Kiosk generation complete.");
            }
            else
            {
                Console.WriteLine("Generating over {0} items, generating on disk, this may be slow...", Properties.Settings.Default.UseDiskThreshold.ToString("N0", CultureInfo.InvariantCulture));
                Console.WriteLine("Beginning Kiosk generation on disk...");
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
                    items = newKiosk.Generate(createAmount);
                    using (FileStream fs = new FileStream("CacheData\\KioskCache-" + filecount.ToString() + ".bin", FileMode.Create))
                    {
                        Serializer.Serialize(fs, items);
                    }
                    filecount++;
                    numberLeft = numberLeft - createAmount;
                    ItemsCreated += items.Count();
                }
                Console.WriteLine("Kiosk generation complete.");
            }

        }
    }
}
