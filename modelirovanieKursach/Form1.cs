using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;

namespace modelirovanieKursach
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            tabControl1.TabPages[0].Text = "значения";
            tabControl1.TabPages[1].Text = "графики";

            #region рисовалка графиков
            PointPairList list = new PointPairList();


            double xmin = -1;
            double xmax = 1;

            // Заполняем список точек
            for (double x = xmin; x <= xmax; x += 0.01)
            {
                // добавим в список точку
                list.Add(x, 4 * Math.Pow(x, 2));
            }

            Drawing drawing = new Drawing();
            ///drawing.DrawGraph(zedGraphControl1, list);
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            var random = new Random();
            var sample = new List<double>();

            for (int i = 0; i < Convert.ToInt32(numericUpDown2.Value); i++)
            {
                double experience = Math.Pow(random.NextDouble(), 2)/4;
                sample.Add(experience);
            }
            foreach (var d in sample)
            {
                textBox1.Text += d + "\r\n";
            }
            //double h = 0;
            //foreach (var d in sample)
            //{
            //    if (d<0.001)
            //    {
            //        h++;
            //    }
            //}
            //textBox1.Text += h;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            var random = new Random();
            var sample = new List<double>();
            var borderInterval = new List<double>();
            double plochad = 0;
            double chanceInterval = 0;
            double dx = 0;
            double x = 0;

            try
            {
                dx = Convert.ToDouble(textBox2.Text);
            }
            catch (Exception)
            {
                MessageBox.Show("точность введите через запятую");
                return;
            }

            plochad = func.integral(0, 9, dx);
            chanceInterval = plochad/Convert.ToDouble(numericUpDown1.Value);

            double S = 0;

            /////////////////////////////////////
            /// последовательный поиск границ
            /// ////////////////

            if (radioButton1.Checked)
            {
                dx *= 0.01;
                while (x<=9)
                {
                    S += func.function(x)*dx;
                    if (S >= chanceInterval)
                    {
                        borderInterval.Add(x);
                        S = 0;
                    }
                    x += dx;
                }
                
            }

            /////////////////////////////////////
            /// бинарный поиск границ
            /// ////////////////

            if (radioButton2.Checked)
            {
                S = 0;
                double xpen = Math.Abs(chanceInterval - S);
                double x0 = 0;
                double x1 = 9;
                double beginInterval = 0;
                for (int i = 0; i < Convert.ToInt32(numericUpDown1.Value) - 1; i++)
                {
                    S = func.integral(beginInterval, 9, dx);
                    xpen = Math.Abs(chanceInterval - S);
                    while (xpen > dx)
                    {
                        S = func.integral(beginInterval, (x1 + x0)/2, dx);
                        if (S > chanceInterval)
                        {
                            x1 = (x1 + x0)/2;
                        }
                        else
                        {
                            x0 = (x1 + x0)/2;
                        }
                        xpen = Math.Abs(chanceInterval - S);
                    }
                    //textBox1.Text += (x1 + x0) / 2 + "\r\n";
                    borderInterval.Add((x1 + x0) / 2);
                    S = 0;
                    x0 = (x1 + x0) / 2;
                    x1 = 9;
                    beginInterval = x0;
                }
            }

            /////////////////////////////////////
            /// случайный поиск границ
            /// ////////////////

            if (radioButton3.Checked)
            {
                S = 0;
                double randomPoint;
                double xpen = Math.Abs(chanceInterval - S);
                double x0 = 0;
                double x1 = 9;
                double beginInterval = 0;
                for (int i = 0; i < Convert.ToInt32(numericUpDown1.Value) - 1; i++)
                {
                    randomPoint = random.NextDouble()*(x1 - x0) + x0;
                    S = func.integral(beginInterval, 9, dx);
                    xpen = Math.Abs(chanceInterval - S);
                    while (xpen > dx)
                    {
                        S = func.integral(beginInterval, randomPoint, dx);
                        if (S > chanceInterval)
                        {
                            x1 = randomPoint;
                        }
                        else
                        {
                            x0 = randomPoint;
                        }
                        randomPoint = random.NextDouble() * (x1 - x0) + x0;
                        xpen = Math.Abs(chanceInterval - S);
                    }
                    //textBox1.Text += (x1 + x0) / 2 + "\r\n";
                    borderInterval.Add(randomPoint);
                    S = 0;
                    x0 = randomPoint;
                    x1 = 9;
                    beginInterval = x0;
                }
            }

            borderInterval.Add(9);

            for (int i = 0; i < Convert.ToInt32(numericUpDown2.Value); i++)
            {
                int r = random.Next(Convert.ToInt32(numericUpDown1.Value) - 1);
                sample.Add(func.function((borderInterval[r + 1] - borderInterval[r])
                * random.NextDouble() + borderInterval[r]));
            }
            
            foreach (var d in sample)
            {
                textBox1.Text += d + "\r\n";
            }
        }
    }
}
