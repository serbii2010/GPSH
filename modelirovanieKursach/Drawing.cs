using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZedGraph;

namespace modelirovanieKursach
{
    class Drawing
    {
        public Drawing(ZedGraphControl zedGraph)
        {
            // Получим панель для рисования
            GraphPane pane = zedGraph.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.DashOn = 10;

            pane.YAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.DashOn = 10;
        }

        public void DrawFunction(ZedGraphControl zedGraph, PointPairList list, Color color)
        {
            GraphPane pane = zedGraph.GraphPane;

            pane.AddCurve("ф-я", list, color, SymbolType.None);
            pane.Title.Text = "";

            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }

        public void drawBar(ZedGraphControl zedGraph, PointPairList pairList, Color color)
        {
            GraphPane pane = zedGraph.GraphPane;

            foreach (var l in pairList)
            {
                PointPairList list = new PointPairList();
                list.Add(l.X,0);
                list.Add(l.X,l.Y);
                LineItem myItem = pane.AddCurve("", list, color, SymbolType.None);
                myItem.Line.Width = 3.0f;
            }

            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }
    }
}
