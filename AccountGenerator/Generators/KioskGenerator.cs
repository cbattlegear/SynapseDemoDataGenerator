using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace SynapseDemoDataGenerator.Generators
{
    class KioskGenerator : Generator<Types.Kiosk>
    {
        public KioskGenerator(int numberToGenerate, int startingId) : base(numberToGenerate, startingId)
        {
            //Passing the number to generate and starting ID to the base Generator class
        }
        public override void Generate()
        {
            Console.WriteLine("Generating Kiosks, starting with kiosk ID {0}", StartId);

            var newKiosk = new Faker<Types.Kiosk>("en")

                .RuleFor(u => u.KioskId, f => StartId++)
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")))
                .RuleFor(u => u.InstallDate, (f, u) => f.Date.Past(4));

            items = newKiosk.Generate(GenerateCount);

        }
    }
}
