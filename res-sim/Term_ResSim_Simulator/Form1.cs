using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Term_ResSim;
using System.IO;

namespace Term_ResSim_Simulator
{
    public partial class SimulatorForm : Form
    {
        SimulatorConsole simConsole;// = new SimulatorConsole();
        int gridWidth = 11;
        int gridHeight = 3;
        int gridLength = 1;
        double[,] plog;// = simConsole.GetPressureLog();
        double maxP;
        double minP;
        double[,] hlog;// = simConsole.GetHeightLog();
        double maxH;
        double minH;
        double[,] philog;// = simConsole.GetPorosityLog();
        double maxPhi;
        double minPhi;
        double[,] strainlog;// = simConsole.GetStrainLog();
        double maxStrain;
        double minStrain;
        double[,] stresslog;
        double maxStress;
        double minStress;
        public double[] storedvals;
        public SimulatorForm()
        {
            simConsole = new SimulatorConsole();
            InitializeComponent();            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }

        private void simulateButton_Click(object sender, EventArgs e)
        {

        }

        private void addWellButton_Click(object sender, EventArgs e)
        {

        }

        private void clearWellsButton_Click(object sender, EventArgs e)
        {
            simConsole.ClearWells();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gridPanel_Paint(object sender, PaintEventArgs e)
        {

        }
        public void UpdatePressureChart(double[,] p, int nodes, int steps, double dt)
        {
            string name;
            double day = 0;
            pressureChart.Series.Clear();
            for (int n = 1; n <= nodes; n++)
            {
                name = n.ToString("0");
                day = 0;
                pressureChart.Series.Add(name);
                pressureChart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                for (int t = 0; t < steps; t++)
                {
                    pressureChart.Series[name].Points.AddXY(day, p[n - 1, t]);
                    day += dt;
                }
                pressureChart.Series[name].ChartArea = "ChartArea1";
            }

        }
        public void UpdateHeightChart(double[,] h, int nodes, int steps, double dt)
        {
            string name;
            double day = 0;
            heightChart.Series.Clear();
            for (int n = 1; n <= nodes; n++)
            {
                name = n.ToString("0");
                day = 0;
                heightChart.Series.Add(name);
                heightChart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                for (int t = 0; t < steps; t++)
                {
                    heightChart.Series[name].Points.AddXY(day, h[n - 1, t]);
                    day += dt;
                }
                heightChart.Series[name].ChartArea = "ChartArea1";
            }
        }
        public void UpdateStrainChart(double[,] s, int nodes, int steps, double dt)
        {
            string name;
            double day = 0;
            strainChart.Series.Clear();
            for (int n = 1; n <= nodes; n++)
            {
                name = n.ToString("0");
                day = 0;
                strainChart.Series.Add(name);
                strainChart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.FastPoint;
                for (int t = 0; t < steps; t++)
                {
                    strainChart.Series[name].Points.AddXY(day, s[n - 1, t]);
                    day += dt;
                }
                strainChart.Series[name].ChartArea = "ChartArea1";
            }

        }

        private void regridButton_Click(object sender, EventArgs e)
        {
            gridWidth = Convert.ToInt16(widthBox.Text);
            gridLength = Convert.ToInt16(lengthBox.Text);
            gridHeight = Convert.ToInt16(heightBox.Text);
            gridPanel.ColumnCount = gridWidth;
            gridPanel.RowCount = gridHeight;            
            //gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize, 1));
            //gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize, 1));


        }

        private void simulateButton_Click_1(object sender, EventArgs e)
        {
            double dt = Convert.ToDouble(stepBox.Text);
            double duration = Convert.ToDouble(durationBox.Text);
            int steps = (int)(duration / dt);
            simConsole.InitializeConsole(gridWidth, gridLength, gridHeight, steps);// = new SimulatorConsole();
            double[] d = { Convert.ToDouble(dxBox.Text), Convert.ToDouble(dyBox.Text), Convert.ToDouble(dzBox.Text) };
            double[] k = { Convert.ToDouble(kxBox.Text), Convert.ToDouble(kyBox.Text), Convert.ToDouble(kzBox.Text) };
            double phi = Convert.ToDouble(porosityText.Text);
            double ct = Convert.ToDouble(ctBox.Text);
            double E = Convert.ToDouble(eBox.Text);
            double v = Convert.ToDouble(vBox.Text);
            double alpha = Convert.ToDouble(alphaBox.Text);
            simConsole.SetRockProperties(d, k, phi, ct, E, v, alpha);
            double p = Convert.ToDouble(pressureBox.Text);
            double sat = Convert.ToDouble(saturationBox.Text);
            double visc = Convert.ToDouble(viscosityBox.Text);
            simConsole.SetFluidProperties(p, sat, visc);
            simConsole.SetSimulatorSettings(porosityCheck.Checked, heightCheck.Checked);
            simConsole.RunSimulator(duration, dt);
            plog = simConsole.GetPressureLog();
            hlog = simConsole.GetHeightLog();
            philog = simConsole.GetPorosityLog();
            strainlog = simConsole.GetStrainLog();
            stresslog = simConsole.GetStressLog();
            int nodes = simConsole.GetNodes();
            storedvals = new double[nodes];
            UpdatePressureChart(plog, nodes, steps, dt);
            UpdateHeightChart(hlog, nodes, steps, dt);
            UpdateStrainChart(strainlog, nodes, steps, dt);
            dataTrack.Minimum = 0;
            dataTrack.Maximum = steps;
            maxH = 0;
            minH = 99999;
            maxP = 0;
            minP = 99999;
            maxPhi = 0;
            minPhi = 1;
            maxStrain = 0;
            minStrain = 99999;
            maxStress = 0;
            minStress = 0;
            try {
                StreamWriter outfileM = new StreamWriter(@"Z:\Workspace\ResSimTerm2.txt", false, Encoding.UTF8);
                for (int t = 0; t < steps; t++)
                {
                    string mat = "";
                    for (int n = 0; n < nodes; n++)
                    {
                        mat += strainlog[n, t].ToString("0.000000000") + ",";
                        maxH = Math.Max(maxH, hlog[n, t]);
                        maxP = Math.Max(maxP, plog[n, t]);
                        maxPhi = Math.Max(maxPhi, philog[n, t]);
                        maxStrain = Math.Max(maxStrain, strainlog[n, t]);
                        minH = Math.Min(minH, hlog[n, t]);
                        minP = Math.Min(minP, plog[n, t]);
                        minPhi = Math.Min(minPhi, philog[n, t]);
                        minStrain = Math.Min(minStrain, strainlog[n, t]);
                        maxStress = Math.Max(maxStress, stresslog[n, t]);
                        minStress = Math.Min(minStress, stresslog[n, t]);
                    }
                    outfileM.WriteLine(mat);
                }
                outfileM.Close();
            }
            catch { }
             //testing purposes only
            // Set the tick frequency to one tick every ten units.
            dataTrack.TickFrequency = 1;

            // Associate the event-handling method with the 
            // ValueChanged event.
            dataTrack.ValueChanged +=
                new System.EventHandler(dataTrack_ValueChanged);
            //this.Controls.Add(this.dataTrack);
        }

        private void addWellButton_Click_1(object sender, EventArgs e)
        {

            byte x = Convert.ToByte(xWellBox.Text);
            byte y = Convert.ToByte(yWellBox.Text);
            byte z = Convert.ToByte(zWellBox.Text);
            double flow = Convert.ToDouble(flowrateBox.Text);
            double skin = Convert.ToDouble(skinBox.Text);
            double rwb = Convert.ToDouble(rwbBox.Text);
            double bhpMin = Convert.ToDouble(bhpBox.Text);
            double start = Convert.ToDouble(startBox.Text);
            double end = Convert.ToDouble(endBox.Text);            
            simConsole.addWell(x, y, z, flow, skin, rwb, bhpMin, start, end);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            wellView.SelectAll();
            wellView.ClearSelection();
            simConsole.ClearWells();
        }

        private void label23_Click(object sender, EventArgs e)
        {

        }

        private void viewButton_Click(object sender, EventArgs e)
        {

        }
        public void UpdateRealTimeChart(double[] vals, int nodes, double max, double min)
        {
            string name = "nodes";
            rtChart.Series.Clear();
            rtChart.Series.Add(name);
            rtChart.Series[name].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            for (int n = 0; n < nodes; n++)
            {
                rtChart.Series[name].Points.AddY(vals[n]);
            }
            rtChart.Series[name].ChartArea = "ChartArea1";
            rtChart.ChartAreas["ChartArea1"].AxisY.Maximum = max;
            rtChart.ChartAreas["ChartArea1"].AxisY.Minimum = min;
        }

        private void stepNumeric_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataTrack_Scroll(object sender, EventArgs e)
        {

        }
        private void dataTrack_ValueChanged(object sender, System.EventArgs e)
        {
            timeText.Text = (System.Math.Round(dataTrack.Value / 10.0)).ToString();
            int step = dataTrack.Value;
            int nodes = simConsole.GetNodes();
            string view = dataCombo.Text;
            double min = 0;
            double max = 0;
            double[] vals = new double[nodes];
            if (view == "Pressure")
            {
                min = 0;
                max = (int)maxP + 500;
                for (int n = 0; n < nodes; n++)
                    vals[n] = plog[n, step];
            }
            else if (view == "Strain")
            {
                min = minStrain;
                max = maxStrain;
                for (int n = 0; n < nodes; n++)
                    vals[n] = strainlog[n, step];
            }
            else if (view == "Height")
            {
                min = 0;
                max = maxH;
                for (int n = 0; n < nodes; n++)
                    vals[n] = hlog[n, step];
            }
            else if (view == "Porosity")
            {
                min = 0;
                max = 1;
                for (int n = 0; n < nodes; n++)
                    vals[n] = philog[n, step];
            }
            else if (view == "Stress")
            {
                min = minStress;
                max = maxStress;
                for (int n = 0; n < nodes; n++)
                    vals[n] = stresslog[n, step];
            }
            for (int n = 0; n < nodes; n++)
                storedvals[n] = vals[n];
            UpdateRealTimeChart(vals, nodes, max, min);
        }

        private void resetSimulationButton_Click(object sender, EventArgs e)
        {
            simConsole = new SimulatorConsole();
        }

        private void extractButton_Click(object sender, EventArgs e)
        {
            try
            {
                StreamWriter outfileM = new StreamWriter(@"Z:\Workspace\ResSim_Extraction.txt", false, Encoding.UTF8);
                string mat = "";
                for (int n = 0; n < simConsole.GetNodes(); n++)
                {
                    mat += storedvals[n].ToString("0.000000000") + ",";                        
                }
                outfileM.WriteLine(mat);
                
                outfileM.Close();
            }
            catch { }
        }
    }
}
