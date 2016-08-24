using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term_ResSim
{
    class Grid //handles calls between grid locations. Does not do reservoir math.
    {
        public int Width { get; private set; }
        public int Length { get; private set; }
        public int Height { get; private set; }    
        public int Size { get; private set; }        
        //Node info should not be accessed directly        
        public List<Node> NodeList { get; private set; }//Grids that NEVER CHANGE  
        public int[] OrderedIndexing { get; private set; } //This shouldn't be Grid, it should be reservoir
        int[,,] gridMap; //ordering of grid ~ ex: checkerboard, natural, etc. 
        protected int[,,] nodeMap; //indexes of NodeList        
        public Grid(int nWidth, int nLength, int nHeight)
        {
            //Initialize Grid Parameters
            NodeList = new List<Node>();
            Width = nWidth;
            Height = nHeight;
            Length = nLength;
            Size = nWidth * nHeight * nLength;
            gridMap = new int[nWidth,nLength, nHeight];
            nodeMap = new int[nWidth, nLength, nHeight];
            OrderedIndexing = new int[Size];
            //Initialize Nodes
            int count = 0;
            for (byte z = 0; z < Height; z++)
                for (byte y = 0; y < Length; y++)            
                    for (byte x = 0; x < Width; x++)
                    {
                        Node TempNode = new Node(count, count, x, y, z);
                        NodeList.Add(TempNode);
                        nodeMap[x, y, z] = count; //ordering of nodes by index
                        //Use Natural Ordering at initialization
                        OrderedIndexing[count] = count;
                        gridMap[x, y, z] = count; //ordering of grid by pattern
                        count++;
                    }
                        
        }
        public void LoadOrdering(int[,,] order) //Loads a new grid order into the map
        {//need to look at later, might not have correct logic....!!!!                  PROBABLY WRONG
            gridMap = order;
            for (int z = 0; z < Height; z++)
                for (int y = 0; y < Length; y++)
                    for (int x = 0; x < Width; x++)
                    {
                        int n = nodeMap[x, y, z];
                        int o = order[x, y, z]; //if you take the number of the order 
                        NodeList[n].order = o;
                        OrderedIndexing[o] = n;
                    }        
        }
        public int GetAdjacentNode(int ind, int dx, int dy, int dz)
        {
            int node;
            int x = NodeList[ind].x + dx;
            int y = NodeList[ind].y + dy;
            int z = NodeList[ind].z + dz;
            if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Length || z >=Height)
                return -1;
            node = nodeMap[x, y, z]; //Room for improvement by handling this with a function? maybe not speed but functionality
            return node;
        }
        public int GetAdjacentNode(int xLoc, int yLoc, int zLoc, int dx, int dy, int dz)
        {
            int node;
            int x = xLoc + dx;
            int y = yLoc + dy;
            int z = zLoc + dz;
            if (x < 0 || y < 0 || z < 0 || x >= Width || y >= Length || z >= Height)
                return -1;
            node = nodeMap[x, y, z];            
            return node;
        }
        public List<int> GetAdjacentNodes(int ind)
        {
            List<int> nodes = new List<int>();
            int x = NodeList[ind].x;
            int y = NodeList[ind].y;
            int n = -1;
            for (int dx = -1; dx < 2; dx++)
                for (int dy = -1; dy < 2; dy++)
                    for (int dz = -1; dz < 2; dz++)
                    {
                        if (dx == 0 && dy == 0 && dz == 0)
                            continue;
                        n = GetAdjacentNode(ind, dx, dy, dz);
                        if (n != -1)
                            nodes.Add(n);
                    }
            return nodes;
        }
        public List<int> GetAdjacentNodes(int x, int y, int z)
        {
            List<int> nodes = new List<int>();
            int ind = nodeMap[x, y, z];
            int n = -1;
            for (int dx = -1; dx < 2; dx++)
                for (int dy = -1; dy < 2; dy++)
                    for (int dz = -1; dz < 2; dz++)
                    { 
                        if (dx == 0 && dy == 0 && dz == 0)
                            continue;
                        n = GetAdjacentNode(ind, dx, dy, dz);
                        if (n != -1)
                            nodes.Add(n);
                    }
            return nodes;
        }
        public List<int> GetFivePoint(int ind)  //Project specific 2d with x and z differentials
        {
            List<int> nodes = new List<int>();
            int x = NodeList[ind].x;
            int y = NodeList[ind].y;
            int z = NodeList[ind].z;
            int n;
            n = GetAdjacentNode(ind, 1, 0, 0);            
            if (n != -1)
                nodes.Add(n);
            n = GetAdjacentNode(ind, -1, 0, 0);
            if (n != -1)
                nodes.Add(n);
            n = GetAdjacentNode(ind, 0, 0, 1);
            if (n != -1)
                nodes.Add(n);
            n = GetAdjacentNode(ind, 0, 0, -1);
            if (n != -1)
                nodes.Add(n);
            return nodes; 
        }
    }
}
