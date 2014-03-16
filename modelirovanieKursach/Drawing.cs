﻿using System;
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
            pane.XAxis.MinorGrid.IsVisible = true;
            pane.XAxis.MinorGrid.DashOn = 3;
            pane.XAxis.MinorGrid.DashOff = 2;

            pane.YAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.DashOn = 10;
            pane.YAxis.MinorGrid.IsVisible = true;
            pane.YAxis.MinorGrid.DashOn = 3;
            pane.YAxis.MinorGrid.DashOff = 2;
        }

        public void DrawFunction(ZedGraphControl zedGraph, PointPairList list)
        {
            GraphPane pane = zedGraph.GraphPane;

            PointPairList list1 = new PointPairList();
            list1.Add(1, 1);
            list1.Add(-1,1);

            //pane.AddCurve("", list1, Color.Red, SymbolType.None);

            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            pane.AddCurve("ф-я", list, Color.Blue, SymbolType.None);
            pane.Title.Text = "";

            // Вызываем метод AxisChange (), чтобы обновить данные об осях. 
            // В противном случае на рисунке будет показана только часть графика, 
            // которая умещается в интервалы по осям, установленные по умолчанию
            zedGraph.AxisChange();

            // Обновляем график
            zedGraph.Invalidate();
        }

        public void drawBar(ZedGraphControl zedGraph, PointPairList pairList)
        {
            // Получим панель для рисования
            GraphPane pane = zedGraph.GraphPane;

            PointPairList list = new PointPairList();
            foreach (var l in pairList)
            {
                list.Add(l.X,0);
                list.Add(l.X,l.Y);
                list.Add(l.X, 0);
            }

            

            LineItem myItem = pane.AddCurve("", list, Color.Red, SymbolType.None);
            myItem.Line.Width = 3.0f;
            zedGraph.AxisChange();
            zedGraph.Invalidate();
        }
    }
}
