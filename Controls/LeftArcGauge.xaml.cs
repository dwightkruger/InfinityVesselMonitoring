//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Geometry;
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
        }

        protected void DrawValueArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            const float arcThickness = 15;
            float radius = Math.Min(this.Center.X, this.Center.Y) - arcThickness;
            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                cp.BeginFigure(this.Center);
                float startAngle = (float)RadiansFromDegrees(130);
                double percentFull = this.PercentFull;
                double endDegrees = this.PercentToStartAngle(this.PercentFull);
                float endAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.PercentFull));
                Debug.Assert(startAngle <= endAngle);
                Debug.Assert(endAngle <= 2.01 * Math.PI);
                Debug.Assert(percentFull <= 1.0D);

                cp.AddArc(this.Center, radius, radius, startAngle, endAngle-startAngle);
                cp.EndFigure(CanvasFigureLoop.Closed);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    ds.FillGeometry(geometry, Colors.Green);
                }
            }
        }

        /// <summary>
        /// Draw the base arc
        /// </summary>
        protected void DrawBaseArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle   = (float) RadiansFromDegrees(0);
            float endAngle     = (float) RadiansFromDegrees(130);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, this.GaugeColor, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Large);
        }

        protected void DrawHighWarningArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(0);
            float endAngle   = (float)RadiansFromDegrees(this.PercentToEndAngle(this.HighWarningStartPercent));
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Orange, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawHighAlarmArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(0);
            float endAngle = (float)RadiansFromDegrees(this.PercentToEndAngle(this.HighAlarmStartPercent));
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Red, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawLowWarningArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.LowWarningEndPercent));
            float endAngle = (float)RadiansFromDegrees(130);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Orange, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected void DrawLowAlarmArc(CanvasControl sender, CanvasDrawingSession ds)
        {
            float startAngle = (float)RadiansFromDegrees(this.PercentToStartAngle(this.LowAlarmEndPercent));
            float endAngle = (float)RadiansFromDegrees(130);
            this.DrawGaugeArc(sender, ds, startAngle, endAngle, Colors.Red, CanvasSweepDirection.CounterClockwise, CanvasArcSize.Small);
        }

        protected double PercentToEndAngle(double percent)
        {
            double value = Math.Max(percent, 0D);
            value = Math.Min(value, 1.0D);

            double delta = 230;     // 230 degrees for the arc total
            double angle = 360 - (delta * (1 - percent));

            return angle;
        }
        protected double PercentToStartAngle(double percent)
        {
            double value = Math.Max(percent, 0D);
            value = Math.Min(value, 1.0D);

            double delta = 230;     // 230 degrees for the arc total
            double angle = 130D + (delta * percent);

            Debug.Assert(angle <= 360);
            Debug.Assert(angle >= 130);

            return angle;
        }

        protected void DrawGaugeArc(CanvasControl sender, CanvasDrawingSession ds, float startAngle, float endAngle, Color color, CanvasSweepDirection canvasSweepDirection, CanvasArcSize canvasArcSize)
        {
            const float arcThickness = 15;
            float radius = Math.Min(this.Center.X, this.Center.Y) - arcThickness;

            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                var startPoint = this.Center + Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(startAngle)) * radius;
                var endPoint = this.Center + Vector2.Transform(Vector2.UnitX, Matrix3x2.CreateRotation(endAngle)) * radius;

                cp.BeginFigure(startPoint);
                cp.AddArc(endPoint, radius, radius, 0, canvasSweepDirection, canvasArcSize);
                cp.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    ds.DrawGeometry(geometry, color, arcThickness, this.ArcStrokeStyle);
                }
            }
        }

        protected CanvasStrokeStyle ArcStrokeStyle = new CanvasStrokeStyle()
        {
            DashStyle = CanvasDashStyle.Solid,
            StartCap = CanvasCapStyle.Flat,
            EndCap = CanvasCapStyle.Flat,
        };

        protected Color PointerColor
        {
            get
            {
                Color result = Colors.Green;

                if (IsLowAlarmEnabled && (Value <= LowAlarmValue))
                {
                    result = LowAlarmColor;
                }
                else if (IsLowWarningEnabled && (Value <= LowWarningValue))
                {
                    result = LowWarningColor;
                }
                else if (IsHighAlarmEnabled && (Value >= HighAlarmValue))
                {
                    result = HighAlarmColor;
                }
                else if (IsHighWarningEnabled && (Value >= HighWarningValue))
                {
                    result = HighWarningColor;
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

        protected CanvasLinearGradientBrush InnerCircleBrush { get; set; }

        protected Vector2 Center { get; set; }
        protected CanvasStrokeStyle PointerStrokeStyle { get; set; }

        override protected void RefreshAlarmColors()
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
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
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshGaugeWidth(object oldValue, object newValue)
        {
            this.canvasControl.Width = Convert.ToDouble(newValue);
            this.MainGrid.Width = Convert.ToDouble(newValue);
            this.canvasControl?.Invalidate();
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

            this.canvasControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshValueFontSize(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        protected override void RefreshUnitsFontSize(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        static protected double RadiansFromDegrees(double degrees)
        {
            return degrees * Math.PI / 180D;
        }
    }
}
