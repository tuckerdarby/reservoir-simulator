using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    public class Simulator
    {
        public double days = 0;
        public int steps = 0;
        Reservoir res;
        public int nodes { get; private set; }
        double[,] pressureLog;
        double[,] heightLog;
        double[,] strainLog;
        double[,] porosityLog;
        double[,] stressLog;
        public bool running { get; private set; }
        public Simulator()
        {
            running = false;
             //Project Specific
            //res.InitializeFluid(4000, 0.25, 1);
            //res.InitializeSimulator(false, false);
            //res.InitializeRock(d, k, 0.08, (0.00001), 1500000, 0.25, 1);
            //res.InitializeWell(5, 0, 1, -100 * 5.615, 0, 0, 500);                       
            //res.InitializeWell(5, 0, 1, 100 * 5.615, 0, 0, 500);
        }
        public void TimeStep(double dt)
        {
            running = true;
            days += dt;
            steps += 1;
            Console.WriteLine("  ");
            Console.WriteLine("Time: " + days.ToString("0.0") + " days");
            res.TimeStep(dt);
            for (int n = 0; n < nodes; n++)
            {
                pressureLog[n, steps] = res.NodeList[n].p;
                heightLog[n, steps] = res.wlayer[res.NodeList[n].x, res.NodeList[n].z];
                strainLog[n, steps] = res.NodeList[n].ev;
                porosityLog[n, steps] = res.NodeList[n].phi;
                stressLog[n, steps] = res.NodeList[n].sh;
            }
            running = false;
        }
        public double[,] GetPressureLog()
        {
            return pressureLog;
        }
        public double[,] GetHeightLog()
        {
            return heightLog;
        }
        public double[,] GetStrainLog()
        {
            return strainLog;
        }
        public double[,] GetStressLog()
        {
            return stressLog;
        }
        public double[,] GetPorosityLog()
        {
            return porosityLog;
        }
        public void InitializeSimulator(int width, int length, int height, int duration)
        {
            res = new Reservoir(width, length, height);
            nodes = width * length * height;
            heightLog = new double[nodes, duration + 1];
            pressureLog = new double[nodes, duration + 1];
            strainLog = new double[nodes, duration + 1];
            porosityLog = new double[nodes, duration + 1];
            stressLog = new double[nodes, duration + 1];
            for (int n = 0; n < nodes; n++)
            {
                heightLog[n, 0] = height;
                strainLog[n, 0] = 0;
            }
        }
        public void SetFluidProperties(double p, double sat, double visc)
        {
            for (int n = 0; n < nodes; n++)
                pressureLog[n, 0] = p;
            res.InitializeFluid(p, sat, visc);
        }
        public void SetSimulatorSettings(bool fixPor, bool fixHeight)
        {
            res.InitializeSimulator(fixPor, fixHeight);
        }
        public void SetRockProperties(double[] d, double[] k, double phi, double ct, double E, double v, double alpha)
        {
            res.InitializeRock(d, k, phi, ct, E, v, alpha);
            for (int n = 0; n < nodes; n++)
                porosityLog[n, 0] = phi;
        }
        public void SetWellInputs(byte x, byte y, byte z, double flowrate, double skin, double rwb, double pwell, double start, double end)
        {
            res.InitializeWell(x, y, z, flowrate*5.615, skin, rwb, pwell, start, end);
        }
        public void ClearWells()
        {
            res.ClearWells();
        }
    }
}