using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZedGraph;

namespace modelirovanieKursach
{
    static class recoveryFunction
    {
        public static Dictionary<double,double> recoveContinuos(List<double> sample)
        {
            PointPairList recove = new PointPairList();

            double xMin = sample[0];
            double xMax = sample[0];
            double dx;

            Mutex mutex = new Mutex();
            Parallel.ForEach(sample, s =>
            {
                if (s>xMax)
                {
                    mutex.WaitOne();
                    xMax = 0.25;
                    mutex.ReleaseMutex();
                }
                if (s<xMin)
                {
                    mutex.WaitOne();
                    xMin = 0;
                    mutex.ReleaseMutex();
                }
            }
            );
            dx = (xMax - xMin) / 50;

            List<double> border = new List<double>();
            Dictionary<double, double> experimentSample = new Dictionary<double, double>();

            
            for (double i = xMin; i < xMax; i+=dx)
            {
                border.Add(Math.Round(i,15));
                experimentSample.Add(Math.Round(i, 15),0);
            }

            

            Parallel.ForEach(sample, s =>
            {
                
                foreach (var d in border)
                {
                    if (d>s)
                    {
                        mutex.WaitOne();
                        experimentSample[d] += (1.0 / sample.Count)/dx;
                        mutex.ReleaseMutex();
                        break;
                    }
                }
                experimentSample[Math.Round(xMin, 15)] += (1.0 / sample.Count) / dx;
            }
            );

            return experimentSample;
        }
    }
}
