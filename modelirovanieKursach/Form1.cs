using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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


            //dataGridView1.Columns.Add("", "");
            for (int i = 0; i < Convert.ToInt32(numericUpDown3.Value); i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 20;
            }
            dataGridView1.Columns[0].Width = 78;
            dataGridView1.Columns[1].Width = 79;
            dataGridView1.Height = dataGridView1.Rows.Count * 20 + 23;
            dataGridView1.Width = 160;
            //dataGridView1.Rows.Add();

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

            Drawing drawing = new Drawing(zedGraphControl1);
            drawing.DrawFunction(zedGraphControl1, list, Color.Blue);
            #endregion
        }

        /*
         метод обратной функции
         */

        private void button1_Click(object sender, EventArgs e)
        {
            /**
             метод обратной функции для непрерывных величин
             */
            if (radioButton4.Checked)
            {
                textBox1.Text = "";
                var random = new Random();
                var sample = new List<double>();
                Mutex mutex = new Mutex();

                Parallel.For(0, Convert.ToInt32(numericUpDown2.Value), i =>
                {
                    double experience = func.functionReverse(random.NextDouble());
                    mutex.WaitOne();
                    sample.Add(experience);
                    mutex.ReleaseMutex();
                }
                );
                for (int i = 0; i < 50; i++)
                {
                    textBox1.Text += sample[i] + "\r\n";
                }
            }


            /**
             метод обратной функции для дискретных величин
             */
            if (radioButton5.Checked)
            {

                List<double> sample = new List<double>();
                double sumP = 0;
                textBox1.Text = "";
                Dictionary<double,double> rowрAllocation = new Dictionary<double, double>();
                
                dataGridView1.SelectAll();
                foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
                {
                    try
                    {
                        rowрAllocation.Add(
                            Convert.ToDouble(dr.Cells[0].Value),
                            Convert.ToDouble(dr.Cells[1].Value)
                        );
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Задается вероятность для одинаковых значений");
                        return;
                    }
                }
                foreach (DataGridViewRow dr in dataGridView1.SelectedRows)
                {
                    sumP += Convert.ToDouble(dr.Cells[1].Value);
                }
                if (Math.Round(sumP, 5) != 1)
                {
                    MessageBox.Show(
                        "Сумма вероятностей должна быть равна 1, а у вас " + 
                        sumP
                        );
                    return;
                }


                Dictionary<double,double> practicalAllocation = new Dictionary<double, double>();
                foreach (var d in rowрAllocation)
                {
                    practicalAllocation.Add(d.Key,0);
                }


                var random = new Random();
                Mutex mutex = new Mutex();
                Parallel.For(0, Convert.ToInt32(numericUpDown2.Value), i =>
                    {
                        double experiment = func.functionDiscrete(rowрAllocation, random.NextDouble());
                        mutex.WaitOne();
                        sample.Add(experiment);
                        //textBox1.Text += experiment + "\r\n";
                        practicalAllocation[experiment] += Convert.ToDouble(1/numericUpDown2.Value);

                        mutex.ReleaseMutex();
                    }
                    );

                textBox1.Text = "";
                for (int i = 0; i < 50; i++)
                {
                    textBox1.Text += sample[i] + "\r\n";
                }
                


                /*
                 * рисование
                 */
                PointPairList list1 = new PointPairList();
                PointPairList list2 = new PointPairList();
                // Заполняем список точек
                foreach (var dr in rowрAllocation)
                {
                //     добавим в список точку
                    list1.Add(dr.Key, dr.Value);

                }
                double s = 0;
                foreach (var dr in practicalAllocation)
                {
                    //     добавим в список точку
                    list2.Add(dr.Key-0.1, dr.Value);
                    s += dr.Value;

                }
                zedGraphControl2.BringToFront();
                Drawing drawing = new Drawing(zedGraphControl2);
                drawing.drawBar(zedGraphControl2, list1, Color.Red);
                drawing.drawBar(zedGraphControl2, list2, Color.RoyalBlue);
            }
        }


        /***
         * универсальный метод
         * */
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
                    S += func.functionUniversal(x)*dx;
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

            Mutex mutex = new Mutex();
            Parallel.For(0, Convert.ToInt32(numericUpDown2.Value), i =>
                {
                    int r = random.Next(Convert.ToInt32(numericUpDown1.Value) - 1);
                    mutex.WaitOne();
                    sample.Add(
                        func.functionUniversal((borderInterval[r + 1] - borderInterval[r])
                                               *random.NextDouble() + borderInterval[r])
                        );
                    mutex.ReleaseMutex();
                }
                );


            for (int i = 0; i < 50; i++)
            {
                textBox1.Text += sample[i] + "\r\n";
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {

            dataGridView1.Rows.Clear();
            
            //dataGridView1.Columns.Add("", "");
            for (int i = 0; i < Convert.ToInt32(numericUpDown3.Value); i++)
            {
                dataGridView1.Rows.Add();
                dataGridView1.Rows[i].Height = 20;
            }
            dataGridView1.Height = dataGridView1.Rows.Count * 20 + 23;
           // dataGridView1.Columns.Add("", "");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Dictionary<double, double> prim = new Dictionary<double, double>();
            prim.Add(-6, 0.11);
            prim.Add(-5, 0.02);
            prim.Add(-1, 0.07);
            prim.Add(0, 0.22);
            prim.Add(2, 0.01);
            prim.Add(5, 0.13);
            prim.Add(6, 0.04);
            prim.Add(12, 0.05);
            prim.Add(15, 0.13);
            prim.Add(20, 0.12);
            prim.Add(22, 0.1);

            numericUpDown3.Value = 11;
            dataGridView1.Rows.Clear();

            int i = 0;
            foreach (var d in prim)
            {
                dataGridView1.Rows.Add(d.Key, d.Value);
                dataGridView1.Rows[i].Height = 20;
                i++;
            }


            dataGridView1.Height = dataGridView1.Rows.Count * 20 + 23;
        }
    }
}
