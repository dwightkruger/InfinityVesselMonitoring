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
using Windows.UI.Xaml.Controls;

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
        protected const float c_outerRectangleThickness = 5;
        protected TankType _tankType;

        protected int totalMajorTics;

        protected const float c_majorTicLength = 12;
        protected const float c_mediumTicLength = 6;
        protected const float c_minorTicLength = 3;
        protected const float c_ticWidth = 2;
        protected const float c_gaugeBoxWith = 2;
        protected int _totalMediumTics;
        protected int _totalMinorTics;
        private bool _needsResourceRecreation = true;

        public TankGaugeBase()
        {
            this.Divisions = 4;
            this.TankType = TankType.Unknown;
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

        virtual protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            totalMajorTics = this.Divisions + 1;
            _totalMediumTics = (totalMajorTics * MediumTicsPerMajorTic) - MediumTicsPerMajorTic + 1;
            _totalMinorTics = (totalMajorTics * MinorTicsPerMajorTic) - MinorTicsPerMajorTic + 1;

            this.DrawOuterBox(sender, args);
            this.DrawInnerBox(args);

            //if (IsLowWarningEnabled) DrawAlarmLow(args, LowWarningColor, LowWarningValue);
            //if (IsLowAlarmEnabled) DrawAlarmLow(args, LowAlarmColor, LowAlarmValue);

            //if (IsHighWarningEnabled) DrawAlarmHigh(args, HighWarningColor, HighWarningValue);
            //if (IsHighAlarmEnabled) DrawAlarmHigh(args, HighAlarmColor, HighAlarmValue);

            this.DrawTics(args, totalMajorTics, c_majorTicLength);
            this.DrawTics(args, _totalMediumTics, c_mediumTicLength);
            this.DrawTics(args, _totalMinorTics, c_minorTicLength);
            this.DrawScale(args, totalMajorTics, c_majorTicLength);
        }

        virtual protected void DrawAlarmLow(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(c_majorTicLength);

            float X = (float)(OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y + OuterRectangle.Height - height - c_outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, alarmColor);
        }

        virtual protected void DrawAlarmHigh(CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = 1 - (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float height = (float)(OuterRectangle.Height * percentValue);
            float width = (float)(c_majorTicLength);

            float X = (float)(OuterRectangle.X - c_majorTicLength - c_outerRectangleThickness);
            float Y = (float)(OuterRectangle.Y);

            Color alarmColorOpacity = alarmColor;
            alarmColorOpacity.A = 0x88;

            ds.FillRectangle(X, Y, width, height, alarmColorOpacity);
        }

        virtual protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        virtual protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            this.IsLoaded = false;

            if ((0 == sender.Size.Width) || (0 == sender.Size.Height))
            {
                this.OuterRectangle = new Rect();
            }
            else
            {
                double topMarginHeight = this.MainGrid.RowDefinitions[0].Height.Value;
                double gaugeAreaHeight = this.MainGrid.RowDefinitions[1].Height.Value;
                double bottomMarginHeight = this.MainGrid.RowDefinitions[2].Height.Value;
                float percentTopMargin = (float)(topMarginHeight / (topMarginHeight + gaugeAreaHeight + bottomMarginHeight));
                float percentGaugeArea = (float)(gaugeAreaHeight / (topMarginHeight + gaugeAreaHeight + bottomMarginHeight));

                this.OuterRectangle = new Rect(sender.Size.Width * (1f / 3f) + 8, sender.Size.Height * percentTopMargin + 10,
                                               sender.Size.Width * (1f / 3f) - 8, sender.Size.Height * percentGaugeArea - 10);
            }

            this.CreateTankBrushes(sender);

            IsLoaded = true;
            this.CanvasControl.Invalidate();
        }

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            _needsResourceRecreation = false;
        }

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

            ds.DrawRectangle(
                (float)this.OuterRectangle.Left, 
                (float)this.OuterRectangle.Top, 
                (float)this.OuterRectangle.Width, 
                (float)this.OuterRectangle.Height,
                this.GaugeColor, 
                c_gaugeBoxWith);
        }

        virtual protected void DrawInnerBox(CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            double value = Value;
            value = Math.Min(MaxValue, value);
            value = Math.Max(MinValue, value);

            double percentFull = (value - MinValue) / (MaxValue - MinValue);
            float height = (float)(this.OuterRectangle.Height * percentFull) - (2f * c_outerRectangleThickness);
            height = Math.Max(height, 1);

            float width = (float)(OuterRectangle.Width - 2 * c_outerRectangleThickness);
            float X = (float)(this.OuterRectangle.X + c_outerRectangleThickness);
            float Y = (float)(this.OuterRectangle.Y + this.OuterRectangle.Height - height - c_outerRectangleThickness);

            ds.FillRectangle(X, Y, width, height, this.TankBrush);
        }

        virtual protected void DrawTics(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        virtual protected void DrawScale(CanvasDrawEventArgs args, int totalTics, float ticLength)
        {
            throw new NotImplementedException("Implement in parent class");
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


        protected CanvasControl CanvasControl { get; set; }
        protected Grid MainGrid { get; set; }
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
