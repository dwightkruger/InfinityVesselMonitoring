//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;

namespace InfinityGroup.VesselMonitoring.Controls
{
    class ChartRenderer
    {
        public void RenderAxes(CanvasControl canvas, CanvasDrawEventArgs args)
        {
            var width = (float)canvas.ActualWidth;
            var height = (float)(canvas.ActualHeight);
            var midWidth = (float)(width * .5);
            var midHeight = (float)(height * .5);

            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                // Horizontal line
                cpb.BeginFigure(new Vector2(0, midHeight));
                cpb.AddLine(new Vector2(width, midHeight));
                cpb.EndFigure(CanvasFigureLoop.Open);

                // Horizontal line arrow
                cpb.BeginFigure(new Vector2(width - 10, midHeight - 3));
                cpb.AddLine(new Vector2(width, midHeight));
                cpb.AddLine(new Vector2(width - 10, midHeight + 3));
                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", 5, midHeight - 30, Colors.Gray);
            args.DrawingSession.DrawText(canvas.ActualWidth.ToString(), width - 50, midHeight - 30, Colors.Gray);

            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                // Vertical line
                cpb.BeginFigure(new Vector2(midWidth, 0));
                cpb.AddLine(new Vector2(midWidth, height));
                cpb.EndFigure(CanvasFigureLoop.Open);

                // Vertical line arrow
                cpb.BeginFigure(new Vector2(midWidth - 3, 10));
                cpb.AddLine(new Vector2(midWidth, 0));
                cpb.AddLine(new Vector2(midWidth + 3, 10));
                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), Colors.Gray, 1);
            }

            args.DrawingSession.DrawText("0", midWidth + 5, height - 30, Colors.Gray);
            args.DrawingSession.DrawText("1", midWidth + 5, 5, Colors.Gray);
        }

        public void RenderData(CanvasControl canvas, CanvasDrawEventArgs args, Color color, float thickness, List<double> data, bool renderArea)
        {
            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                cpb.BeginFigure(new Vector2(0, (float)(canvas.ActualHeight * (1 - data[0]))));

                for (int i = 1; i < data.Count; i++)
                {
                    cpb.AddLine(new Vector2(i, (float)(canvas.ActualHeight * (1 - data[i]))));
                }

                if (renderArea)
                {
                    cpb.AddLine(new Vector2(data.Count, (float)canvas.ActualHeight));
                    cpb.AddLine(new Vector2(0, (float)canvas.ActualHeight));
                    cpb.EndFigure(CanvasFigureLoop.Closed);
                    args.DrawingSession.FillGeometry(CanvasGeometry.CreatePath(cpb), Colors.LightGreen);
                }
                else
                {
                    cpb.EndFigure(CanvasFigureLoop.Open);
                    args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
                }
            }
        }

        public void RenderAveragesAsColumns(CanvasControl canvas, CanvasDrawEventArgs args, int columnAvgDataRange, float columnWidth, List<double> data)
        {
            var padding = .5 * (columnAvgDataRange - columnWidth);
            for (int start = 0; start < data.Count; start += columnAvgDataRange)
            {
                double total = 0;
                var range = Math.Min(columnAvgDataRange, data.Count - start);

                for (int i = start; i < start + range; i++)
                {
                    total += data[i];
                }

                args.DrawingSession.FillRectangle(
                    start + (float)padding,
                    (float)(canvas.ActualHeight * (1 - total / range)),
                    columnWidth,
                    (float)(canvas.ActualHeight * (total / range)),
                    Colors.WhiteSmoke);
            }
        }

        public void RenderAveragesAsPieChart(
            CanvasControl canvas,                   // Where to render
            CanvasDrawEventArgs args,               // Our drawing session
            List<double> pieValues,                 // The values to calculate the size of each slice
            List<string> pieLabels,                 // The slice labels
            Color borderColor,                      // The color of the border for each slice
            List<Color> palette)                    // A list of colors for the slices
        {
            var total = pieValues.Sum();

            var width = (float)canvas.ActualWidth;
            var height = (float)canvas.ActualHeight;
            var midX = width / 2;
            var midY = height / 2;
            var padding = 1; 
            var radius = Math.Min(width, height) / 2 - padding;

            float angle = 0f;
            var center = new Vector2(midX, midY);

            // Draw the pie slices
            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);
                var arcStartPoint = new Vector2((float)(midX + radius * Math.Sin(angle)), (float)(midY - radius * Math.Cos(angle)));

                using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                {
                    cpb.BeginFigure(center);
                    cpb.AddLine(arcStartPoint);
                    cpb.AddArc(new Vector2(midX, midY), radius, radius, angle - (float)(Math.PI / 2), sweepAngle);
                    cpb.EndFigure(CanvasFigureLoop.Closed);
                    args.DrawingSession.FillGeometry(CanvasGeometry.CreatePath(cpb), palette[i % palette.Count]);
                }

                using (var cpb = new CanvasPathBuilder(args.DrawingSession))
                {
                    cpb.BeginFigure(center);
                    cpb.AddLine(arcStartPoint);
                    cpb.AddArc(new Vector2(midX, midY), radius, radius, angle - (float)(Math.PI / 2), sweepAngle);
                    cpb.EndFigure(CanvasFigureLoop.Open);
                    args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), borderColor);
                }

                angle += sweepAngle;
            }

            args.DrawingSession.DrawCircle(center, radius, borderColor);

            // Label each of the pie slices
            angle = 0f;
            for (int i = 0; i < pieValues.Count; i++)
            {
                float sweepAngle = (float)(2 * Math.PI * pieValues[i] / total);

                if (palette[i % palette.Count] != Colors.Transparent)
                {
                    double midAngle = angle + sweepAngle / 2;
                    Vector2 pt = new Vector2((float)(midX + (radius*.75) * Math.Sin(midAngle)), (float)(midY - (radius*0.75)* Math.Cos(midAngle)));

                    args.DrawingSession.DrawText(
                        pieLabels[i],
                        pt,
                        borderColor,
                        new CanvasTextFormat
                        {
                            HorizontalAlignment = CanvasHorizontalAlignment.Center,
                            VerticalAlignment = CanvasVerticalAlignment.Center,
                            FontSize = 14,
                        });
                }

                angle += sweepAngle;
            }
        }

        public void RenderMovingAverage(CanvasControl canvas, CanvasDrawEventArgs args, Color color, float thickness, int movingAverageRange, List<double> data)
        {
            using (var cpb = new CanvasPathBuilder(args.DrawingSession))
            {
                cpb.BeginFigure(new Vector2(0, (float)(canvas.ActualHeight * (1 - data[0]))));

                double total = data[0];

                int previousRangeLeft = 0;
                int previousRangeRight = 0;

                for (int i = 1; i < data.Count; i++)
                {
                    var range = Math.Max(0, Math.Min(movingAverageRange / 2, Math.Min(i, data.Count - 1 - i)));
                    int rangeLeft = i - range;
                    int rangeRight = i + range;

                    for (int j = previousRangeLeft; j < rangeLeft; j++)
                    {
                        total -= data[j];
                    }

                    for (int j = previousRangeRight + 1; j <= rangeRight; j++)
                    {
                        total += data[j];
                    }

                    previousRangeLeft = rangeLeft;
                    previousRangeRight = rangeRight;

                    cpb.AddLine(new Vector2(i, (float)(canvas.ActualHeight * (1 - total / (range * 2 + 1)))));
                }

                cpb.EndFigure(CanvasFigureLoop.Open);

                args.DrawingSession.DrawGeometry(CanvasGeometry.CreatePath(cpb), color, thickness);
            }
        }
    }
}
