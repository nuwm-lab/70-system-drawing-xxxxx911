using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace LabWork
{
    internal sealed class GraphDrawer
    {
        private readonly double _xStart;
        private readonly double _xEnd;
        private readonly double _step;

        public GraphDrawer(double xStart, double xEnd, double step)
        {
            if (step <= 0) throw new ArgumentOutOfRangeException(nameof(step));
            if (xEnd <= xStart) throw new ArgumentException("xEnd must be greater than xStart");

            _xStart = xStart;
            _xEnd = xEnd;
            _step = step;
        }

        public void Draw(Graphics graphics, Rectangle clientRect)
        {
            if (graphics is null) throw new ArgumentNullException(nameof(graphics));

            graphics.SmoothingMode = SmoothingMode.AntiAlias;

            const int margin = 40;
            var plotRect = new Rectangle(
                clientRect.Left + margin,
                clientRect.Top + margin,
                Math.Max(10, clientRect.Width - margin * 2),
                Math.Max(10, clientRect.Height - margin * 2)
            );

            var points = ComputePoints();
            if (points.Count == 0) return;

            double yMin = double.PositiveInfinity, yMax = double.NegativeInfinity;
            foreach (var p in points)
            {
                if (p.Y < yMin) yMin = p.Y;
                if (p.Y > yMax) yMax = p.Y;
            }

            if (yMin == yMax)
            {
                yMin -= 1;
                yMax += 1;
            }

            using var axisPen = new Pen(Color.Black, 1);
            graphics.DrawLine(axisPen, plotRect.Left, plotRect.Bottom, plotRect.Right, plotRect.Bottom);
            graphics.DrawLine(axisPen, plotRect.Left, plotRect.Top, plotRect.Left, plotRect.Bottom);

            using var font = new Font("Segoe UI", 8);
            for (double x = _xStart; x <= _xEnd + 1e-9; x += _step)
            {
                var px = XToPixel(x, plotRect);
                graphics.DrawLine(Pens.Black, px, plotRect.Bottom, px, plotRect.Bottom + 4);
                var lbl = x.ToString("0.##");
                var sz = graphics.MeasureString(lbl, font);
                graphics.DrawString(lbl, font, Brushes.Black, px - sz.Width / 2, plotRect.Bottom + 4);
            }

            for (int i = 0; i <= 3; i++)
            {
                double yValue = yMin + i * (yMax - yMin) / 3.0;
                var py = YToPixel(yValue, plotRect, yMin, yMax);
                graphics.DrawLine(Pens.Black, plotRect.Left - 4, py, plotRect.Left, py);
                var lbl = yValue.ToString("0.###");
                var sz = graphics.MeasureString(lbl, font);
                graphics.DrawString(lbl, font, Brushes.Black, plotRect.Left - sz.Width - 6, py - sz.Height / 2);
            }

            var graphPoints = new List<PointF>(points.Count);
            foreach (var p in points)
            {
                float px = XToPixel(p.X, plotRect);
                float py = YToPixel(p.Y, plotRect, yMin, yMax);
                graphPoints.Add(new PointF(px, py));
            }

            using var curvePen = new Pen(Color.Blue, 2);
            if (graphPoints.Count >= 2)
            {
                graphics.DrawLines(curvePen, graphPoints.ToArray());
            }

            graphics.DrawString("y = arccos(x) / (2x + 1)", font, Brushes.DarkRed, clientRect.Left + 6, clientRect.Top + 6);
        }

        private List<(double X, double Y)> ComputePoints()
        {
            var list = new List<(double X, double Y)>();
            for (double x = _xStart; x <= _xEnd + 1e-9; x += _step)
            {
                double y = Math.Acos(x) / (2.0 * x + 1.0);
                list.Add((x, y));
            }

            return list;
        }

        private float XToPixel(double x, Rectangle plotRect)
        {
            double t = (x - _xStart) / (_xEnd - _xStart);
            return (float)(plotRect.Left + t * plotRect.Width);
        }

        private float YToPixel(double y, Rectangle plotRect, double yMin, double yMax)
        {
            double t = (y - yMin) / (yMax - yMin);
            return (float)(plotRect.Bottom - t * plotRect.Height);
        }
    }
}