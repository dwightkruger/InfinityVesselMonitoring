//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;

namespace InfinityGroup.VesselMonitoring.Controls
{
    public enum TankType
    {
        Unknown = 0,
        OilTank = 1,
        DieselTank = 2,
        SeawaterTank = 3,
        FreshwaterTank = 4,
        GreywaterTank = 5,
        BlackwaterTank = 6,
        HydraulicFluidTank = 7
    }

    public partial class TankGaugeBase : BaseGauge
    {
        protected const float outerRectangleThickness = 5;
        protected TankType _tankType;

        protected int totalMajorTics;

        protected const float majorTicLength = 12;
        protected const float mediumTicLength = 6;
        protected const float minorTicLength = 3;
        protected const float ticWidth = 2;
        protected int totalMediumTics;
        protected int totalMinorTics;

        public TankGaugeBase()
        {
            Divisions = 10;
            this.TankType = TankType.Unknown;
        }

        virtual protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            totalMajorTics = Divisions + 1;
            totalMediumTics = (totalMajorTics * MediumTicsPerMajorTic) - MediumTicsPerMajorTic + 1;
            totalMinorTics = (totalMajorTics * MinorTicsPerMajorTic) - MinorTicsPerMajorTic + 1;

            DrawOuterBox(sender, args);
            DrawInnerBox(args);

            if (IsLowWarningEnabled) DrawAlarmLow(args, LowWarningColor, LowWarningValue);
            if (IsLowAlarmEnabled) DrawAlarmLow(args, LowAlarmColor, LowAlarmValue);

            if (IsHighWarningEnabled) DrawAlarmHigh(args, HighWarningColor, HighWarningValue);
            if (IsHighAlarmEnabled) DrawAlarmHigh(args, HighAlarmColor, HighAlarmValue);

            DrawTics(args, totalMajorTics, majorTicLength);
            DrawTics(args, totalMediumTics, mediumTicLength);
            DrawTics(args, totalMinorTics, minorTicLength);
            DrawScale(args, totalMajorTics, majorTicLength);
        }

        virtual protected void DrawAlarmLow(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(majorTicLength);

            float X = (float)(OuterRectangle.X - majorTicLength - outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y + OuterRectangle.Height - height - outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        virtual protected void DrawAlarmHigh(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = 1 - (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(majorTicLength);

            float X = (float)(OuterRectangle.X - majorTicLength - outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        virtual protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        virtual protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            IsLoaded = false;

            OuterRectangle = new Rect(sender.Size.Width * (2f / 3f), 16f,
                                      sender.Size.Width * (1f / 3f) - 8, sender.Size.Height - 32);
            //args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());

            CreateTankBrushes(sender);

            IsLoaded = true;
            CanvasControl.Invalidate();
        }

        //virtual protected async Task CreateResourcesAsync(CanvasControl sender)
        //{
        //}

        virtual protected void DrawOuterBox(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            Color edgeColor = this.GaugeColor;

            if (IsLowAlarmEnabled && (Value <= LowAlarmValue) && IsOnline)
            {
                edgeColor = LowAlarmColor;
            }
            else if (IsLowWarningEnabled && (Value <= LowWarningValue) && IsOnline)
            {
                edgeColor = LowWarningColor;
            }
            else if (IsHighAlarmEnabled && (Value >= HighAlarmValue) && IsOnline)
            {
                edgeColor = HighAlarmColor;
            }
            else if (IsHighWarningEnabled && (Value >= HighWarningValue) && IsOnline)
            {
                edgeColor = HighWarningColor;
            }

            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                clds.DrawRectangle(OuterRectangle, edgeColor, outerRectangleThickness);
            }

            GaussianBlurEffect blur = new GaussianBlurEffect();
            blur.Source = cl;
            blur.BlurAmount = 3.0f;
            blur.BorderMode = EffectBorderMode.Soft;
            args.DrawingSession.DrawImage(blur);
        }

        virtual protected void DrawInnerBox(CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            double value = Value;
            value = Math.Min(MaxValue, value);
            value = Math.Max(MinValue, value);

            double percentFull = (value - MinValue) / (MaxValue - MinValue);
            float height = (float)(OuterRectangle.Height * percentFull) - (2f * outerRectangleThickness);
            height = Math.Max(height, 1);

            float width = (float)(OuterRectangle.Width - 2 * outerRectangleThickness);
            float X = (float)(OuterRectangle.X + outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y + OuterRectangle.Height - height - outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, this.TankBrush);
        }

        virtual protected void DrawTics(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float Y = (float)(OuterRectangle.Y + (i * increment));

                Vector2 from = new Vector2((float)OuterRectangle.X - outerRectangleThickness, Y);
                Vector2 to = new Vector2((float)OuterRectangle.X - ticLength - outerRectangleThickness, Y);
                ds.DrawLine(from, to, this.GaugeColor, ticWidth);
            }
        }

        virtual protected void DrawScale(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            var ds = args.DrawingSession;

            double increment = OuterRectangle.Height / (totalTics - 1);
            double valueIncrement = (MaxValue - MinValue) / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                float Y = (float)(OuterRectangle.Y + (i * increment));

                Vector2 at = new Vector2((float)OuterRectangle.X - outerRectangleThickness - 28, Y);

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Right,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)ValueFontSize,
                })
                {
                    string format = "{0:F" + string.Format("{0:F0}", Resolution) + "}";
                    ds.DrawText(string.Format(format, MinValue + (MaxValue - (i * valueIncrement))), at, this.GaugeColor, textFormat);
                }
            }
        }

        CanvasLinearGradientBrush TankBrush
        {
            get
            {
                switch (TankType)
                {
                    case TankType.Unknown: return DieselTankBrush;
                    case TankType.BlackwaterTank: return BlackwaterTankBrush;
                    case TankType.DieselTank: return DieselTankBrush;
                    case TankType.FreshwaterTank: return FreshwaterTankBrush;
                    case TankType.GreywaterTank: return GreywaterTankBrush;
                    case TankType.HydraulicFluidTank: return HydraulicFluidTankBrush;
                    case TankType.OilTank: return OilTankBrush;
                    case TankType.SeawaterTank: return SeawaterTankBrush;

                    default: return DieselTankBrush;
                }
            }
        }

        protected void CreateTankBrushes(CanvasControl sender)
        {
            var oilTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,          Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.Gold,           Position =  1.2f },
            };

            var dieselTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,          Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.Orange,         Position =  1.2f },
            };

            var seawaterTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,           Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.Green,           Position =  1.2f },
            };

            var freshwaterTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,           Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.CornflowerBlue,  Position =  1.2f },
            };

            var greywaterTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,          Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.LightGray,      Position =  1.2f },
            };

            var blackwaterTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,          Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.Gray,           Position =  1.2f },
            };

            var hydraulicFluidTankStops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black,          Position = -0.5f },
                new CanvasGradientStop() { Color=Colors.Magenta,        Position =  1.2f },
            };

            OilTankBrush = new CanvasLinearGradientBrush(sender, oilTankStops);
            OilTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            OilTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            DieselTankBrush = new CanvasLinearGradientBrush(sender, dieselTankStops);
            DieselTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            DieselTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            SeawaterTankBrush = new CanvasLinearGradientBrush(sender, seawaterTankStops);
            SeawaterTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            SeawaterTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            FreshwaterTankBrush = new CanvasLinearGradientBrush(sender, freshwaterTankStops);
            FreshwaterTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            FreshwaterTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            GreywaterTankBrush = new CanvasLinearGradientBrush(sender, greywaterTankStops);
            GreywaterTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            GreywaterTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            BlackwaterTankBrush = new CanvasLinearGradientBrush(sender, blackwaterTankStops);
            BlackwaterTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            BlackwaterTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);

            HydraulicFluidTankBrush = new CanvasLinearGradientBrush(sender, hydraulicFluidTankStops);
            HydraulicFluidTankBrush.StartPoint = new Vector2((float)OuterRectangle.X, 0);
            HydraulicFluidTankBrush.EndPoint = new Vector2((float)(OuterRectangle.X + OuterRectangle.Width), 0);
        }

        override protected void RefreshAlarmColors()
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshMaxValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshMinValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshHighAlarmValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.CanvasControl.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
        {
            this.CanvasControl.Invalidate();
        }

        protected CanvasControl CanvasControl { get; set; }
        protected bool IsLoaded { get; set; }
        protected Rect OuterRectangle { get; set; }
        public CanvasLinearGradientBrush OilTankBrush { get; set; }
        public CanvasLinearGradientBrush DieselTankBrush { get; set; }
        public CanvasLinearGradientBrush SeawaterTankBrush { get; set; }
        public CanvasLinearGradientBrush FreshwaterTankBrush { get; set; }
        public CanvasLinearGradientBrush GreywaterTankBrush { get; set; }
        public CanvasLinearGradientBrush BlackwaterTankBrush { get; set; }
        public CanvasLinearGradientBrush HydraulicFluidTankBrush { get; set; }

        #region TankType
        public static readonly DependencyProperty TankTypeProperty = DependencyProperty.Register(
            "TankType",
            typeof(TankType),
            typeof(TankGaugeBase),
            new PropertyMetadata(TankType.Unknown,
                                  new PropertyChangedCallback(OnTankTypePropertyChanged)));

        public TankType TankType
        {
            get { return (TankType)this.GetValue(TankTypeProperty); }
            set { this.SetValue(TankTypeProperty, value); }
        }

        protected static void OnTankTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TankGaugeBase g = d as TankGaugeBase;
            g?.CanvasControl?.Invalidate();
        }
        #endregion

    }

}
