//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
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

            float percentValue = (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(majorTicLength);

            float X = (float)(OuterRectangle.X - majorTicLength - outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y + OuterRectangle.Height - height - outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        override protected void DrawAlarmHigh(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = 1 - (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(majorTicLength);

            float X = (float)(OuterRectangle.X - majorTicLength - outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        override protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        override protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            IsLoaded = false;

            if ((0 == sender.Size.Width) || (0 == sender.Size.Height))
            {
                OuterRectangle = new Rect();
            }
            else
            {
                OuterRectangle = new Rect(sender.Size.Width * (1f / 3f) + 8, 16f,
                                          sender.Size.Width * (1f / 3f) - 8, sender.Size.Height - 32);
            }

            //args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());

            CreateTankBrushes(sender);

            IsLoaded = true;
            CanvasControl.Invalidate();
        }

        //override protected async Task CreateResourcesAsync(CanvasControl sender)
        //{
        //}

        override protected void DrawTics(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float X1 = (float)OuterRectangle.X - outerRectangleThickness;
                float X2 = (float)OuterRectangle.X - ticLength - outerRectangleThickness;
                float Y = (float)(OuterRectangle.Y + (i * increment));

                Vector2 from = new Vector2(X1, Y);
                Vector2 to = new Vector2(X2, Y);
                ds.DrawLine(from, to, Colors.White, ticWidth);
            }
        }

        override protected void DrawScale(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);
            double valueIncrement = (MaxValue - MinValue) / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float X = (float)OuterRectangle.X - majorTicLength - outerRectangleThickness;
                float Y = (float)(OuterRectangle.Y + (i * increment));

                Vector2 at = new Vector2(X, Y);

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Right,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)ValueFontSize
                })
                {
                    string format = "{0:F" + string.Format("{0:F0}", Resolution) + "}";
                    ds.DrawText(string.Format(format, MinValue + (MaxValue - (i * valueIncrement))), at, Colors.White, textFormat);
                }
            }
        }
    }
}
