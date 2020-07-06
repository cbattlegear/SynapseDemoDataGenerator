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
        public static int AccountAmount = 10000;
        public static int KioskAmount = 1000;
        public static int RentalAmount = 10000;

        //Starting IDs for each item type
        public static int AccountStartID = 100001;
        public static int KioskStartID = 10001;
        public static int RentalStartID = 10000;

        public static int SplitSize = 1000;

        static void Main(string[] args)
        {

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            GenerateAccounts(AccountAmount, AccountStartID);
            GenerateKiosks(KioskAmount, KioskStartID);
            GenerateRentals(RentalAmount, RentalStartID, AccountStartID, AccountStartID + AccountAmount, KioskStartID, KioskStartID + KioskAmount);
            
            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.ReadLine();
        }

        static void GenerateKiosks(int generateCount, int startID)
        {
            Generators.KioskGenerator kioskgenerator = new Generators.KioskGenerator(generateCount, startID);

            kioskgenerator.Generate();
            kioskgenerator.OutputCsv("Kiosks", SplitSize);

            Console.WriteLine("Completed! {0} Total Kiosks created\n", kioskgenerator.items.Count());
        }

        static void GenerateRentals(int generateCount, int startID, int accountStartId, int accountEndId, int kioskStartId, int kioskEndId)
        {
            Generators.RentalGenerator rentalgenerator = new Generators.RentalGenerator(generateCount, startID, accountStartId, accountEndId, kioskStartId, kioskEndId);

            rentalgenerator.Generate();
            rentalgenerator.OutputCsv("Rentals", SplitSize);

            Console.WriteLine("Completed! {0} Total Rentals created\n", rentalgenerator.items.Count());
        }

        static void StreamingEvents(int generateCount)
        {
            //TODO
            //var Json = ConvertToJson.Dump("OBJECT HERE");
        }

        static void GenerateAccounts(int generateCount, int startID)
        {
            Generators.UserAccountGenerator useraccountgenerator = new Generators.UserAccountGenerator(generateCount, startID);

            useraccountgenerator.Generate();
            useraccountgenerator.OutputCsv("Accounts", SplitSize);

            Console.WriteLine("Completed! {0} Total Accounts created\n", useraccountgenerator.items.Count());
        }

    }
}
