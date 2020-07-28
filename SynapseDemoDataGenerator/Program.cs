using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace SynapseDemoDataGenerator
{
    public enum FormatOptions { csv, json }
    public class RetailCommandLineOptions
    {
        public int numberOfAccounts { get; set; }
        public int numberOfKiosks { get; set; }
        public int numberOfRentals { get; set; }
        public int numberOfStreamEvents { get; set; }
        public int accountStartId { get; set; }
        public int kioskStartId { get; set; }
        public int rentalStartId { get; set; }
        public int recordsPerFile { get; set; }
        public int numberOfEach { get; set; }
        public int startId { get; set; }
        public DirectoryInfo output { get; set; }
        public FormatOptions format { get; set; }
    }
    class Program
    {
        //Variables to determine how many of each data set types to generate.
        public static int AccountAmount = 10000;
        public static int KioskAmount = 1000;
        public static int RentalAmount = 10000;
        public static int StreamEventAmount = 10000;

        //Starting IDs for each item type
        public static int AccountStartID = 100001;
        public static int KioskStartID = 10001;
        public static int RentalStartID = 10001;

        public static int SplitSize = 0;

        private static Stopwatch stopWatch = new Stopwatch();

        public static async Task<int> Main(params string[] args)
        {
            var root = new RootCommand
            {

            };

            var retail = new Command("retail")
            {
                new Option<int>("--numberofaccounts", getDefaultValue: () => 10000, description: "The number of User Accounts to generate"),
                new Option<int>("--numberofkiosks", getDefaultValue: () => 1000, description: "The number of Kiosks to generate"),
                new Option<int>("--numberofrentals", getDefaultValue: () => 10000, description: "The number of Rental Transactions to generate"),
                new Option<int>("--numberofstreamevents", getDefaultValue: () => 10000, description: "The number of Streaming Events to generate"),

                new Option<int>("--accountstartid", getDefaultValue: () => 100001, description: "The starting id number (as an integer) for the generated User Accounts"),
                new Option<int>("--kioskstartid", getDefaultValue: () => 10001, description: "The starting id number (as an integer) for the generated Kiosks"),
                new Option<int>("--rentalstartid", getDefaultValue: () => 10001, description: "The starting id number (as an integer) for the generated Rental Transactions"),

                new Option<int>("--recordsperfile", getDefaultValue: () => 0, description: "(Optional) Number of records to insert into a file before creating a new file"),

                new Option<int>("--numberofeach", description: "(Optional) Use to generate the same number of objects for all types"),
                new Option<int>("--startid", description: "(Optional) Use to have all objects start on the same ID number"),

                new Option<FormatOptions>("--format", getDefaultValue: () => FormatOptions.csv, description: "(Optional) Format to output currently can be csv or json, defaults to csv"),

                new Option<string>("--output", description: "(Optional) Output directory for the created data")
            };

            retail.AddAlias("rental");
            retail.Description = "Generate Retail data from a simulated Rental company. It will include User Accounts, Kiosks, and Rental Transactions";

            retail.Handler = CommandHandler.Create(
                (RetailCommandLineOptions commandLineOptions) =>
                {
                    GenerateRetail(commandLineOptions);
                }
                );

            root.Add(retail);

            var healthcare = new Command("healthcare");
            healthcare.Description = "Currently Unimplemented Healthcare data generation";
            healthcare.Handler = CommandHandler.Create(
                () =>
                {
                    Console.WriteLine("Healthcare data generation is currently not implemented");
                }
                );

            root.Add(healthcare);

            var financial = new Command("financial");
            financial.Description = "Currently Unimplemented Financial data generation";
            financial.Handler = CommandHandler.Create(
                () =>
                {
                    Console.WriteLine("Financial data generation is currently not implemented");
                }
                );

            root.Add(financial);

            var manufacturing = new Command("manufacturing");
            manufacturing.Description = "Currently Unimplemented Manufacturing data generation";
            manufacturing.Handler = CommandHandler.Create(
                () =>
                {
                    Console.WriteLine("Manufacturing data generation is currently not implemented");
                }
                );

            root.Add(manufacturing);

            return await root.InvokeAsync(args);
        }

        static public void GenerateRetail(RetailCommandLineOptions commandLineOptions)
        {
            string outputDirectory = VerifyDirectory(commandLineOptions.output);

            int DiskUseThreshold = Convert.ToInt32(Program.Configuration["UseDiskThreshold"]);
            if (commandLineOptions.startId != 0)
            {
                AccountStartID = commandLineOptions.startId;
                KioskStartID = commandLineOptions.startId;
                RentalStartID = commandLineOptions.startId;
            }
            else
            {
                AccountStartID = commandLineOptions.accountStartId;
                KioskStartID = commandLineOptions.kioskStartId;
                RentalStartID = commandLineOptions.rentalStartId;
            }

            if (commandLineOptions.numberOfEach != 0)
            {
                AccountAmount = commandLineOptions.numberOfEach;
                KioskAmount = commandLineOptions.numberOfEach;
                RentalAmount = commandLineOptions.numberOfEach;
                StreamEventAmount = commandLineOptions.numberOfEach;
            }
            else
            {
                AccountAmount = commandLineOptions.numberOfAccounts;
                KioskAmount = commandLineOptions.numberOfKiosks;
                RentalAmount = commandLineOptions.numberOfRentals;
                StreamEventAmount = commandLineOptions.numberOfStreamEvents;
            }

            //If our split size is over the set limit (in Settings) make it a the setting value as that's where we are limiting list size for memory limitations
            SplitSize = commandLineOptions.recordsPerFile <= DiskUseThreshold ? commandLineOptions.recordsPerFile : DiskUseThreshold;

            stopWatch.Start();

            GenerateAccounts(AccountAmount, AccountStartID, SplitSize, outputDirectory, commandLineOptions.format);
            GenerateKiosks(KioskAmount, KioskStartID, SplitSize, outputDirectory, commandLineOptions.format);
            GenerateRentals(RentalAmount, RentalStartID, AccountStartID, AccountStartID + AccountAmount, KioskStartID, KioskStartID + KioskAmount, SplitSize, outputDirectory, commandLineOptions.format);
            GenerateStreamEvents(StreamEventAmount, AccountStartID, AccountStartID + AccountAmount, SplitSize, outputDirectory, commandLineOptions.format);

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("Total RunTime " + elapsedTime);
        }

        static void GenerateKiosks(int generateCount, int startID, int splitCount, string outputDirectory, FormatOptions format)
        {
            Generators.KioskGenerator kioskgenerator = new Generators.KioskGenerator(generateCount, startID, splitCount);

            kioskgenerator.Generate();

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            switch (format)
            {
                case FormatOptions.csv:
                    kioskgenerator.OutputCsv("Kiosks", outputDirectory);
                    break;
                case FormatOptions.json:
                    kioskgenerator.OutputJson("Kiosks", outputDirectory);
                    break;
            }

            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Completed! {0} Total Kiosks created\n", kioskgenerator.ItemsCreated.ToString("N0", CultureInfo.InvariantCulture));
        }

        static void GenerateRentals(int generateCount, int startID, int accountStartId, int accountEndId, int kioskStartId, int kioskEndId, int splitCount, string outputDirectory, FormatOptions format)
        {
            Generators.RentalGenerator rentalgenerator = new Generators.RentalGenerator(generateCount, startID, accountStartId, accountEndId, kioskStartId, kioskEndId, splitCount);

            rentalgenerator.Generate();

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            switch (format)
            {
                case FormatOptions.csv:
                    rentalgenerator.OutputCsv("Rentals", outputDirectory);
                    break;
                case FormatOptions.json:
                    rentalgenerator.OutputJson("Rentals", outputDirectory);
                    break;
            }

            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Completed! {0} Total Rentals created\n", rentalgenerator.ItemsCreated.ToString("N0", CultureInfo.InvariantCulture));
        }

        static void GenerateStreamEvents(int generateCount, int accountStartId, int accountEndId, int splitCount, string outputDirectory, FormatOptions format)
        {
            Generators.StreamingEventGenerator streamgenerator = new Generators.StreamingEventGenerator(generateCount, accountStartId, accountEndId, splitCount);

            streamgenerator.Generate();

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            switch (format)
            {
                case FormatOptions.csv:
                    streamgenerator.OutputCsv("StreamingEvents", outputDirectory);
                    break;
                case FormatOptions.json:
                    streamgenerator.OutputJson("StreamingEvents", outputDirectory);
                    break;
            }

            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Completed! {0} Total Streaming Events created\n", streamgenerator.ItemsCreated.ToString("N0", CultureInfo.InvariantCulture));
        }

        static void GenerateAccounts(int generateCount, int startID, int splitCount, string outputDirectory, FormatOptions format)
        {
            Generators.UserAccountGenerator useraccountgenerator = new Generators.UserAccountGenerator(generateCount, startID, splitCount);

            useraccountgenerator.Generate();

            TimeSpan ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            switch (format)
            {
                case FormatOptions.csv:
                    useraccountgenerator.OutputCsv("Accounts", outputDirectory);
                    break;
                case FormatOptions.json:
                    useraccountgenerator.OutputJson("Accounts", outputDirectory);
                    break;
            }

            ts = stopWatch.Elapsed;
            // Format and display the TimeSpan value.
            elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            Console.WriteLine("Completed! {0} Total Accounts created\n", useraccountgenerator.ItemsCreated.ToString("N0", CultureInfo.InvariantCulture));
        }

#nullable enable
        static string VerifyDirectory(DirectoryInfo? di)
        {
            if(di != null)
            {
                if (!di.Exists)
                {

                    di.Create();
                }

                if (di.ToString().EndsWith('\\'))
                {
                    return di.ToString();
                }
                else
                {
                    return di.ToString() + "\\";
                }
            } else
            {
                return "";
            }
            
        }
#nullable disable

        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appconfig.json", optional: false, reloadOnChange: true)

            // This allows us to set a system environment variable to Development
            // when running a compiled Release build on a local workstation, so we don't
            // have to alter our real production appsettings file for compiled-local-test.
            //.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)

            .AddEnvironmentVariables()
            //.AddAzureKeyVault()
            .Build();
    }
}
