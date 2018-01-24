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
    public partial class VerticalBarGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        private const float c_boxThickness = 2;
        private const float c_gaugeWidth = 75;
        private float _gaugeGridWidth  = 0;
        private float _gaugeGridHeight = 0;

        public VerticalBarGauge()
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

            float xRight = (float)(sender.ActualWidth * _gaugeGridWidth);
            float center = xRight - (c_gaugeWidth / 2);
            Vector2 at = new Vector2(center, 4);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Top,
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

            float height = (float)(sender.ActualHeight * _gaugeGridHeight);
            float yTop = (float)(sender.ActualHeight - height) / 2;
            float xRight = (float)(sender.ActualWidth * _gaugeGridWidth);
            float xLeft = xRight - c_gaugeWidth;

            Rect rect = new Rect(xLeft, yTop, c_gaugeWidth, height);
            ds.DrawRectangle(rect, this.GaugePointerColor, c_boxThickness);

            xLeft += 2 * c_boxThickness;
            height -= 4 * c_boxThickness;
            yTop += 2 * c_boxThickness;
            float increment = (float)(height / (this.MaxValue - this.MinValue));

            yTop = (float)(yTop + height - ((this.Value - this.MinValue) * increment));
            height = (float)(height * this.PercentFull);

            height = Math.Max(1, height);

            rect = new Rect(xLeft, yTop, c_gaugeWidth - 4*c_boxThickness, height);
            ds.FillRectangle(rect, this.GaugePointerColor);
        }

        protected void valueControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            float height = (float)(sender.ActualHeight * _gaugeGridHeight);
            float yTop = (float)(sender.ActualHeight - height) / 2;
            float increment = (float) (height / (this.MaxValue - this.MinValue));

            float atX = 10;
            float atY = (float)(yTop + height - ((this.Value - this.MinValue) * increment));

            Vector2 atPointer = new Vector2(atX, atY);

            Rect pointerBoundingRectangle;
            Rect valueBoundingRectangle;

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = 32, // (float)this.ValueFontSize,
            })
            {
                ds.DrawText("◄", atPointer, this.GaugePointerColor, textFormat);
                pointerBoundingRectangle = Utilities.CalculateStringBoundingRectangle(sender, args, "◄", textFormat);
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = (float)this.ValueFontSize,
            })
            {
                string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";
                Vector2 at = new Vector2(
                    atPointer.X + (float)pointerBoundingRectangle.Width + 22, 
                    atPointer.Y);

                ds.DrawText(string.Format(format, this.Value), at, this.GaugePointerColor, textFormat);
                valueBoundingRectangle = Utilities.CalculateStringBoundingRectangle(sender, args, string.Format(format, this.Value), textFormat);
            }

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Left,
                VerticalAlignment = CanvasVerticalAlignment.Center,
                FontSize = (float)this.UnitsFontSize,
            })
            {
                Vector2 at = new Vector2(
                    atPointer.X + 12, 
                    atPointer.Y + 
                        (float)pointerBoundingRectangle.Height + 
                        (float)valueBoundingRectangle.Height/2);
                ds.DrawText(this.Units, at, this.GaugePointerColor, textFormat);
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

            // Calculate the percentage of the gauge width the bounding box will occupy
            _gaugeGridWidth = 0;
            for (int i = 0; i < this.MainGrid.ColumnDefinitions.Count; i++)
                _gaugeGridWidth += (float)this.MainGrid.ColumnDefinitions[i].Width.Value;
            _gaugeGridWidth = (float)this.MainGrid.ColumnDefinitions[0].Width.Value / _gaugeGridWidth;

            // Calculate the percentage of the gauge height the bounding box will occupy
            _gaugeGridHeight = 0;
            for (int i = 0; i < this.MainGrid.RowDefinitions.Count; i++)
                _gaugeGridHeight += (float)this.MainGrid.RowDefinitions[i].Height.Value;
            _gaugeGridHeight = (float)this.MainGrid.RowDefinitions[2].Height.Value / _gaugeGridHeight;

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

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
            this.ValueControl?.Invalidate();

        }
    }
}
