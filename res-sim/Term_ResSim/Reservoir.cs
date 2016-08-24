using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace Term_ResSim
{
    class Reservoir : Grid //handles math between nodes, coordination through grid
    {
        List<Well> WellList = new List<Well>();
        Breakdown formationBreakdown;
        bool[] initialized = { false, false, false, false };
        double reservoirTime = 0;        
        //Rock properties class needed?
        double cE;
        double cV;
        double cG;
        double cL;
        double cAlpha;
        double fluidVisc; //Fluids properties class needed? ~ not for this project...
        public double[,] wlayer { get; private set; } //only set for 2D right now...
        bool fixedPorosity = false;
        bool fixedHeight = false;
        bool breakdown = false;
        public Reservoir(int nWidth,int nLength, int nHeight) : base(nWidth, nLength, nHeight)
        {
            wlayer = new double[nWidth*nLength,nHeight + 1];
        }
        public void InitializeSimulator(bool fixPorosity, bool fixHeight)
        {
            fixedPorosity = fixPorosity;
            fixedHeight = fixHeight;
            initialized[0] = true;
        }
        public void InitializeFluid(double p, double sw, double visc)
        {
            for (int n = 0; n < Size; n++)
            {
                NodeList[n].p = p;
                NodeList[n].sw = sw;
            }
            fluidVisc = visc;
            initialized[1] = true;
        }
        public void InitializeRock(double[] d, double[] k, double phi, double ct, double E, double v, double alpha)
        {
            for (int n = 0; n < Size; n++)
            {
                NodeList[n].dx = d[0];
                NodeList[n].dy = d[1];
                NodeList[n].dz = d[2];
                NodeList[n].kx = k[0];
                NodeList[n].ky = k[1];
                NodeList[n].kz = k[2];
                NodeList[n].phi = phi;
                NodeList[n].ct = ct;
            }
            for (int w = 0; w < Height + 1; w++)
                for (int i = 0; i < Length*Width; i++)
                        wlayer[i,w] = w * d[2];
            cAlpha = alpha;
            cE = E;
            cV = v;
            cG = E / (2 * (1 + v));
            cL = (E * v) / (1 + v) / (1 - 2 * v);
            initialized[2] = true;
        }
        public void InitializeWell(byte xLoc, byte yLoc, byte zLoc, double flowRate, double skin, double rw, double pwell, double start, double end)
        {
            Well newWell = new Well(xLoc, yLoc, zLoc, start, end);
            newWell.q = flowRate;
            newWell.skin = skin;
            newWell.rwb = rw;
            newWell.bhpMin = pwell;
            WellList.Add(newWell);    
            initialized[3] = true;
        }
        public void TimeStep(double dt)
        {
            List<Node> TempList;
            if (!(initialized[0] && initialized[1] && initialized[2] && initialized[3]))
                return; //user missed initialization ~ this should really be part of a larger construct
            else
                TempList = new List<Node>();
            reservoirTime += dt;
            //Variable Steps//
            Matrix<double> aMatrix = Matrix<double>.Build.Sparse(Size, Size);
            Matrix<double> rMatrix = Matrix<double>.Build.Sparse(Size, 1);
            Matrix<double> pMatrix = Matrix<double>.Build.Sparse(Size, 1);
            List<int> AdjNodes;
            int node;
            double matrixVal = 0;
            double eVal = 0;
            double vcphi = 0;
            double kx;
            double ky;
            double kz;
            double ndx;
            double ndy;
            double ndz;
            int gdx;
            int gdy;
            int gdz;
            int d;
            //Node Step
            for (int n = 0; n < Size; n++)
            {
                node = OrderedIndexing[n];
                AdjNodes = GetFivePoint(node); //(diagonals C,D,F,G)  
                eVal = 0;
                vcphi = NodeList[node].V() * NodeList[node].phi * ((1 - NodeList[node].ev) * NodeList[node].ct + (cAlpha / (cL + 2 * cG)));
                foreach (int adj in AdjNodes)
                {
                    ndx = NodeList[adj].dx;
                    ndy = NodeList[adj].dy;
                    ndz = NodeList[adj].dz;
                    gdx = Math.Max(NodeList[node].x, NodeList[adj].x) - Math.Min(NodeList[node].x, NodeList[adj].x);
                    gdy = 0;// Math.Max(NodeList[node].y, NodeList[n].y) - Math.Min(NodeList[node].y, NodeList[n].y);
                    gdz = Math.Max(NodeList[node].z, NodeList[adj].z) - Math.Min(NodeList[node].z, NodeList[adj].z);
                    d = gdx + gdy + Width * gdz - 1; //NOT PROPERLY SET FOR 3D
                    //apply adjacent node to matrix row (diagonals C,D,F,G)  //only works with 5 point.      !!!                   
                    if (gdx != 0)
                    {
                        kx = NodeList[adj].kx;
                        matrixVal = 0.006328 * kx * ndy * ndz / ndx;
                    }
                    else if (gdy != 0)
                    {
                        ky = NodeList[adj].ky;
                        matrixVal = 0.006328 * ky * ndx * ndz / ndy;
                    }
                    else if (gdz != 0)
                    {
                        kz = NodeList[adj].kz;
                        matrixVal = 0.006328 * kz * ndx * ndy / ndz;
                    }
                    eVal += matrixVal;
                    aMatrix[n, adj] = matrixVal; // n/node

                }
                //Apply current node to matrix row (diagonal E)
                matrixVal = -eVal - vcphi / dt;
                aMatrix[n, n] = matrixVal;
                //rMatrix
                matrixVal = -NodeList[node].p * vcphi / dt - GetWellFlowrate(node);
                rMatrix[n, 0] = matrixVal; // n or node???                     
            }
            aMatrix = aMatrix.Inverse();
            pMatrix = aMatrix * rMatrix;
            int ind;// = nodeMap[i, 0, k];
            double[,] wOld = new double[Width, Height+1];
            for (int k = 0; k <= Height; k++)
                for (int i = 0; i < Width; i++)
                    wOld[i, k] = wlayer[i, k];
            if (!fixedHeight)
                for (int k = 1; k <= Height; k++)
                    for (int i = 0; i < Width; i++)
                    {
                        ind = nodeMap[i, 0, k-1];
                        wlayer[i, Height-k] = wlayer[i, Height-k+1] - ( wOld[i, Height-k+1]-wOld[i,Height-k]) + (cAlpha / (cL + 2 * cG)) * (NodeList[ind].p - pMatrix[ind, 0]) * NodeList[ind].dz;
                        NodeList[ind].UpdateStrain(wlayer[i,Height-k+1]-wlayer[i,Height-k]);
                        if (!breakdown && CheckBreakdown(ind))
                            RecordBreakdown(ind);
                        if (!fixedPorosity)
                            NodeList[k].UpdatePorosity();
                    }
            for (int n = 0; n < Size; n++)
            {
                NodeList[n].p = pMatrix[n, 0];                                
            }
        }
        bool CheckBreakdown(int n)
        {
            double ev = NodeList[n].ev;
            double sigmav = (cL + 2 * cG) * (ev);
            double sigmah = cV / (1 - cV) * sigmav;
            double r = Math.Abs((sigmav - sigmah) / 2);
            double xc = Math.Abs((sigmav + sigmah) / 2);
            double yaxis = 600; //ENTER THIS IN
            double slope = 0.6 ; //ENTER THIS IN
            NodeList[n].sh = sigmah;
            for (int i = 0; i < 91; i++)
            {
                double x = xc - r * Math.Cos(Math.PI * i / 180);
                double y = r * Math.Sin(Math.PI * i / 180);
                if (yaxis + slope * x < y)
                    return true;
            }
            return false;
        }
        void RecordBreakdown(int n)
        {
            breakdown = true;
            formationBreakdown = new Breakdown(NodeList[n], reservoirTime);
        }
        double GetWellFlowrate(int node)
        {
            foreach (Well well in WellList)
                if (well.x == NodeList[node].x && well.y == NodeList[node].y && well.z == NodeList[node].z)
                {
                    well.EstablishWell(NodeList[node].kx, NodeList[node].ky, NodeList[node].dy*NodeList[node].dz/(NodeList[node].dx/2));
                    return well.GetFlowrate(NodeList[node].p, reservoirTime);
                }
            return 0;
        }
        public void ClearWells()
        {
            WellList.Clear();
        }
    }
}
