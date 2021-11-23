using System;
using MRFParser;
using MRFParser.ValueTypes;
using Testing.Utility;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var parser = new FileParser(FileRetrieval.getDataFile("TestData.txt"), "File");
            var rand = new Random();


            Console.WriteLine(parser.FindRequiredString("FileUniqueId"));
            Console.WriteLine(parser.FindRequiredInt("RangeTest").Dispense(rand));
            Console.WriteLine(parser.FindRequiredInt("SingleTest").Dispense(rand));

            DesignTesting(parser);



            Console.ReadLine();
        }

        public static void DesignTesting(FileParser parser)
        {
            var tester = new TestDesign(parser);
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
