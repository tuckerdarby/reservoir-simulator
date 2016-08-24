using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    class Node //stores simple data about grid space
    {
        //Node Variables
        public int index;
        public int order;
        public byte x;
        public byte y;
        public byte z;
        public double dx;
        public double dy;
        public double dz;
        public bool drilled = false;
        //Reservoir Properties
        public double p;
        public double sw;
        public double kx;
        public double ky;
        public double kz;
        //Rock Properties ~ where's the line between What the node should contain and what the reservoir should contain        
        public double phi;        
        public double ev;
        public double ct;
        public double sh; 
        double phio; //original phi
        public double ho { get; private set; } //original height
        public double w;
        public Node(int ind, int ord, byte xLoc, byte yLoc, byte zLoc)
        {
            index = ind;
            order = ord;
            x = xLoc;
            y = yLoc;
            z = zLoc;     
            //Initialize other variables to avoid errors ~ term input conditions
            dx = 200;
            dy = 200;
            dz = 30;            
            p = 4000;
            phi = 0.15;
            ev = 0.0;
            ct = 0.00001;
            sw = 0.25;
            phio = 0.15;
            ho = dz;
            sh = 0;
        } 
        public double V()
        {
            return dx * dy * dz;
        }
        public void SetPorosity(double porosity)
        {
            phi = porosity;
            phio = porosity;
        }
        public void UpdatePorosity()
        {
            phi = 1 - (1 - phio) * Math.Exp(ev);
        }
        public void UpdateStrain(double h)
        {
            ev = 1 - h/ho;
        }
    }
}
