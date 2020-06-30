using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SynapseDemoDataGenerator.Generators
{
    public class Generator
    {
        public int GenerateCount;

        public static void WriteCSV(string dataType, string csvString, string csvHeader, int filesCreated)
        {
            var csvDir = "C:\\Users\\nilop.NORTHAMERICA\\Downloads\\SynapseDemoData\\";
            var csvPath = string.Format("{0}\\{1}_part-{2}.csv", csvDir, dataType, filesCreated.ToString());
            string csvDump = csvHeader + "\n" + csvString;
            File.WriteAllText(csvPath, csvDump);
        }
    }
}
