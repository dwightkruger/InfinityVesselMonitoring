//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class PieChartGauge : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        private ChartRenderer _chartRenderer;

        public PieChartGauge()
        {
            this.InitializeComponent();
            _chartRenderer = new ChartRenderer();
            this.SensorCollection = new ObservableCollection<ISensorItem>();
            Globals.Globals.SensorValueMessenger.Register<ISensorItem>(this, (sensorItem) =>
            {
                if (this.SensorCollection.Contains(sensorItem))
                {
                    this.TitleControl?.Invalidate();
                    this.canvasControl?.Invalidate();
                }
            });
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void TitleControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            Vector2 at = new Vector2((float)sender.ActualWidth / 2, (float)sender.ActualHeight);

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


        private readonly Color[] _colorVector = new Color[3] { Colors.Cyan, Colors.Green, Colors.Blue };
        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {

            this.EnsureResources(sender, args);
            CanvasDrawingSession ds = args.DrawingSession;

            // The Piechart has N slices; one for each sensor.
            var pieValues = new List<double>(this.SensorCollection.Count + 1);
            var pieLabels = new List<string>(this.SensorCollection.Count + 1);
            var pallet    = new List<Color>(this.SensorCollection.Count + 1);

            for (int i= 0; i < this.SensorCollection.Count; i++)
            {
                pieValues.Add(this.SensorCollection[i].PercentFull);
                pieLabels.Add(this.SensorCollection[i].Name);
                pallet.Add(_colorVector[i % _colorVector.Count()]);
            }
            
            pieValues.Add(pieValues.Count - pieValues.Sum());
            pieLabels.Add("% remaining");
            pallet.Add(Colors.Transparent);

            _chartRenderer.RenderAveragesAsPieChart(
                sender,
                args,
                pieValues,
                pieLabels,
                this.GaugeColor,
                pallet
            );
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

        public ObservableCollection<ISensorItem> SensorCollection { get; set; }

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
