using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using Bogus.DataSets;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace SynapseDemoDataGenerator
{
    class Program
    {
        //Variables to determine how many of each data set types to generate.
        public static int AccountAmount = 500000;
        public static int KioskAmount = 15000;
            
        static void Main(string[] args)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            GenerateAccounts(AccountAmount);
            GenerateKiosks(KioskAmount);
            
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.ReadLine();
        }

        static void GenerateKiosks(int generateCount)
        {
            Generators.KioskGenerator kioskgenerator = new Generators.KioskGenerator(generateCount, 10001);

            kioskgenerator.Generate();
            kioskgenerator.Output();

            Console.WriteLine("Completed! {0} Total Kiosks created\n", kioskgenerator.TotalGenerated - 1);
        }

        /* static void RentalEvents(int generateCount)
        {
            var RentalIds = 1001;

            //Counter for full run and files created
            var totalGenerated = 1;
            var filesCreated = 1;
            var intermediaryCount = 0;

            Console.WriteLine("Generating Rental Events, starting with Rental ID {0}", RentalIds);

            //create CSV buiilder and empty string
            var csv = new StringBuilder();
            var newLine = "";
            var csvHeader = string.Format("RentalId,UserId,RentalDate,RentalDuration,ScheduledReturnDate,ActualDuration,ActualReturnDate,MediaId,MediaType,RentalAmount,AdditionalFees,TotalAmount,KioskId");

            //csv.AppendLine(newLine);

            var faker = new Faker("en");

            var newUser = new Faker<Rental>()

                .RuleFor(u => u.RentalId, f => RentalIds++)
                .FinishWith((f, u) =>
                {

                    newLine = string.Format("{0}",
                                u.KioskId.ToString()
                              );

                    csv.AppendLine(newLine);

                    intermediaryCount++;

                    //Output Status
                    if (totalGenerated % 100000 == 0)
                    {

                        WriteCSV("Kiosks", csv.ToString(), csvHeader, filesCreated);
                        filesCreated++;
                        Console.WriteLine("Latest Kiosk Id: {0}, Total Created: {1}", u.KioskId, totalGenerated);
                        csv = new StringBuilder();
                        intermediaryCount = 0;
                    }

                    totalGenerated++;

                });


            var users = newUser.Generate(generateCount);

            //Flush remaining values generated if any
            if (intermediaryCount > 0)
            {
                WriteCSV("Kiosks", csv.ToString(), csvHeader, filesCreated);
            }

            Console.WriteLine("Completed! {0} Total Kiosks created\n", totalGenerated - 1);
        } */

        static void StreamingEvents(int generateCount)
        {
            //TODO
            //var Json = ConvertToJson.Dump("OBJECT HERE");
        }

        static void GenerateAccounts(int generateCount)
        {
            Generators.UserAccountGenerator useraccountgenerator = new Generators.UserAccountGenerator(generateCount, 100001);

            useraccountgenerator.Generate();
            useraccountgenerator.Output();

            Console.WriteLine("Completed! {0} Total Accounts created\n", useraccountgenerator.TotalGenerated - 1);
        }

    }
}
