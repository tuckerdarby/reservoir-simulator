using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    class Well
    {
        public byte x;
        public byte y;
        public byte z;
        public double bhpMin;
        public double rwb; //wellbore radius
        public double q;//flowrate
        public double skin;
        double WI;
        double startTime = 0;
        double endTime = 0;
        //add more

        public Well(byte xLoc, byte yLoc, byte zLoc, double start, double end)  
        {//Paramaters should set necessary things to avoid errors
            x = xLoc;
            y = yLoc;
            z = zLoc;
            //Initialize other variables
            rwb = 0.5;
            skin = 0;
            q = 0;
            bhpMin = 500;
            startTime = start;
            endTime = end;
        }
        public void EstablishWell(double kx, double ky, double spacialChange)
        {
            WI = 0.006328 * Math.Sqrt(kx * ky) * spacialChange;
        }
        public double GetFlowrate(double Pnode, double time)
        {
            double Pwell = q / WI + Pnode;
            if (time >= startTime && time <= endTime)
                if (Pwell >= bhpMin)
                    return q;
                else
                    return -WI * (Pnode - bhpMin);
            else
                return 0;        
        }

    }

    class Breakdown
    {
        public Node node;
        public double time;
        public Breakdown(Node n, double t)
        {
            node = n;
            time = t;
        }
    }
}
