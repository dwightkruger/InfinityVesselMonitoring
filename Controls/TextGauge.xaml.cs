﻿//////////////////////////////////////////////////////////////////////////////////////////////////////
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
    public partial class TextGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;

        public TextGauge()
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

            Rect unitsBoundingRectangle;

            // Draw the units, and then draw the value above it
            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.UnitsFontSize,
            })
            {
                Vector2 at = new Vector2(
                    (float)(sender.ActualWidth / 2F),
                    (float)sender.ActualHeight);
                ds.DrawText(this.Units, at, this.GaugePointerColor, textFormat);
                unitsBoundingRectangle = Utilities.CalculateStringBoundingRectangle(sender, args, this.Units, textFormat);
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Bottom,
                FontSize = (float)this.ValueFontSize,
            })
            {
                Vector2 at = new Vector2(
                    (float)(sender.ActualWidth / 2F),
                    (float)(sender.ActualHeight - unitsBoundingRectangle.Height));
                string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";

                ds.DrawText(string.Format(format, this.Value), at, this.GaugePointerColor, textFormat);
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
            this.TitleControl?.Invalidate();
            this.canvasControl?.Invalidate();
        }
    }
}
