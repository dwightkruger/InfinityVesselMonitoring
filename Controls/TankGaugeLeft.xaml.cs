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
    public sealed partial class TankGaugeLeft : TankGaugeBase
    {
        public TankGaugeLeft()
        {
            this.InitializeComponent();
            this.CanvasControl = canvasControl;

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

        override protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            this.IsLoaded = false;

            if ((0 == sender.Size.Width) || (0 == sender.Size.Height))
            {
                this.OuterRectangle = new Rect();
            }
            else
            {
                this.OuterRectangle = new Rect(sender.Size.Width * (1f / 3f) + 8, 16f,
                                               sender.Size.Width * (1f / 3f) - 8, sender.Size.Height - 32);
            }

            this.CreateTankBrushes(sender);

            IsLoaded = true;
            this.CanvasControl.Invalidate();
        }

        override protected void DrawTics(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);
            double delta = (this.MaxValue - this.MinValue) / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float X1 = (float)this.OuterRectangle.X - c_outerRectangleThickness;
                float X2 = (float)this.OuterRectangle.X - ticLength - c_outerRectangleThickness;
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

            double increment = OuterRectangle.Height / (totalTics - 1);
            double valueIncrement = (MaxValue - MinValue) / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float X = (float)OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness;
                float Y = (float)(OuterRectangle.Y + (i * increment));

                Vector2 at = new Vector2(X, Y);

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Right,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)this.LabelsFontSize,
                })
                {
                    string format = "{0:F" + string.Format("{0:F0}", Resolution) + "}";
                    ds.DrawText(string.Format(format, MinValue + (MaxValue - (i * valueIncrement))), at, this.GaugeColor, textFormat);
                }
            }
        }

        override protected void RefreshAlarmColors()
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshMaxValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshMinValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshHighAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
        {
            this.CanvasControl.Invalidate();
        }

        override protected void RefreshGaugeColor(object oldValue, object newValue)
        {
            this.TitleControl?.Invalidate();
            this.CanvasControl?.Invalidate();
        }
    }
}
