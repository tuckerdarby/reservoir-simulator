using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term_ResSim;

namespace Term_ResSim_Simulator
{
    class SimulatorConsole
    {
        Simulator ResSim = new Simulator();
        List<Well> WellList = new List<Well>();
        public SimulatorConsole()
        {
     
        }
        public void InitializeConsole(int w, int l, int h, int t)
        {
            ResSim.InitializeSimulator(w, l, h, t);
        }
        public void SetFluidProperties(double p, double sat, double visc)
        {
            ResSim.SetFluidProperties(p, sat, visc);
        }
        public void SetSimulatorSettings(bool fixPor, bool fixHeight)
        {
            ResSim.SetSimulatorSettings(fixPor, fixHeight);
        }
        public void SetRockProperties(double[] d, double[] k, double phi, double ct, double E, double v, double alpha)
        {
            ResSim.SetRockProperties(d, k, phi, ct, E, v, alpha);
        }
        public void RunSimulator(double duration, double dt)
        {
            int steps = (int)(duration / dt);
            foreach (Well well in WellList)
                ResSim.SetWellInputs(well.x, well.y, well.z, well.flowrate, well.skin, well.rwb, well.bhp, well.startTime, well.endTime);
            for (int i = 0; i < steps; i++ )
                ResSim.TimeStep(dt);
        }
        public void addWell(byte x, byte y, byte z, double flow, double skin, double rwb, double bhpMin, double start, double end)
        {
            Well tempWell = new Well(x, y, z, flow, skin, rwb, bhpMin, start, end);
            WellList.Add(tempWell);            
        }
        public void ClearWells()
        {
            ResSim.ClearWells();
            WellList.Clear();        
        }
        public double[,] GetPressureLog()
        {
            return ResSim.GetPressureLog();
        }
        public double[,] GetHeightLog()
        {
            return ResSim.GetHeightLog();
        }
        public double[,] GetStrainLog()
        {
            return ResSim.GetStrainLog();
        }
        public double[,] GetPorosityLog()
        {
            return ResSim.GetPorosityLog();
        }
        public double[,] GetStressLog()
        {
            return ResSim.GetStressLog();
        }
        public int GetNodes()
        {
            return ResSim.nodes;
        }
    }
    class Well
    {
        public int index;
        public byte x;
        public byte y;
        public byte z;
        public double flowrate;
        public double skin;
        public double rwb;
        public double bhp;
        public double startTime;
        public double endTime;       
        public Well(byte xloc, byte yloc, byte zloc, double flow, double s, double rw, double pmin, double start, double end)        
        {
            index = 0;
            x = xloc;
            y = yloc;
            z = zloc;
            flowrate = flow;
            skin = s;
            rwb = rw;
            bhp = pmin;
            startTime = start;
            endTime = end;
        }
    }
}
