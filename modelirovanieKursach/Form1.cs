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
                    if (i < numericUpDown2.Value)
                    {
                        textBox1.Text += sample[i] + "\r\n";
                    }
                }

                var functionPlotRecove = intervalsPRecove(sample);
                var functionRasprRecove = plotToRaspr(functionPlotRecove);


                /*
             * распределения теоретическое и практические на интервалах
             * для метода обратной функции непрерывных величин
             */

                List<double> reversRasprContinPract = new List<double>();
                for (int i = 1; i < functionRasprRecove.Count; i++)
                {
                    reversRasprContinPract.Add(functionRasprRecove[i]);
                }

                List<double> reversRasprContinTeor = new List<double>();
                double step = func.functionReverse(1)/Convert.ToDouble(numericUpDown1.Value);
                for (int i = 0; i < numericUpDown1.Value; i++ )
                {
                    reversRasprContinTeor.Add(func.functionReverseRasp((i+1)*step));
                }
                
                
                /*
                 * рисование
                 */
                #region Plotnost

                 step = 0;
                PointPairList rasprRecove = new PointPairList();
                for (int i = 1; i <= numericUpDown1.Value; i++)
                {
                    rasprRecove.Add(step, 0);
                    rasprRecove.Add(step, functionPlotRecove[i]);
                    step += functionPlotRecove[0];
                    rasprRecove.Add(step, functionPlotRecove[i]);
                    rasprRecove.Add(step, 0);
                }

                PointPairList list4 = new PointPairList();
                for (double i = 0; i <= 0.25; i += 0.0001)
                {
                    list4.Add(i, func.functionReversePlotn(i));
                }

                Drawing drawing1 = new Drawing(zedGraphControl3);
                drawing1.DrawFunction(zedGraphControl3, rasprRecove, Color.Blue);
                //drawing1.DrawFunction(zedGraphControl2, list3, Color.Red);
                drawing1.DrawFunction(zedGraphControl3, list4, Color.Red);

                #endregion


                #region viborka
                PointPairList list = new PointPairList();
                for (int i = 0; i < 50; i++)
                {
                    if (i < numericUpDown2.Value)
                    {
                        list.Add(i + 1, 0);
                        list.Add(i + 1, sample[i]);
                        list.Add(i + 1, 0);
                    }

                }
                Drawing drawing = new Drawing(zedGraphControl1);
                drawing.DrawFunction(zedGraphControl1, list, Color.Red);
                #endregion


                #region raspred

                step = 0;

                rasprRecove = new PointPairList();
                for (int i = 1; i <= numericUpDown1.Value; i++)
                {
                    rasprRecove.Add(step, 0);
                    rasprRecove.Add(step, functionRasprRecove[i]);
                    step += functionRasprRecove[0];
                    rasprRecove.Add(step, functionRasprRecove[i]);
                    rasprRecove.Add(step, 0);
                }

                
                list4 = new PointPairList();
                for (double i = 0; i <= 0.25; i += 0.0001)
                {
                    list4.Add(i, func.functionReverseRasp(i));
                }

                Drawing drawing2 = new Drawing(zedGraphControl2);
                drawing2.DrawFunction(zedGraphControl2, rasprRecove, Color.Blue);
                drawing2.DrawFunction(zedGraphControl2, list4, Color.Red);


                #endregion

            }


            /**
             метод обратной функции для дискретных величин
             */
            if (radioButton5.Checked)
            {

                List<double> sample = new List<double>();
                double sumP = 0;
                textBox1.Text = "";
                Dictionary<double, double> rowрAllocation = new Dictionary<double, double>();

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


                Dictionary<double, double> practicalAllocation = new Dictionary<double, double>();
                foreach (var d in rowрAllocation)
                {
                    practicalAllocation.Add(d.Key, 0);
                }


                var random = new Random();
                Mutex mutex = new Mutex();
                Parallel.For(0, Convert.ToInt32(numericUpDown2.Value), i =>
                    {
                        double experiment = func.functionDiscrete(rowрAllocation, random.NextDouble());
                        mutex.WaitOne();
                        sample.Add(experiment);
                        practicalAllocation[experiment] += Convert.ToDouble(1 / numericUpDown2.Value);
                        mutex.ReleaseMutex();
                    }
                    );

                textBox1.Text = "";
                for (int i = 0; i < 50; i++)
                {
                    if (i < numericUpDown2.Value)
                    {
                        textBox1.Text += sample[i] + "\r\n";
                    }
                }



                /*
                 * рисование
                 */

                #region viborka
                PointPairList list = new PointPairList();
                for (int i = 0; i < 50; i++)
                {
                    if (i < numericUpDown2.Value)
                    {
                        list.Add(i + 1, 0);
                        list.Add(i + 1, sample[i]);
                        list.Add(i + 1, 0);
                    }

                }
                Drawing drawing1 = new Drawing(zedGraphControl1);
                drawing1.DrawFunction(zedGraphControl1, list, Color.Red);
                #endregion

                #region plot
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
                    list2.Add(dr.Key - 0.1, dr.Value);
                    s += dr.Value;

                }
                zedGraphControl3.BringToFront();
                Drawing drawing = new Drawing(zedGraphControl3);
                drawing.drawBar(zedGraphControl3, list1, Color.Red);
                drawing.drawBar(zedGraphControl3, list2, Color.RoyalBlue);
                #endregion

                #region raspr
                PointPairList list3 = new PointPairList();
                PointPairList list4 = new PointPairList();
                // Заполняем список точек
                double akk = 0;
                
                foreach (var dr in rowрAllocation.Reverse())
                {
                    list3.Add(dr.Key,akk);
                    list3.Add(dr.Key,dr.Value+akk);
                    akk += dr.Value;

                }
                s = 0;
                akk = 0;
                foreach (var dr in practicalAllocation.Reverse())
                {
                    list4.Add(dr.Key, akk);
                    list4.Add(dr.Key, dr.Value + akk);
                    akk += dr.Value;
                }
                drawing = new Drawing(zedGraphControl2);
                drawing.DrawFunction(zedGraphControl2, list3, Color.Red);
                drawing.DrawFunction(zedGraphControl2, list4, Color.RoyalBlue);
                #endregion
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

            plochad = func.integralUniversal(0, 9, dx);
            chanceInterval = plochad / Convert.ToDouble(numericUpDown1.Value);

            double S = 0;
            borderInterval.Add(0);
            /////////////////////////////////////
            /// последовательный поиск границ
            /// ////////////////

            if (radioButton1.Checked)
            {
                dx *= 0.01;
                while (x <= 9)
                {
                    S += func.functionUniversal(x) * dx;
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
                    S = func.integralUniversal(beginInterval, 9, dx);
                    xpen = Math.Abs(chanceInterval - S);
                    while (xpen > dx)
                    {
                        S = func.integralUniversal(beginInterval, (x1 + x0) / 2, dx);
                        if (S > chanceInterval)
                        {
                            x1 = (x1 + x0) / 2;
                        }
                        else
                        {
                            x0 = (x1 + x0) / 2;
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
                    randomPoint = random.NextDouble() * (x1 - x0) + x0;
                    S = func.integralUniversal(beginInterval, 9, dx);
                    xpen = Math.Abs(chanceInterval - S);
                    while (xpen > dx)
                    {
                        S = func.integralUniversal(beginInterval, randomPoint, dx);
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
                    int r = random.Next(Convert.ToInt32(numericUpDown1.Value) );

                    mutex.WaitOne();
                    sample.Add(
                        ((borderInterval[r + 1] - borderInterval[r])
                                               * random.NextDouble() + borderInterval[r])
                        );
                    mutex.ReleaseMutex();
                }
                );

            


            for (int i = 0; i < 50; i++)
            {
                textBox1.Text += sample[i] + "\r\n";
            }


            var functionPlotRecove = intervalsPRecove(sample);
            var functionRasprRecove = plotToRaspr(functionPlotRecove);

            /*
             * распределения теоретическое и практические на интервалах
             * для универсального метода
             */

            List<double> universalRasprContinPract = new List<double>();
            for (int i = 1; i < functionRasprRecove.Count; i++)
            {
                universalRasprContinPract.Add(functionRasprRecove[i]);
            }

            List<double> universalRasprContinTeor = new List<double>();
            S = func.integralUniversal(0, 9, 0.001);
            double step = 9 / Convert.ToDouble(numericUpDown1.Value);
            for (int i = 0; i < numericUpDown1.Value; i++)
            {
                universalRasprContinTeor.Add(func.integralUniversal(0, (i + 1) * step, 0.001)/S);
            }


            /*
             * рисование
             */

            #region viborka
            PointPairList list = new PointPairList();
            for (int i = 0; i < 50; i++)
            {
                if (i < numericUpDown2.Value)
                {
                    list.Add(i + 1, 0);
                    list.Add(i + 1, sample[i]);
                    list.Add(i + 1, 0);
                }

            }
            Drawing drawing = new Drawing(zedGraphControl1);
            drawing.DrawFunction(zedGraphControl1, list, Color.Red);
            #endregion

            #region Plotnost

             step = 0;
            PointPairList rasprRecove = new PointPairList();
            for (int i = 1; i <= numericUpDown1.Value; i++)
            {
                rasprRecove.Add(step, 0);
                rasprRecove.Add(step, functionPlotRecove[i]);
                step += functionPlotRecove[0];
                rasprRecove.Add(step, functionPlotRecove[i]);
                rasprRecove.Add(step, 0);
            }

            PointPairList list4 = new PointPairList();
            list4 = new PointPairList();
            double koef = func.integralUniversal(0, 9, 0.001);
            for (double i = 0; i <= 9; i += 0.001)
            {
                list4.Add(i, func.functionUniversal(i)/koef);
            }

            drawing = new Drawing(zedGraphControl3);
            drawing.DrawFunction(zedGraphControl3, rasprRecove, Color.Red);
            drawing.DrawFunction(zedGraphControl3, list4, Color.Blue);

            #endregion

            #region raspred

            step = 0;

            rasprRecove = new PointPairList();
            for (int i = 1; i <= numericUpDown1.Value; i++)
            {
                rasprRecove.Add(step, 0);
                rasprRecove.Add(step, functionRasprRecove[i]);
                step += functionRasprRecove[0];
                rasprRecove.Add(step, functionRasprRecove[i]);
                rasprRecove.Add(step, 0);
            }


            list4 = new PointPairList();
            double akk = 0;
            double S1 = func.integralUniversal(0, 9, 0.001);

            for (double i = 0; i <= 9; i += 0.0001)
            {
                akk = akk + (func.functionUniversal(i)/S1*0.0001);
                list4.Add(i,akk);
            }

            Drawing drawing2 = new Drawing(zedGraphControl2);
            drawing2.DrawFunction(zedGraphControl2, rasprRecove, Color.Red);
            drawing2.DrawFunction(zedGraphControl2, list4, Color.Blue);


            #endregion

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

        private List<double> intervalsPRecove(List<double> sampleList)
        {
            var result = new List<double>();
            double max = sampleList[0];

            Mutex mutex = new Mutex();
            Parallel.ForEach(sampleList, sam =>
            {
                mutex.WaitOne();
                if (sam > max)
                {
                    max = sam;
                }
                mutex.ReleaseMutex();
            }
                );

            for (int i = 0; i < numericUpDown1.Value + 1; i++)
            {
                result.Add(0);
            }

            double step = max / Convert.ToDouble(numericUpDown1.Value);

            result[0] = step;

            Parallel.ForEach(sampleList, sam =>
            {
                for (int i = 1; i <= numericUpDown1.Value; i++)
                {
                    if (sam <= i * step)
                    {
                        mutex.WaitOne();
                        result[i]++;
                        mutex.ReleaseMutex();
                        break;
                    }
                }
            }
                );
            for (int i = 1; i <= numericUpDown1.Value; i++)
            {
                result[i] /= Convert.ToDouble(numericUpDown2.Value);
                result[i] /= step;
            }
            return result;
        }

        private List<double> plotToRaspr(List<double> plot)
        {
            List<double> raspr = new List<double>();
            raspr.Add(plot[0]);
            double akk = 0;
            for (int i = 1; i < plot.Count; i++)
            {
                akk += plot[i];
                raspr.Add(akk);
            }
            for (int i = 1; i <= numericUpDown1.Value; i++)
            {
                raspr[i] *= plot[0];
            }
            return raspr;
        }

    }
}
