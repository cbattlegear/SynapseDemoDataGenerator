using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using ExtensionMethods;
using CsvHelper;
using ProtoBuf;

namespace SynapseDemoDataGenerator.Generators
{
    abstract public class Generator<T>
    {
        public int GenerateCount;
        public int StartId;
        public int SplitAmount;
        public List<T> items = new List<T>();
        public int ItemsCreated = 0;

        public Generator(int numberToGenerate, int startingId, int splitCount)
        {
            GenerateCount = numberToGenerate;
            StartId = startingId;
            SplitAmount = splitCount;
        }

        abstract public void Generate();

        public void OutputCsv(string Filename)
        {
            // Determine the directory name and create it
            // Currently just taking Item 2 as all our types are three part named currently Probably should make it more generic...
            string directoryName = typeof(T).ToString().Split('.')[2];
            Directory.CreateDirectory(directoryName);
            Console.WriteLine("Beginning to write {0} CSV files.", directoryName);
            // Dealing with large generation memory issues
            if (GenerateCount < Convert.ToInt32(Program.Configuration["UseDiskThreshold"]))
            {
                // If the split amount is zero (or less just to deal with negatives) don't split
                if (SplitAmount <= 0)
                {
                    using (var writer = new StreamWriter(directoryName + "\\" + Filename + ".csv"))
                    {
                        using (var csvout = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csvout.WriteRecords(items);
                        }
                    }
                }
                else
                {
                    int fileCount = 1;
                    foreach (var splits in items.Split(SplitAmount))
                    {
                        using (var writer = new StreamWriter(directoryName + "\\" + Filename + fileCount.ToString() + ".csv"))
                        {
                            using (var csvout = new CsvWriter(writer, CultureInfo.InvariantCulture))
                            {
                                csvout.WriteRecords(splits);
                            }
                        }
                        fileCount++;
                    }
                }
            } else
            // For large generations we will read in the files, once complete delete the cache folder
            {
                // If the split amount is zero (or less just to deal with negatives) don't split (append to the file after reading our binary)
                if (SplitAmount <= 0)
                {
                    bool firstFile = true;
                    string[] cacheFiles = Directory.GetFiles("CacheData\\");
                    foreach (string file in cacheFiles)
                    {
                        using (Stream cache = File.Open(file, FileMode.Open))
                        {
                            items = Serializer.DeserializeItems<T>(cache, PrefixStyle.Base128, 1).ToList();
                            if (firstFile)
                            {
                                using (var writer = new StreamWriter(directoryName + "\\" + Filename + ".csv"))
                                {
                                    using (var csvout = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                    {
                                        csvout.WriteRecords(items);
                                    }
                                }
                                firstFile = false;
                            }
                            else
                            {
                                using (var stream = File.Open(directoryName + "\\" + Filename + ".csv", FileMode.Append))
                                using (var writer = new StreamWriter(stream))
                                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                {
                                    // Don't write the header again.
                                    csv.Configuration.HasHeaderRecord = false;
                                    csv.WriteRecords(items);
                                }
                            }
                        }
                    }
                    
                }
                else
                {
                    int fileCount = 1;
                    string[] cacheFiles = Directory.GetFiles("CacheData\\");
                    foreach (string file in cacheFiles)
                    {
                        using (Stream cache = File.Open(file, FileMode.Open))
                        {
                            items = Serializer.DeserializeItems<T>(cache, PrefixStyle.Base128, 1).ToList();

                            using (var writer = new StreamWriter(directoryName + "\\" + Filename + fileCount.ToString() + ".csv"))
                            {
                                using (var csvout = new CsvWriter(writer, CultureInfo.InvariantCulture))
                                {
                                    csvout.WriteRecords(items);
                                }
                            }
                            fileCount++;
                           
                        }
                    }
                }
                // Wipe our cache directory after writing our data
                Directory.Delete("CacheData\\", true);
            }
            Console.WriteLine("Finished writing {0} CSV files.", directoryName);
        }
    }
}
