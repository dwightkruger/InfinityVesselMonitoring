//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class RectangleControl : UserControl
    {
        public RectangleControl()
        {
            this.InitializeComponent();

            if (!ViewModelBase.IsInDesignModeStatic)
            {
                DesignRectangle.Visibility = Visibility.Collapsed;
            }
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
        }


        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            ds.Transform = Matrix3x2.CreateRotation(this.Rotation);
            ds.DrawRoundedRectangle(this.Rect, this.RadiusX, this.RadiusY, this.Color, this.StrokeWidth);
        }

        #region Rect
        public static readonly DependencyProperty RectProperty = DependencyProperty.Register(
            "Rect",
            typeof(Rect),
            typeof(RectangleControl),
            new PropertyMetadata(new Rect(0, 0, 400, 400),
                                  new PropertyChangedCallback(OnRectPropertyChanged)));

        public Rect Rect
        {
            get { return (Rect)this.GetValue(RectProperty); }
            set { this.SetValue(RectProperty, value); }
        }

        protected static void OnRectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region RadiusX
        public static readonly DependencyProperty RadiusXProperty = DependencyProperty.Register(
            "RadiusX",
            typeof(float),
            typeof(RectangleControl),
            new PropertyMetadata(0f,
                                  new PropertyChangedCallback(OnRadiusXPropertyChanged)));

        public float RadiusX
        {
            get { return (float)this.GetValue(RadiusXProperty); }
            set { this.SetValue(RadiusXProperty, value); }
        }

        protected static void OnRadiusXPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region RadiusY
        public static readonly DependencyProperty RadiusYProperty = DependencyProperty.Register(
            "RadiusY",
            typeof(float),
            typeof(RectangleControl),
            new PropertyMetadata(0f,
                                  new PropertyChangedCallback(OnRadiusYPropertyChanged)));

        public float RadiusY
        {
            get { return (float)this.GetValue(RadiusYProperty); }
            set { this.SetValue(RadiusYProperty, value); }
        }

        protected static void OnRadiusYPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region Color
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(
            "Color",
            typeof(Color),
            typeof(RectangleControl),
            new PropertyMetadata(Colors.Red,
                                  new PropertyChangedCallback(OnColorPropertyChanged)));

        public Color Color
        {
            get { return (Color)this.GetValue(ColorProperty); }
            set { this.SetValue(ColorProperty, value); }
        }

        protected static void OnColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region StrokeWidth
        public static readonly DependencyProperty StrokeWidthProperty = DependencyProperty.Register(
            "StrokeWidth",
            typeof(float),
            typeof(RectangleControl),
            new PropertyMetadata(2f,
                                  new PropertyChangedCallback(OnStrokeWidthPropertyChanged)));

        public float StrokeWidth
        {
            get { return (float)this.GetValue(StrokeWidthProperty); }
            set { this.SetValue(StrokeWidthProperty, value); }
        }

        protected static void OnStrokeWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region Rotation
        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
            "Rotation",
            typeof(float),
            typeof(RectangleControl),
            new PropertyMetadata(0f,
                                  new PropertyChangedCallback(OnRotationPropertyChanged)));

        public float Rotation
        {
            get { return (float)this.GetValue(RotationProperty); }
            set { this.SetValue(RotationProperty, value); }
        }

        protected static void OnRotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RectangleControl g = d as RectangleControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion
    }
}
