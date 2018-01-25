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
        private const float c_boxThickness = 2;

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
    }
}
