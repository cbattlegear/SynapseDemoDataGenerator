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
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Runtime.InteropServices;

namespace SynapseDemoDataGenerator
{
    public class CommandLineOptions
    {
        public int numberOfAccounts { get; set; }
        public int numberOfKiosks { get; set; }
        public int numberOfRentals { get; set; }
        public int accountsStartId { get; set; }
        public int kiosksStartId { get; set; }
        public int rentalStartId { get; set; }
        public int recordsPerFile { get; set; }
        public int numberOfEach { get; set; }
        public int startId { get; set; }
    }
    class Program
    {
        //Variables to determine how many of each data set types to generate.
        public static int AccountAmount = 10000;
        public static int KioskAmount = 1000;
        public static int RentalAmount = 10000;

        //Starting IDs for each item type
        public static int AccountStartID = 100001;
        public static int KioskStartID = 10001;
        public static int RentalStartID = 10001;

        public static int SplitSize = 1000;

        public static async Task<int> Main(params string[] args)
        {
            var command = new RootCommand
            {
                new Option<int>("--numberofaccounts", getDefaultValue: () => 10000, description: "The number of User Accounts to generate"),
                new Option<int>("--numberofkiosks", getDefaultValue: () => 1000, description: "The number of Kiosks to generate"),
                new Option<int>("--numberofrentals", getDefaultValue: () => 10000, description: "The number of Rental Transactions to generate"),

                new Option<int>("--accountstartid", getDefaultValue: () => 100001, description: "The starting id number (as an integer) for the generated User Accounts"),
                new Option<int>("--kioskstartid", getDefaultValue: () => 10001, description: "The starting id number (as an integer) for the generated Kiosks"),
                new Option<int>("--rentalstartid", getDefaultValue: () => 10001, description: "The starting id number (as an integer) for the generated Rental Transactions"),

                new Option<int>("--recordsperfile", getDefaultValue: () => 0, description: "(Optional) Number of records to insert into a file before creating a new file"),

                new Option<int>("--numberofeach", description: "(Optional) Use to generate the same number of objects for all types"),
                new Option<int>("--startid", description: "(Optional) Use to have all objects start on the same ID number")
            };

            command.Handler = CommandHandler.Create(
                (CommandLineOptions commandLineOptions) =>
                {
                    Generate(commandLineOptions);
                }
                );

            return await command.InvokeAsync(args);
        }

        static public void Generate(CommandLineOptions commandLineOptions)
        {
            if(commandLineOptions.startId != 0)
            {
                AccountStartID = commandLineOptions.startId;
                KioskStartID = commandLineOptions.startId;
                RentalStartID = commandLineOptions.startId;
            } else
            {
                AccountStartID = commandLineOptions.accountsStartId;
                KioskStartID = commandLineOptions.kiosksStartId;
                RentalStartID = commandLineOptions.rentalStartId;
            }

            if (commandLineOptions.numberOfEach != 0)
            {
                AccountAmount = commandLineOptions.numberOfEach;
                KioskAmount = commandLineOptions.numberOfEach;
                RentalAmount = commandLineOptions.numberOfEach;
            } else
            {
                AccountAmount = commandLineOptions.numberOfAccounts;
                KioskAmount = commandLineOptions.numberOfKiosks;
                RentalAmount = commandLineOptions.numberOfRentals;
            }

            SplitSize = commandLineOptions.recordsPerFile;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            GenerateAccounts(AccountAmount, AccountStartID, SplitSize);
            GenerateKiosks(KioskAmount, KioskStartID, SplitSize);
            GenerateRentals(RentalAmount, RentalStartID, AccountStartID, AccountStartID + AccountAmount, KioskStartID, KioskStartID + KioskAmount, SplitSize);

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        static void GenerateKiosks(int generateCount, int startID, int splitCount)
        {
            Generators.KioskGenerator kioskgenerator = new Generators.KioskGenerator(generateCount, startID);

            kioskgenerator.Generate();
            kioskgenerator.OutputCsv("Kiosks", splitCount);

            Console.WriteLine("Completed! {0} Total Kiosks created\n", kioskgenerator.items.Count());
        }

        static void GenerateRentals(int generateCount, int startID, int accountStartId, int accountEndId, int kioskStartId, int kioskEndId, int splitCount)
        {
            Generators.RentalGenerator rentalgenerator = new Generators.RentalGenerator(generateCount, startID, accountStartId, accountEndId, kioskStartId, kioskEndId);

            rentalgenerator.Generate();
            rentalgenerator.OutputCsv("Rentals", splitCount);

            Console.WriteLine("Completed! {0} Total Rentals created\n", rentalgenerator.items.Count());
        }

        static void StreamingEvents(int generateCount)
        {
            //TODO
            //var Json = ConvertToJson.Dump("OBJECT HERE");
        }

        static void GenerateAccounts(int generateCount, int startID, int splitCount)
        {
            Generators.UserAccountGenerator useraccountgenerator = new Generators.UserAccountGenerator(generateCount, startID);

            useraccountgenerator.Generate();
            useraccountgenerator.OutputCsv("Accounts", splitCount);

            Console.WriteLine("Completed! {0} Total Accounts created\n", useraccountgenerator.items.Count());
        }

    }
}
