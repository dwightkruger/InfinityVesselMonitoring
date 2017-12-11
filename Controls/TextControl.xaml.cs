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
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class TextControl : BaseGauge
    {
        private bool _needsResourceRecreation = true;

        public TextControl()
        {
            this.InitializeComponent();
            this.CanvasControl = canvasControl;

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                DesignGrid.Visibility = Visibility.Collapsed;
                DesignTextBlock.Visibility = Visibility.Collapsed;
            }

            this.TextFontSize = 14;
        }

        protected CanvasControl CanvasControl { get; set; }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
            _needsResourceRecreation = true;
        }

        virtual protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (null == this.Text) return;
            if (0 == this.TextFontSize) return;

            this.EnsureResources(sender, args);

            var ds = args.DrawingSession;

            using (var textFormat = new CanvasTextFormat()
            {
                HorizontalAlignment = this.TextHorizontalAlignment,
                VerticalAlignment = this.TextVerticalAlignment,
                FontSize = (float)this.TextFontSize,
                Options = CanvasDrawTextOptions.Default,
            })
            {
                var textLayout = new CanvasTextLayout(sender, this.Text, textFormat, (float)this.GaugeWidth, (float)this.GaugeWidth);
                var layoutBounds = textLayout.LayoutBounds;

                double radians = this.TextAngle * Math.PI / 180D;
                ds.Transform *= Matrix3x2.CreateRotation((float)radians, new Vector2((float)layoutBounds.Width / 2F, (float)layoutBounds.Height / 2F));

                double x = Math.Cos(radians) * layoutBounds.Width / 2F;
                double y = -Math.Sin(radians) * layoutBounds.Height / 2F;
                ds.DrawTextLayout(textLayout, (float)x, (float)y, this.TextFontColor);
            }
        }

        override protected void RefreshLeft(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }
        override protected void RefreshTop(object oldValue, object newValue)
        {
            this.CanvasControl?.Invalidate();
        }

        private void canvasControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _needsResourceRecreation = true;
        }

        void EnsureResources(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (!_needsResourceRecreation)
                return;

            sender.Invalidate();

            _needsResourceRecreation = false;
        }
    }
}
