using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;

namespace SynapseDemoDataGenerator.Generators
{
    class KioskGenerator : Generator
    {
        public List<Types.Kiosk> kiosks = new List<Types.Kiosk>();

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

            kiosks = newKiosk.Generate(GenerateCount);

        }

        public override void Output()
        {
            //create CSV buiilder and empty string
            var csv = new StringBuilder();
            var newLine = "";
            var csvHeader = string.Format("KioskId,Address,ZipCode,InstallDate");

            foreach (Types.Kiosk kiosk in kiosks)
            {
                newLine = string.Format("{0},{1},{2},{3}",
                                                 kiosk.KioskId.ToString()
                                                 , kiosk.Address
                                                 , kiosk.ZipCode
                                                 , kiosk.InstallDate.ToString("yyyy-MM-dd"));
                csv.AppendLine(newLine);

                IntermediaryCount++;

                //Output Status
                if (TotalGenerated % 100000 == 0)
                {

                    WriteCSV("Kiosks", csv.ToString(), csvHeader, FilesCreated);
                    FilesCreated++;
                    Console.WriteLine("Latest Kiosk Id: {0}, Total Created: {1}", kiosk.KioskId, TotalGenerated);
                    csv = new StringBuilder();
                    IntermediaryCount = 0;
                }

                TotalGenerated++;
            }
            //Flush remaining values generated if any
            if (IntermediaryCount > 0)
            {
                WriteCSV("Kiosks", csv.ToString(), csvHeader, FilesCreated);
            }
        }
    }
}
