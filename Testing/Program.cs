using System;
using RWDargatzUtility.MRFParser;
using RWDargatzUtility.MRFParser.ValueTypes;
using RWDargatzUtility.Testing.Utility;

namespace RWDargatzUtility.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new FileParser(FileRetrieval.getDataFile("TestData.mrf"), "File");
            var rand = new Random();


            Console.WriteLine(parser.FindRequiredString("FileUniqueId"));
            Console.WriteLine(parser.FindRequiredInt("RangeTest").Dispense(rand));
            Console.WriteLine(parser.FindRequiredInt("SingleTest").Dispense(rand));

            var seedArray = parser.FindSection("FOOD").FindRequiredIntArray("Seeds");
            foreach (MRFInteger i in seedArray) Console.WriteLine(i.Dispense(rand));

            DesignTesting(parser);



            Console.ReadLine();
        }

        public static void DesignTesting(FileParser parser)
        {
            
        }

        public static void TestParsingNumbers()
        {

            Console.WriteLine();
            

            for (int i = 0; i < 10; i++)
            {
                //Console.WriteLine(variableInteger.Dispense(rand));
            }
        }
    }
}
