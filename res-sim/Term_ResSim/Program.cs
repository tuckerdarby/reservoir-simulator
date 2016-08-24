using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    class Program
    {
        static void Main(string[] args)
        {
            Reservoir res = new Reservoir(11,1,3); //Project Specific
            double[] d = { 20, 200, 30 };
            double[] k = { 0.1, 0.1, 0.04 };                      
            res.InitializeFluid(4000, 0.25, 1);
            res.InitializeSimulator(false, false);
            res.InitializeRock(d, k, 0.08, (0.00001), 1500000, 0.25, 1);
            res.InitializeWell(5, 0, 1, -100*5.615, 0, 0, 500, 0, 10);
            double days = 0;
            for (int t = 0; t < 100; t++)
            {
                days += 0.1;
                Console.WriteLine("  ");
                Console.WriteLine("Time: " + days.ToString("0.0") + " days");
                res.TimeStep(0.1);
            }
            res.ClearWells();
            res.InitializeWell(5, 0, 1, 100 * 5.615, 0, 0, 500,0,10);
            for (int t = 0; t < 100; t++)
            {
                days += 0.1;
                Console.WriteLine("  ");
                Console.WriteLine("Time: " + days.ToString("0.0") + " days");
                res.TimeStep(0.1);
            }
            Console.ReadKey();
        }
    }
}
