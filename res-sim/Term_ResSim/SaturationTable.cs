using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    class SaturationTable
    {
        public double[] Sw { get; private set; }
        public double[] Krw { get; private set; }
        public double[] Kro { get; private set; }
        public double[] Pc { get; private set; }

        double krom; //max oil perm.
        double krwm; //max water perm.
        double swr; //residual water saturation
        double sor; //residual oil saturation
        double swx; //        
        double alpha1;
        double alpha2;
        //Below variables may be better suited as function continuous equation to the end and not as class arrays    
        public SaturationTable(double residualSw, double residualSo, double maxKrw, double maxKro, double xSw, double alpha)
        { //parameters are assumed to stay constant throughout reservoir
            swr = residualSw;
            sor = residualSo;
            krwm = maxKrw;
            krom = maxKro;
            swx = xSw;
            alpha1 = alpha;
            //alpha2 = ?????
            //Setup public
            int range = 100 + 1; //OR: should this be between Swr and 1-Sor?            
            Sw = new double[range];
            Krw = new double[range];
            Kro = new double[range];
            Pc = new double[range];
        }
        public void InitializeTable(double wellSkin, double wellRadius, double pcth)
        { //parameters are assumed to change from well to well...unknown: pcth really is

        }
    }
}
