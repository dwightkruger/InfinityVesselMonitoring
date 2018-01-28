//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Utilities;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class CompassRoseGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;

        public CompassRoseGauge()
        {
            this.InitializeComponent();
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void TitleControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = (float)this.TextFontSize,
            })
            {
                Vector2 at = new Vector2(0, (float)sender.ActualHeight / 2);
                ds.DrawText(this.Text, at, this.GaugeColor, textFormat);
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = (float)this.TextFontSize,
            })
            {
                string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
                Vector2 at = new Vector2((float)sender.ActualWidth, 0F);
                ds.DrawText(string.Format(format + "{1}", this.Value, UnitItem.ToString(VesselMonitoring.Utilities.Units.Degrees)), at, this.GaugeColor, textFormat);
            }
        }

        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            this.Center = new Vector2((float)sender.ActualWidth / 2, (float)sender.ActualHeight / 2);

            float length = this.Center.Y - this.Center.Y / 2F;
            Vector2 start  = new Vector2(this.Center.X,          this.Center.Y - length);
            Vector2 left   = new Vector2(this.Center.X - length, this.Center.Y);
            Vector2 bottom = new Vector2(this.Center.X,          this.Center.Y + length);
            Vector2 right = new Vector2(this.Center.X + length,  this.Center.Y);
            Vector2 leftTop     = new Vector2((float)Math.Cos(RadiansFromDegrees(5*45)) * length/2F, (float)Math.Sin(RadiansFromDegrees(5*45)) * length/2F) + this.Center;
            Vector2 leftBottom  = new Vector2((float)Math.Cos(RadiansFromDegrees(3*45)) * length / 2F, (float)Math.Sin(RadiansFromDegrees(3*45)) * length / 2F) + this.Center;
            Vector2 rightTop    = new Vector2((float)Math.Cos(RadiansFromDegrees(7*45)) * length / 2F, (float)Math.Sin(RadiansFromDegrees(7*45)) * length / 2F) + this.Center;
            Vector2 rightBottom = new Vector2((float)Math.Cos(RadiansFromDegrees(45)) * length / 2F, (float)Math.Sin(RadiansFromDegrees(45)) * length / 2F) + this.Center;

            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                cp.BeginFigure(start);
                cp.AddLine(leftTop);
                cp.AddLine(left);
                cp.AddLine(leftBottom);
                cp.AddLine(bottom);
                cp.AddLine(rightBottom);
                cp.AddLine(right);
                cp.AddLine(rightTop);
                cp.AddLine(start);
                cp.EndFigure(CanvasFigureLoop.Closed);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    ds.FillGeometry(geometry, Colors.Blue);
                }
            }

            float circle5Radius = (float)sender.ActualWidth / 12;
            float circle4Radius = (float)sender.ActualWidth / 10;
            float circle3Radius = (float)sender.ActualWidth / 7;
            float circle2Radius = (float)sender.ActualWidth / 6.5F;
            float circle1Radius = (float)sender.ActualWidth / 6.0F;

            ds.FillCircle(this.Center, circle1Radius, Colors.DarkGray);
            ds.FillCircle(this.Center, circle2Radius, Colors.Black);
            ds.FillCircle(this.Center, circle3Radius, Colors.DarkGray);
            ds.FillCircle(this.Center, circle4Radius, Colors.White);
            ds.FillCircle(this.Center, circle5Radius, Colors.DarkGray);

            float radius = (float)sender.ActualWidth / 2F - 2F;
            ds.DrawCircle(this.Center, radius, this.GaugeColor, 4F);

            float minorTicLength = 15;
            float minorTicstrokeWidth = 3;
            for (int i = 0; i < 360; i += 10)
            {
                float x1 = (float)Math.Cos(RadiansFromDegrees(i)) * radius;
                float y1 = (float)Math.Sin(RadiansFromDegrees(i)) * radius;
                float x2 = (float)Math.Cos(RadiansFromDegrees(i)) * (radius - minorTicLength);
                float y2 = (float)Math.Sin(RadiansFromDegrees(i)) * (radius - minorTicLength);
                using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
                {
                    cp.BeginFigure(new Vector2(x1, y1) + this.Center);
                    cp.AddLine(new Vector2(x2, y2) + this.Center);
                    cp.EndFigure(CanvasFigureLoop.Open);
                    using (var geometry = CanvasGeometry.CreatePath(cp))
                    {
                        ds.DrawGeometry(geometry, this.GaugeColor, minorTicstrokeWidth);
                    }
                }
            }

            float majorTicLength = 30;
            float majorTicstrokeWidth = 6;
            for (int i=0; i<360; i+=30)
            {
                float x1 = (float)Math.Cos(RadiansFromDegrees(i)) * radius;
                float y1 = (float)Math.Sin(RadiansFromDegrees(i)) * radius;
                float x2 = (float)Math.Cos(RadiansFromDegrees(i)) * (radius - majorTicLength);
                float y2 = (float)Math.Sin(RadiansFromDegrees(i)) * (radius - majorTicLength);
                using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
                {
                    cp.BeginFigure(new Vector2(x1,y1) + this.Center);
                    cp.AddLine(new Vector2(x2, y2) + this.Center);
                    cp.EndFigure(CanvasFigureLoop.Open);
                    using (var geometry = CanvasGeometry.CreatePath(cp))
                    {
                        ds.DrawGeometry(geometry, this.GaugeColor, majorTicstrokeWidth);
                    }
                }

                float atX = (float)Math.Cos(RadiansFromDegrees(i)) * (radius - 1.8F*majorTicLength);
                float atY = (float)Math.Sin(RadiansFromDegrees(i)) * (radius - 1.8F*majorTicLength);
                Vector2 at = new Vector2(atX, atY) + this.Center;

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)this.LabelsFontSize * 1.5F,
                })
                {
                    int label = (i+90) % 360;
                    if (000 == label)      ds.DrawText("N", at, this.GaugeColor, textFormat);
                    else if (090 == label) ds.DrawText("E", at, this.GaugeColor, textFormat);
                    else if (180 == label) ds.DrawText("S", at, this.GaugeColor, textFormat);
                    else if (270 == label) ds.DrawText("W", at, this.GaugeColor, textFormat);
                    else               ds.DrawText(i.ToString(), at, this.GaugeColor, textFormat);
                }
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.TextFontSize,
            })
            {
                Vector2 at = new Vector2((float)sender.ActualWidth, (float)sender.ActualHeight);
                ds.DrawText("Magnetic", at, this.GaugeColor, textFormat);
            }
        }

        protected Vector2 Center { get; set; }

        protected CanvasStrokeStyle ArcStrokeStyle = new CanvasStrokeStyle()
        {
            DashStyle = CanvasDashStyle.Solid,
            StartCap = CanvasCapStyle.Flat,
            EndCap = CanvasCapStyle.Flat,
        };

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            _needsResourceRecreation = false;
        }

        protected Color GaugePointerColor
        {
            get
            {
                Color result = this.GaugeColor;

                if (IsLowAlarmEnabled && (Value <= LowAlarmValue))
                {
                    result = this.LowAlarmColor;
                }
                else if (IsLowWarningEnabled && (Value <= LowWarningValue))
                {
                    result = this.LowWarningColor;
                }
                else if (IsHighAlarmEnabled && (Value >= HighAlarmValue))
                {
                    result = this.HighAlarmColor;
                }
                else if (IsHighWarningEnabled && (Value >= HighWarningValue))
                {
                    result = this.HighWarningColor;
                }

                return result;
            }
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLeft(object oldValue, object newValue)
        {
            Canvas.SetLeft(this.MainGrid, this.Left);
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshTop(object oldValue, object newValue)
        {
            Canvas.SetTop(this.MainGrid, this.Top);
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshGaugeHeight(object oldValue, object newValue)
        {
            this.canvasControl.Height = Convert.ToDouble(newValue);
            this.MainGrid.Height = Convert.ToDouble(newValue);
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshGaugeWidth(object oldValue, object newValue)
        {
            this.canvasControl.Width = Convert.ToDouble(newValue);
            this.MainGrid.Width = Convert.ToDouble(newValue);
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshLabelsFontSize(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshValueFontSize(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        static protected float RadiansFromDegrees(double degrees)
        {
            return (float)(degrees * Math.PI / 180F);
        }
    }
}
