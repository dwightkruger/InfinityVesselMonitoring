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
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class HorizontalBarGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        private const float c_boxThickness = 10;
        private const float c_boxHeight = 3* c_boxThickness;

        public HorizontalBarGauge()
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

            Vector2 at = new Vector2((float)sender.ActualWidth/2, (float)sender.ActualHeight);

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
            CanvasDrawingSession ds = args.DrawingSession;

            Rect leftRect   = new Rect(0,                                     sender.ActualHeight - c_boxHeight,    c_boxThickness,     c_boxHeight);
            Rect bottomRect = new Rect(0,                                     sender.ActualHeight - c_boxThickness, sender.ActualWidth, c_boxThickness);
            Rect rightRect  = new Rect(sender.ActualWidth/2 - c_boxThickness, sender.ActualHeight - c_boxHeight,    c_boxThickness,     c_boxHeight);
            Rect fillRect   = new Rect(
                c_boxThickness,                                                 // X
                sender.ActualHeight - 2*c_boxThickness,                         // Y
                (sender.ActualWidth/2 - 2*c_boxThickness) * this.PercentFull,   // Width
                c_boxThickness);                                                // Height

            ds.FillRectangle(leftRect, this.GaugeColor);
            ds.FillRectangle(bottomRect, this.GaugeColor);
            ds.FillRectangle(rightRect, this.GaugeColor);
            ds.FillRectangle(fillRect, Colors.Blue);

            using (CanvasPathBuilder cp = new CanvasPathBuilder(sender))
            {
                float left = c_boxThickness/2;
                float top  = (float)sender.ActualHeight - 3*c_boxThickness;
                cp.BeginFigure(new Vector2(left,top));

                cp.AddLine(new Vector2(left + c_boxThickness/2, top + c_boxThickness));
                cp.AddLine(new Vector2(left + c_boxThickness,   top));
                cp.AddLine(new Vector2(left, top));

                cp.EndFigure(CanvasFigureLoop.Closed);

                using (var geometry = CanvasGeometry.CreatePath(cp))
                {
                    Vector2 at = new Vector2((float)fillRect.Width, 0);
                    ds.Transform *= System.Numerics.Matrix3x2.CreateTranslation(at);
                    ds.FillGeometry(geometry, Colors.Blue);
                }
            }
        }

        protected void valueControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;
            Rect unitsBoundingRectangle;

            // Draw the units, and then above it draw the value
            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.UnitsFontSize * 0.8F,
            })
            {
                Vector2 at = new Vector2(12, (float)sender.ActualHeight);
                ds.DrawText(this.Units, at, this.GaugeColor, textFormat);
                unitsBoundingRectangle = Utilities.CalculateStringBoundingRectangle(sender, args, this.Units, textFormat);
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.ValueFontSize * 0.8F,
            })
            {
                Vector2 at = new Vector2(
                    12, 
                    (float)(sender.ActualHeight - unitsBoundingRectangle.Height));
                string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";

                ds.DrawText(string.Format(format, this.Value), at, this.GaugeColor, textFormat);
            }
        }

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            _needsResourceRecreation = false;
        }

        protected CanvasStrokeStyle ArcStrokeStyle = new CanvasStrokeStyle()
        {
            DashStyle = CanvasDashStyle.Solid,
            StartCap = CanvasCapStyle.Flat,
            EndCap = CanvasCapStyle.Flat,
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

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        override protected void RefreshAlarmColors()
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
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
        }

        override protected void RefreshGaugeWidth(object oldValue, object newValue)
        {
            this.canvasControl.Width = Convert.ToDouble(newValue);
            this.MainGrid.Width = Convert.ToDouble(newValue);
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
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
            this.ValueControl?.Invalidate();
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();
        }
    }
}
