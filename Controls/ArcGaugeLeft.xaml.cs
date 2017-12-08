//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
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
    public partial class ArcGaugeLeft : BaseGauge
    {
        private bool _needsResourceRecreation = true;
        protected const float skip = 5.5f;
        protected const float GaugeStartAngle = 360f * skip / 14f;
        protected const float GaugeEndAngle = (360f / 14f * 10f) + (360f * skip / 14f);

        protected int totalMajorTics;

        protected int outerCircleThickness = 10;
        protected float ticWidth = 2;
        protected int totalMediumTics;
        protected int totalMinorTics;

        public ArcGaugeLeft() : base()
        {
            this.InitializeComponent();
            this.GaugeHeight = 400;
            this.GaugeWidth = 400;
            this.ValueFontSize = 14;
            this.UnitsFontSize = 14;

            Divisions = 7;
            outerCircleThickness = 2;

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                DesignEllipse.Visibility = Visibility.Collapsed;
            }
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            _needsResourceRecreation = true;
        }

        //async Task CreateResourcesAsync(CanvasControl sender)
        //{
        //    //try
        //    //{
        //    //    BatteryBitmap = await CanvasBitmap.LoadAsync(sender, @"Graphics/battery-128x128.png");
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Telemetry.TrackException(ex);
        //    //    Debug.WriteLine(ex.Message);
        //    //}
        //}

        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            this.EnsureResources(sender, args);

            var ds = args.DrawingSession;

            Center = new Vector2((float)(sender.Size.Height / 2f), (float)(sender.Size.Width / 2f));

            // BUGBUG create only if dirty
            CreateInnerCircleBrush(sender);

            totalMajorTics = Divisions + 1;
            totalMediumTics = (totalMajorTics * MediumTicsPerMajorTic) - MediumTicsPerMajorTic + 1;
            totalMinorTics = (totalMajorTics * MinorTicsPerMajorTic) - MinorTicsPerMajorTic + 1;

            if (GaugeOutlineVisibility == Visibility.Visible) DrawOuterCircle(sender, args);

            ds.FillCircle(Center, InnerCircleRadius, InnerCircleBrush);

            if (IsLowWarningEnabled) DrawAlarmLowArc(sender, args, LowWarningBrush.Color, LowWarningValue);
            if (IsLowAlarmEnabled) DrawAlarmLowArc(sender, args, LowAlarmBrush.Color, LowAlarmValue);

            if (IsHighWarningEnabled) DrawAlarmHighArc(sender, args, HighWarningBrush.Color, HighWarningValue);
            if (IsHighAlarmEnabled) DrawAlarmHighArc(sender, args, HighAlarmBrush.Color, HighAlarmValue);

            DrawTicks(args, totalMajorTics, (float)MajorTicLength, MiddleCircleRadius);    // Major tics
            DrawTicks(args, totalMediumTics, (float)MediumTicLength, MiddleCircleRadius);  // Medium tics
            DrawTicks(args, totalMinorTics, (float)MinorTicLength, MiddleCircleRadius);    // Minor tics

            DrawLabels(args, totalMajorTics, (float)MajorTicLength, MiddleCircleRadius);
            if (!double.IsNaN(Value) && IsOnline) DrawValuePointer(args, PointerLength, InnerCircleRadius);
            DrawUnits(sender, args, Units);

            //Vector2 bitmapSize = BatteryBitmap.Size.ToVector2();
            //ds.DrawImage(BatteryBitmap, (Center - bitmapSize / 2));
        }

        protected void DrawOuterCircle(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            // # FF144960
            Color edgeColor = Colors.CornflowerBlue;

            if (IsLowAlarmEnabled && (Value <= LowAlarmValue) && IsOnline)
            {
                edgeColor = LowAlarmBrush.Color;
            }
            else if (IsLowWarningEnabled && (Value <= LowWarningValue) && IsOnline)
            {
                edgeColor = LowWarningBrush.Color;
            }
            else if (IsHighAlarmEnabled && (Value >= HighAlarmValue) && IsOnline)
            {
                edgeColor = HighAlarmBrush.Color;
            }
            else if (IsHighWarningEnabled && (Value >= HighWarningValue) && IsOnline)
            {
                edgeColor = HighWarningBrush.Color;
            }

            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                clds.DrawCircle(Center, OuterCircleRadius, edgeColor, outerCircleThickness);
            }

            GaussianBlurEffect blur = new GaussianBlurEffect();
            blur.Source = cl;
            blur.BlurAmount = 3.0f;
            args.DrawingSession.DrawImage(blur);
        }

        protected void DrawTicks(CanvasDrawEventArgs args, int totalTics, float ticLength, float radius)
        {
            var ds = args.DrawingSession;

            double degrees = GaugeStartAngle;
            double increment = (GaugeEndAngle - GaugeStartAngle) / (totalTics - 1);

            for (int i = 0; i < totalTics; i++)
            {
                double radian = degrees * Math.PI / 180D;

                float fromX = (float)(Math.Cos(radian) * (radius + 2)) + Center.X;
                float fromY = (float)(Math.Sin(radian) * (radius + 2)) + Center.Y;
                Vector2 from = new Vector2(fromX, fromY);

                float toX = (float)(Math.Cos(radian) * (radius + ticLength)) + Center.X;
                float toY = (float)(Math.Sin(radian) * (radius + ticLength)) + Center.Y;
                Vector2 to = new Vector2(toX, toY);

                ds.DrawLine(from, to, Colors.White, ticWidth);

                degrees += increment;
            }
        }

        protected void DrawLabels(CanvasDrawEventArgs args, int totalTics, float ticLength, float radius)
        {
            var ds = args.DrawingSession;

            double degrees = GaugeStartAngle;
            double increment = (GaugeEndAngle - GaugeStartAngle) / (totalTics - 1);

            double valueIncrement = (MaxValue - MinValue) / (totalTics - 1);
            double offset = MajorTicLength;

            for (int i = 0; i < totalTics; i++)
            {
                double radian = degrees * Math.PI / 180D;

                float atX = (float)(Math.Cos(radian) * (radius + ticLength + offset)) + Center.X;
                float atY = (float)(Math.Sin(radian) * (radius + ticLength + offset)) + Center.Y;
                Vector2 at = new Vector2(atX, atY);

                using (var textFormat = new CanvasTextFormat()
                {
                    HorizontalAlignment = CanvasHorizontalAlignment.Center,
                    VerticalAlignment = CanvasVerticalAlignment.Center,
                    FontSize = (float)ValueFontSize
                })
                {
                    string format = "{0:F" + string.Format("{0:F0}", Resolution) + "}";
                    ds.DrawText(string.Format(format, MinValue + (i * valueIncrement)), at, Colors.White, textFormat);
                }

                degrees += increment;
            }
        }

        protected void DrawValuePointer(CanvasDrawEventArgs args, float ticLength, float radius)
        {
            double value = Value;
            value = Math.Min(MaxValue, value);
            value = Math.Max(MinValue, value);

            var ds = args.DrawingSession;

            double percentValue = (value - MinValue) / (MaxValue - MinValue);

            float percentIncrement = GaugeEndAngle - GaugeStartAngle;
            double degrees = GaugeStartAngle + (percentIncrement * percentValue);
            double radian = degrees * Math.PI / 180D;

            float fromX = (float)(Math.Cos(radian) * (radius + 2)) + Center.X;
            float fromY = (float)(Math.Sin(radian) * (radius + 2)) + Center.Y;
            Vector2 from = new Vector2(fromX, fromY);

            float toX = (float)(Math.Cos(radian) * (radius + ticLength)) + Center.X;
            float toY = (float)(Math.Sin(radian) * (radius + ticLength)) + Center.Y;
            Vector2 to = new Vector2(toX, toY);

            ds.DrawLine(from, to, PointerColor, 4 * ticWidth, PointerStrokeStyle);
        }

        protected void DrawUnits(CanvasControl sender, CanvasDrawEventArgs args, string units)
        {
            var ds = args.DrawingSession;

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = CanvasHorizontalAlignment.Center,
                VerticalAlignment = CanvasVerticalAlignment.Top,
                FontSize = (float)UnitsFontSize,
            })
            {
                Rect bounds = Utilities.CalculateStringBoundingRectangle(sender, args, units, textFormat);

                float atX = Center.X;
                float atY = Center.Y + InnerCircleRadius + ((float)(bounds.Height / 2));
                Vector2 at = new Vector2(atX, atY);

                ds.DrawText(units, at, Colors.White, textFormat);
            }
        }

        protected CanvasStrokeStyle arcStrokeStyle = new CanvasStrokeStyle()
        {
            StartCap = CanvasCapStyle.Flat,
            EndCap = CanvasCapStyle.Flat,
        };

        protected Color PointerColor
        {
            get
            {
                Color result = Colors.Green;

                if (IsLowAlarmEnabled && (Value <= LowAlarmValue))
                {
                    result = LowAlarmBrush.Color;
                }
                else if (IsLowWarningEnabled && (Value <= LowWarningValue))
                {
                    result = LowWarningBrush.Color;
                }
                else if (IsHighAlarmEnabled && (Value >= HighAlarmValue))
                {
                    result = HighAlarmBrush.Color;
                }
                else if (IsHighWarningEnabled && (Value >= HighWarningValue))
                {
                    result = HighWarningBrush.Color;
                }

                return result;
            }
        }

        virtual protected void DrawAlarmLowArc(CanvasControl sender, CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = (float)((alarmValue - MinValue) / (MaxValue - MinValue));
            float range = GaugeEndAngle - GaugeStartAngle;
            float degrees = GaugeStartAngle + (range * percentValue);

            float arcSweepAngle = degrees - GaugeStartAngle;

            float startAngle = (float)(GaugeStartAngle * Math.PI / 180D);
            float sweepAngle = (float)(arcSweepAngle * Math.PI / 180D);

            double radian = GaugeStartAngle * Math.PI / 180D;
            float startX = (float)(Math.Cos(radian) * MiddleCircleRadius) + Center.X;
            float startY = (float)(Math.Sin(radian) * MiddleCircleRadius) + Center.Y;
            Vector2 startPoint = new Vector2(startX, startY);

            using (var builder = new CanvasPathBuilder(sender))
            {
                builder.BeginFigure(startPoint);
                builder.AddArc(Center, MiddleCircleRadius, MiddleCircleRadius, startAngle, sweepAngle);
                builder.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                {
                    ds.DrawGeometry(geometry, alarmColor, (float)MediumTicLength, arcStrokeStyle);
                }
            }
        }

        protected void DrawAlarmHighArc(CanvasControl sender, CanvasDrawEventArgs args, Color alarmColor, double alarmValue)
        {
            var ds = args.DrawingSession;

            float percentValue = (float)((alarmValue - MinValue) / (MaxValue - MinValue));

            float range = GaugeEndAngle - GaugeStartAngle;
            float arcStartAngle = GaugeStartAngle + (range * percentValue);

            float arcSweepAngle = GaugeEndAngle - arcStartAngle;
            if (arcSweepAngle > 360) arcSweepAngle -= 360;

            float startAngle = (float)(arcStartAngle * Math.PI / 180D);
            float sweepAngle = (float)(arcSweepAngle * Math.PI / 180D);

            double radian = arcStartAngle * Math.PI / 180D;
            float startX = (float)(Math.Cos(radian) * MiddleCircleRadius) + Center.X;
            float startY = (float)(Math.Sin(radian) * MiddleCircleRadius) + Center.Y;
            Vector2 startPoint = new Vector2(startX, startY);

            using (var builder = new CanvasPathBuilder(sender))
            {
                builder.BeginFigure(startPoint);
                builder.AddArc(Center, MiddleCircleRadius, MiddleCircleRadius, startAngle, sweepAngle);
                builder.EndFigure(CanvasFigureLoop.Open);

                using (var geometry = CanvasGeometry.CreatePath(builder))
                {
                    ds.DrawGeometry(geometry, alarmColor, (float)MediumTicLength, arcStrokeStyle);
                }
            }
        }

        protected void CreateInnerCircleBrush(CanvasControl sender)
        {
            Color step1Color = new Color();
            step1Color.A = 0xFF;
            step1Color.R = 0x01;
            step1Color.G = 0x10;
            step1Color.B = 0x18;

            // #FF01415C
            Color step2Color = new Color();
            step2Color.A = 0xFF;
            step2Color.R = 0x01;
            step2Color.G = 0x41;
            step2Color.B = 0x5C;

            var stops = new CanvasGradientStop[]
            {
                new CanvasGradientStop() { Color=Colors.Black, Position = -0.2f },
                new CanvasGradientStop() { Color=step2Color,   Position = 1.0f },
            };

            InnerCircleBrush = new CanvasLinearGradientBrush(sender, stops);
            InnerCircleBrush.StartPoint = new Vector2(Center.X, Center.Y - InnerCircleRadius);
            InnerCircleBrush.EndPoint = new Vector2(Center.X, Center.Y + InnerCircleRadius);
        }

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            if ((0 == sender.Size.Width) || (0 == sender.Size.Height))
            {
                this.OuterCircleRadius = 0;
            }
            else
            {
                this.OuterCircleRadius = Math.Min((float)sender.Size.Height / 2f - 2 * outerCircleThickness,
                                             (float)sender.Size.Width / 2f - 2 * outerCircleThickness);
            }

            this.MiddleCircleRadius = OuterCircleRadius - MiddleCircleDelta;
            this.InnerCircleRadius = MiddleCircleRadius - InnerCircleDelta;

            this.PointerStrokeStyle = new CanvasStrokeStyle()
            {
                StartCap = CanvasCapStyle.Flat,
                EndCap = CanvasCapStyle.Triangle,
                DashStyle = CanvasDashStyle.Solid
            };

            CreateInnerCircleBrush(sender);

            sender.Invalidate();

            _needsResourceRecreation = false;
        }
        protected CanvasLinearGradientBrush InnerCircleBrush { get; set; }

        protected Vector2 Center { get; set; }
        protected float OuterCircleRadius { get; set; }
        protected float MiddleCircleRadius { get; set; }
        protected float InnerCircleRadius { get; set; }
        protected CanvasStrokeStyle PointerStrokeStyle { get; set; }
        protected CanvasBitmap BatteryBitmap { get; set; }
        protected float PointerLength { get { return MiddleCircleRadius - InnerCircleRadius; } }

        override protected void RefreshAlarmColors()
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshValue(object oldValue, object newValue)
        {
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
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshHighWarningValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowAlarmValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshLowWarningValue(object oldValue, object newValue)
        {
            this.canvasControl?.Invalidate();
        }

        override protected void RefreshNominalValue(object oldValue, object newValue)
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
    }
}
