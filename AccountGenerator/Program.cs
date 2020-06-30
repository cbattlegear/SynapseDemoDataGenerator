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

        static void GenerateAccountsOld(int generateCount)
        {
            var userIds = 100001;

            //Counter for full run and files created
            var totalGenerated = 1;
            var filesCreated = 1;
            var intermediaryCount = 0;

            Console.WriteLine("Generating User Accounts, starting with UserID {0}", userIds);

            //create CSV buiilder and empty string
            var csv = new StringBuilder();
            var newLine = "";
            var csvHeader = string.Format("UserID,FirstName,LastName,Email,PhoneNumber,Address,State,ZipCode,CreditCardNumber,CreditCardExpiration,MemberSince");

            //csv.AppendLine(newLine);

            var faker = new Faker("en");

            var newUser = new Faker<UserAccount>()

                .RuleFor(u => u.UserId, f => userIds++)
                .RuleFor(u => u.FirstName, (f, u) => f.Name.FirstName(f.Person.Gender))
                .RuleFor(u => u.LastName, (f, u) => f.Name.LastName(f.Person.Gender))
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
                .RuleFor(u => u.PhoneNumber, (f, u) => f.Phone.PhoneNumberFormat())
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.State, (f, u) => f.Address.StateAbbr())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")))
                .RuleFor(u => u.CreditCardNumber, (f, u) => f.Finance.CreditCardNumber(CardType.Visa))
                .RuleFor(u => u.CreditCardExpiration, (f, u) => f.Date.Future(4))
                .RuleFor(u => u.MemberSince, (f, u) => f.Date.Past(8))
                .FinishWith((f, u) =>
                {

                    newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}",
                                                 u.UserId.ToString()
                                                 , u.FirstName
                                                 , u.LastName
                                                 , u.Email
                                                 , u.PhoneNumber
                                                 , u.Address
                                                 , u.State
                                                 , u.ZipCode
                                                 , u.CreditCardNumber
                                                 , u.CreditCardExpiration.ToString("yyyy-MM")
                                                 , u.MemberSince.ToString("yyyy-MM-dd"));
                    csv.AppendLine(newLine);

                    intermediaryCount++;

                    //Output Status
                    if (totalGenerated % 100000 == 0)
                    {

                        WriteCSV("Accounts", csv.ToString(), csvHeader, filesCreated);
                        filesCreated++;
                        Console.WriteLine("Latest User Id: {0}, Total Created: {1}", u.UserId, totalGenerated);
                        csv = new StringBuilder();
                        intermediaryCount = 0;
                    }

                    totalGenerated++;

                });

            var users = newUser.Generate(generateCount);

            //Flush remaining values generated if any
            if (intermediaryCount > 0)
            {
                WriteCSV("Accounts", csv.ToString(), csvHeader, filesCreated);
            }

            Console.WriteLine("Completed! {0} Total Accounts created\n", totalGenerated - 1);
        }


        static void GenerateKiosks(int generateCount)
        {
            var kioskIds = 10001;

            //Counter for full run and files created
            var totalGenerated = 1;
            var filesCreated = 1;
            var intermediaryCount = 0;

            Console.WriteLine("Generating Kiosks, starting with kiosk ID {0}", kioskIds);

            //create CSV buiilder and empty string
            var csv = new StringBuilder();
            var newLine = "";
            var csvHeader = string.Format("KioskId,Address,ZipCode,InstallDate");

            //csv.AppendLine(newLine);

            var faker = new Faker("en");

            var newKiosk = new Faker<Kiosk>()

                .RuleFor(u => u.KioskId, f => kioskIds++)
                .RuleFor(u => u.Address, (f, u) => f.Address.StreetAddress())
                .RuleFor(u => u.ZipCode, (f, u) => f.Address.ZipCode(f.Random.Replace("#####")))
                .RuleFor(u => u.InstallDate, (f, u) => f.Date.Past(4))
                .FinishWith((f, u) =>
                {

                    newLine = string.Format("{0},{1},{2},{3}",
                                                 u.KioskId.ToString()
                                                 , u.Address
                                                 , u.ZipCode
                                                 , u.InstallDate.ToString("yyyy-MM-dd"));
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

            var kiosks = newKiosk.Generate(generateCount);

            //Flush remaining values generated if any
            if (intermediaryCount > 0)
            {
                WriteCSV("Kiosks", csv.ToString(), csvHeader, filesCreated);
            }

            Console.WriteLine("Completed! {0} Total Kiosks created\n", totalGenerated - 1);
        }

        static void RentalEvents(int generateCount)
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
        }

        static void StreamingEvents(int generateCount)
        {
            //TODO
            //var Json = ConvertToJson.Dump("OBJECT HERE");
        }




        static void GenerateAccounts(int generateCount)
        {
            Generators.UserAccount useraccounts = new Generators.UserAccount();

            useraccounts.GenerateCount = generateCount;

            Console.WriteLine("Completed! {0} Total Accounts created\n", totalGenerated - 1);
        }

        static void WriteCSV(string dataType, string csvString, string csvHeader, int filesCreated)
        {
            var csvDir = "C:\\Users\\nilop.NORTHAMERICA\\Downloads\\SynapseDemoData\\";
            var csvPath = string.Format("{0}\\{1}_part-{2}.csv", csvDir, dataType, filesCreated.ToString());
            string csvDump = csvHeader + "\n" + csvString;
            File.WriteAllText(csvPath, csvDump);
        }

    }


    public static class ConvertToJson
    {
        public static string Dump(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

    }

 
}
