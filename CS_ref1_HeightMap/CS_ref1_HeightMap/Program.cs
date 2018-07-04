using System;
using System.IO;
namespace CS_ref1_HeightMap
{
    class Program
    {
        static void Main(string[] args)
        {
            //if args != 0    CS_ref1_HeightMap.exe input(.hgt) output(.bmp) ElevationTreshold(int)
            try
            {
                //check if args are given
                if (args.Length == 0)
                {

                    Console.WriteLine("Input file (.hgt): ");
                    string input = Console.ReadLine();
                    Console.WriteLine("Elevation Threshold (integer): ");
                    int threshold = int.Parse(Console.ReadLine());
                    Console.WriteLine("Output file (.bmp): ");
                    string output = Console.ReadLine();

                    HeightMap hm = HeightMap.Parse(input);
                    hm.ElevationThreshold = threshold;
                    hm.SaveToBitmap(output);

                }
                else if (args.Length != 0)
                {
                    HeightMap hm = HeightMap.Parse(args[0]);
                    hm.ElevationThreshold = int.Parse(args[2]);
                    hm.SaveToBitmap(args[1]);

                }
                else
                {
                    Console.WriteLine("Error!\n");
                    Console.ReadLine();
                }
            }
            catch
            {
                //invalid file name(s)
                FileNotFoundException ex = new FileNotFoundException();
                Console.WriteLine(ex.ToString());
                Console.ReadLine();
            }

        }
    }
}
