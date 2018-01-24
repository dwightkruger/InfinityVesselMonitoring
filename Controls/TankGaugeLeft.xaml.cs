//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class TankGaugeLeft : TankGaugeBase
    {
        public TankGaugeLeft()
        {
            this.InitializeComponent();
            this.CanvasControl = canvasControl;
            this.MainGrid = mainGrid;

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                DesignGrid.Visibility = Visibility.Collapsed;
                DesignRectangle.Visibility = Visibility.Collapsed;
            }
        }

        override protected void DrawAlarmLow(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = (float)((alarmValue - this.MinValue) / (this.MaxValue - this.MinValue));

            float height = (float)(this.OuterRectangle.Height * percentValue);
            float width = (float)(c_majorTicLength);

            float X = (float)(this.OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness);
            float Y = (float)(this.OuterRectangle.Y + this.OuterRectangle.Height - height - c_outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        override protected void DrawAlarmHigh(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = 1 - (float)((alarmValue - this.MinValue) / (this.MaxValue - this.MinValue));

            float height = (float)(this.OuterRectangle.Height * percentValue);
            float width = (float)(c_majorTicLength);

            float X = (float)(this.OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness);
            float Y = (float)(this.OuterRectangle.Y);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        override protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        override protected void DrawTics(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);
            double delta = (this.MaxValue - this.MinValue) / (totalTics - 1);
            float X1 = (float)this.OuterRectangle.X - c_outerRectangleThickness;
            float X2 = (float)this.OuterRectangle.X - ticLength - c_outerRectangleThickness;

            for (int i = 0; i < totalTics; i++)
            {
                float Y = (float)(this.OuterRectangle.Y + (i * increment));

                Vector2 from = new Vector2(X1, Y);
                Vector2 to = new Vector2(X2, Y);

                // Set the tic color to reflect the alarm/wanring values
                Color ticColor = this.GaugeColor;
                if (i * delta <= this.LowAlarmValue) ticColor = this.LowAlarmColor;
                else if (i * delta <= this.LowWarningValue) ticColor = this.LowWarningColor;
                else if (i * delta >= this.HighAlarmValue) ticColor = this.HighAlarmColor;
                else if (i * delta >= this.HighWarningValue) ticColor = this.HighWarningColor;

                ds.DrawLine(from, to, ticColor, c_ticWidth);
            }
        }

        override protected void DrawScale(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double yIncrement = this.OuterRectangle.Height / (totalTics - 1);
            double valueIncrement = (this.MaxValue - this.MinValue) / (totalTics - 1);
            float X = (float)this.OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness;
            string format = "{0:F" + string.Format("{0:F0}", this.Resolution) + "}";

            for (int i = 0; i < totalTics; i++)
            {
                float Y = (float)(this.OuterRectangle.Y + (i * yIncrement));

                Vector2 at = new Vector2(X, Y);

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Right,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)this.LabelsFontSize,
                })
                {
                    ds.DrawText(string.Format(format, this.MaxValue - (i * valueIncrement)), at, this.GaugeColor, textFormat);
                }
            }
        }

        protected void unitsControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            float atX = (float)sender.ActualWidth/2F;
            float atY = (float)0;
            Vector2 at = new Vector2(atX, atY);

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = (float)this.UnitsFontSize,
            })
            {
                ds.DrawText(this.Units, at, this.GaugeColor, textFormat);
            }
        }

        override protected void RefreshAlarmColors()
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshMaxValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshMinValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshHighAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl.Invalidate();
            this.UnitsControl?.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
        {
            this.CanvasControl.Invalidate();
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
            this.UnitsControl?.Invalidate();
        }
    }
}
