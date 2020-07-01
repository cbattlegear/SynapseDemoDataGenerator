using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using ExtensionMethods;
using CsvHelper;

namespace SynapseDemoDataGenerator.Generators
{
    abstract public class Generator<T>
    {
        public int GenerateCount;
        public int StartId;
        public List<T> items = new List<T>();

        public Generator(int numberToGenerate, int startingId)
        {
            GenerateCount = numberToGenerate;
            StartId = startingId;
        }

        abstract public void Generate();

        public void OutputCsv(string Filename, int SplitAmount )
        {
            // Determine the directory name and create it
            // Currently just taking Item 2 as all our types are three part named currently Probably should make it more generic...
            string directoryName = typeof(T).ToString().Split('.')[2];
            Directory.CreateDirectory(directoryName);
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
            } else
            {
                int fileCount = 1;
                foreach(var splits in items.Split(SplitAmount))
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
        }
    }
}
