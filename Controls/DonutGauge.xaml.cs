//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

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
    public partial class DonutGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        private const float c_arcThickness = 14;

        public DonutGauge()
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
            Vector2 at = new Vector2((float)sender.ActualWidth / 2F, (float)sender.ActualHeight);

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

            ds.DrawCircle(this.Center, this.Radius + c_arcThickness/2F, this.GaugePointerColor, 1);
            ds.DrawCircle(this.Center, this.Radius - c_arcThickness/2F, this.GaugePointerColor, 1);

            float startAngle = 0;

            double value = Math.Min(this.MaxValue, this.Value);
            double increment = 360D / (this.MaxValue - this.MinValue);
            float endAngle = (float) ((value - this.MinValue) * increment);
            endAngle = Math.Max(endAngle, 1);
            endAngle = Math.Min(endAngle, 359.99F);

            if (endAngle <= 180)
                this.DrawGaugeArc(sender, ds, RadiansFromDegrees(startAngle), RadiansFromDegrees(endAngle), this.GaugePointerColor, CanvasSweepDirection.Clockwise, CanvasArcSize.Small);
            else
                this.DrawGaugeArc(sender, ds, RadiansFromDegrees(startAngle), RadiansFromDegrees(endAngle), this.GaugePointerColor, CanvasSweepDirection.Clockwise, CanvasArcSize.Large);
        }

        protected void valueControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
            float atX = (float)sender.ActualWidth / 2;
            float atY = (float)sender.ActualHeight;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.ValueFontSize,
            })
            {
                ds.DrawText(string.Format(format, this.Value), at, this.GaugePointerColor, textFormat);
            }
        }

        protected void unitsControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            float atX = (float)sender.ActualWidth / 2;
            float atY = 0;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = (float)this.UnitsFontSize,
            })
            {
                ds.DrawText(this.Units, at, this.GaugePointerColor, textFormat);
            }
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

        protected Vector2 Center { get; set; }
        
        protected float Radius
        {
            get
            {
                return Math.Min(this.Center.X, this.Center.Y) - 1 * c_arcThickness;
            }
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
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        static protected float RadiansFromDegrees(double degrees)
        {
            return (float)(degrees * Math.PI / 180F);
        }
    }
}
