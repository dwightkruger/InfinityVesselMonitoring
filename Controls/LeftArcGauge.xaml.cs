//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Diagnostics;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class LeftArcGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        private const float c_arcThickness = 4;
        private const float c_needleThickness = 4;
        private const float c_endAngle = 0;
        private const float c_startAngle = 130;
        private const double c_arcSweep = 360 - c_endAngle - c_startAngle;

        public LeftArcGauge()
        {
            this.InitializeComponent();
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            _needsResourceRecreation = true;
        }

        protected void TitleControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;
            Vector2 at = new Vector2((float)sender.ActualWidth/2F, (float)sender.ActualHeight);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.TextFontSize,
            })
            {
                ds.DrawText(this.Text, at, this.GaugeColor, textFormat);
            }
        }

        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);

            var ds = args.DrawingSession;
            this.Center = new Vector2((float)(sender.Size.Width / 2f), (float)(sender.Size.Height / 2f));

            this.DrawValueArc(sender, ds);
            this.DrawBaseArc(sender, ds);
            this.DrawLowWarningArc(sender, ds);
            this.DrawLowAlarmArc(sender, ds);
            this.DrawHighWarningArc(sender, ds);
            this.DrawHighAlarmArc(sender, ds);
            this.DrawMinValue(sender, ds);
            this.DrawMaxValue(sender, ds);
            this.DrawPointer(sender, ds);
        }

        protected void valueControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            this.DrawValue(sender, args.DrawingSession);
        }

        protected void unitsControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            this.DrawUnits(sender, args.DrawingSession);
        }

        protected void DrawValue(CanvasControl sender, CanvasDrawingSession ds)
        {
            string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
            float atX = (float)sender.ActualWidth - c_arcThickness;
            float atY = (float)sender.ActualHeight;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.ValueFontSize,
            })
            {
                ds.DrawText(string.Format(format, this.Value), at, this.GaugePointerColor, textFormat);
            }
        }

        protected void DrawUnits(CanvasControl sender, CanvasDrawingSession ds)
        {
            float atX = (float) sender.ActualWidth - c_arcThickness;
            float atY = 0; 
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = (float)this.UnitsFontSize,
            })
            {
                ds.DrawText(this.Units, at, this.GaugePointerColor, textFormat);
            }
        }

        protected void DrawMinValue(CanvasControl sender, CanvasDrawingSession ds)
        {
            string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
            double radian = RadiansFromDegrees(c_startAngle-5);
            float atX = (float)(Math.Cos(radian) * (this.Radius + 1.3*c_arcThickness)) + Center.X;
            float atY = (float)(Math.Sin(radian) * (this.Radius + 1.3*c_arcThickness)) + Center.Y + 5;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = (float)this.LabelsFontSize
            })
            {
                ds.DrawText(string.Format(format, this.MinValue), at, this.GaugeColor, textFormat);
            }
        }

        protected void DrawMaxValue(CanvasControl sender, CanvasDrawingSession ds)
        {
            string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
            double radian = RadiansFromDegrees(c_endAngle);
            float atX = (float)(Math.Cos(radian) * (this.Radius - 1 * c_arcThickness)) + Center.X - 2;
            float atY = (float)(Math.Sin(radian) * (this.Radius - 1 * c_arcThickness)) + Center.Y;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Right,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = (float)this.LabelsFontSize
            })
            {
                ds.DrawText(string.Format(format, this.MaxValue), at, this.GaugeColor, textFormat);
            }
        }

        protected void DrawPointer(CanvasControl sender, CanvasDrawingSession ds)
        {
            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                cp.BeginFigure(this.Center);
                cp.AddLine(new Vector2(this.Center.X - c_needleThickness / 2, this.Center.Y));
                cp.AddLine(new Vector2(this.Center.X - c_needleThickness / 2, this.Center.Y + this.Radius));
                cp.AddLine(new Vector2(this.Center.X + c_needleThickness / 2, this.Center.Y + this.Radius));
                cp.AddLine(new Vector2(this.Center.X + c_needleThickness / 2, this.Center.Y));
                cp.AddLine(new Vector2(this.Center.X - c_needleThickness / 2, this.Center.Y));
                cp.EndFigure(CanvasFigureLoop.Closed);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    float angle = (float)RadiansFromDegrees(this.PercentToPointerAngle(this.PercentFull));
                    ds.Transform *= System.Numerics.Matrix3x2.CreateRotation(angle, this.Center);  
                    ds.FillGeometry(geometry, this.GaugePointerColor);
                }
            }
        }

        protected void DrawValueArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            this.CreateGaugeArcBrush(sender);

            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                cp.BeginFigure(this.Center);
                float startAngle = (float)RadiansFromDegrees(c_startAngle);
                double endDegrees = this.PercentToStartAngle(this.PercentFull);
                float endAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.PercentFull));

                cp.AddArc(this.Center, this.Radius, this.Radius, startAngle, endAngle-startAngle);
                cp.EndFigure(CanvasFigureLoop.Closed);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    ds.FillGeometry(geometry, this.GaugeArcBrush); // Colors.Green);
                }
            }
        }

        /// <summary>
        /// Draw the base arc
        /// </summary>
        protected void DrawBaseArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle   = (float) RadiansFromDegrees(c_endAngle);
            float endAngle     = (float) RadiansFromDegrees(c_startAngle);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, this.GaugeColor, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Large);
        }

        protected void DrawHighWarningArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(c_endAngle);
            float endAngle   = (float)RadiansFromDegrees(this.PercentToEndAngle(this.HighWarningStartPercent));
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Orange, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawHighAlarmArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(c_endAngle);
            float endAngle = (float)RadiansFromDegrees(this.PercentToEndAngle(this.HighAlarmStartPercent));
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Red, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawLowWarningArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.LowWarningEndPercent));
            float endAngle = (float)RadiansFromDegrees(c_startAngle);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Orange, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawLowAlarmArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.LowAlarmEndPercent));
            float endAngle = (float)RadiansFromDegrees(c_startAngle);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Red, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected double PercentToEndAngle(double percent)
        {
            double value = Math.Max(percent, 0D);
            value = Math.Min(value, 1.0D);

            double angle = 360 - (c_arcSweep * (1 - percent));

            return angle;
        }

        protected double PercentToStartAngle(double percent)
        {
            double value = Math.Max(percent, 0D);
            value = Math.Min(value, 1.0D);

            double angle = c_startAngle + (c_arcSweep * percent);

            Debug.Assert(angle <= 360);
            Debug.Assert(angle >= c_startAngle);

            return angle;
        }

        protected double PercentToPointerAngle(double percent)
        {
            double value = Math.Max(percent, 0D);
            value = Math.Min(value, 1.0D);

            double angle = 40D + (c_arcSweep * percent);

            Debug.Assert(angle <= 270);
            Debug.Assert(angle >= 40);

            return angle;
        }

        protected void DrawGaugeArc(CanvasControl sender, CanvasDrawingSession ds, float startAngle, float endAngle, Color color, CanvasSweepDirection canvasSweepDirection, CanvasArcSize canvasArcSize)
        {
            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                var startPoint = this.Center + Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(startAngle)) * this.Radius;
                var endPoint = this.Center + Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(endAngle)) * this.Radius;

                cp.BeginFigure(startPoint);
                cp.AddArc(endPoint, this.Radius, this.Radius, 0, canvasSweepDirection, canvasArcSize);
                cp.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    ds.DrawGeometry(geometry, color, c_arcThickness, this.ArcStrokeStyle);
                }
            }
        }

        protected float Radius
        {
            get
            {
                return Math.Min(this.Center.X, this.Center.Y) - 2*c_arcThickness;
            }
        }

        protected CanvasStrokeStyle ArcStrokeStyle = new CanvasStrokeStyle()
        {
            DashStyle = CanvasDashStyle.Solid,
            StartCap = CanvasCapStyle.Round,
            EndCap = CanvasCapStyle.Round,
        };

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

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            _needsResourceRecreation = false;
        }

        protected CanvasLinearGradientBrush GaugeArcBrush { get; set; }

        protected Vector2 Center { get; set; }

        protected void CreateGaugeArcBrush(CanvasControl sender)
        {
            Color step2Color = this.GaugePointerColor;
            Color step1Color = ColorHelper.FromArgb(step1Color.A,
                                                    Convert.ToByte(~step2Color.R & 0xFF),
                                                    Convert.ToByte(~step2Color.G & 0xFF),
                                                    Convert.ToByte(~step2Color.B & 0xFF));
            if (step2Color == Colors.Red)
            {
                step1Color = Colors.Black;
            }

            var stops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=step1Color, Position = -0.2f },
                new CanvasGradientStop() { Color=step2Color,   Position = 1.0f },
            };

            this.GaugeArcBrush = new CanvasLinearGradientBrush(sender, stops);
            this.GaugeArcBrush.StartPoint = new Vector2(Center.X, Center.Y - this.Radius);
            this.GaugeArcBrush.EndPoint = new Vector2(Center.X, Center.Y + this.Radius);
        }

        override protected void RefreshAlarmColors()
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
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
            this.ValueControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshGaugeWidth(object oldValue, object newValue)
        {
            this.canvasControl.Width = Convert.ToDouble(newValue);
            this.MainGrid.Width = Convert.ToDouble(newValue);
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshMaxValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshMinValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshHighAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshLabelsFontSize(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshValueFontSize(object oldValue, object newValue)
        {
            this.ValueControl?.Invalidate();
        }

        protected override void RefreshUnitsFontSize(object oldValue, object newValue)
        {
            this.UnitsControl?.Invalidate();
        }

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        static protected double RadiansFromDegrees(double degrees)
        {
            return degrees * Math.PI / 180D;
        }
    }
}
