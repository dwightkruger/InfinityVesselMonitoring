//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.ObjectModel;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Controls
{
    public partial class PolyLineControl : UserControl
    {
        public PolyLineControl()
        {
            this.InitializeComponent();
            this.Points = new ObservableCollection<Point>();

            this.Points.CollectionChanged += Points_CollectionChanged;
            this.InitializeComponent();
        }

        private void Points_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.canvasControl?.Invalidate();
        }

        protected void canvasControl_Loaded(object sender, RoutedEventArgs e)
        {
        }

        protected void canvasControl_CreateResources(CanvasControl sender, CanvasCreateResourcesEventArgs args)
        {
        }


        protected void canvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            if (Points.Count > 0)
            {
                var ds = args.DrawingSession;
                ds.Transform = Matrix3x2.CreateRotation((float)(this.Rotation * Math.PI / 180f), PolyLineCenter);

                CanvasPathBuilder pathBuilder = new CanvasPathBuilder(sender);
                pathBuilder.BeginFigure((float)Points[0].X, (float)Points[0].Y);

                for (int i = 1; i < Points.Count; i++)
                {
                    pathBuilder.AddLine((float)Points[i].X, (float)Points[i].Y);
                }

                pathBuilder.EndFigure(CanvasFigureLoop.Open);

                CanvasGeometry canvasGeometry = CanvasGeometry.CreatePath(pathBuilder);

                CanvasCommandList cl = new CanvasCommandList(sender);
                using (CanvasDrawingSession clds = cl.CreateDrawingSession())
                {
                    ds.DrawGeometry(canvasGeometry, PolyLineColor, PolyLineStrokeWidth);
                }

                GaussianBlurEffect blur = new GaussianBlurEffect();
                blur.Source = cl;
                blur.BlurAmount = PolyLineStrokeWidth;
                args.DrawingSession.DrawImage(blur);
            }
        }

        public ObservableCollection<Point> Points { get; set; }

        #region Rotation
        public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
            "Rotation",
            typeof(float),
            typeof(PolyLineControl),
            new PropertyMetadata(0f,
                                  new PropertyChangedCallback(OnRotationPropertyChanged)));

        public float Rotation
        {
            get { return (float)this.GetValue(RotationProperty); }
            set { this.SetValue(RotationProperty, value); }
        }

        protected static void OnRotationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyLineControl g = d as PolyLineControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region PolyLineCenter
        public static readonly DependencyProperty PolyLineCenterProperty = DependencyProperty.Register(
            "PolyLineCenterCenter",
            typeof(Vector2),
            typeof(PolyLineControl),
            new PropertyMetadata(new Vector2(0, 0),
                                  new PropertyChangedCallback(OnPolyLineCenterCenterPropertyChanged)));

        public Vector2 PolyLineCenter
        {
            get { return (Vector2)this.GetValue(PolyLineCenterProperty); }
            set { this.SetValue(PolyLineCenterProperty, value); }
        }

        protected static void OnPolyLineCenterCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyLineControl g = d as PolyLineControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region PolyLineColor
        public static readonly DependencyProperty PolyLineColorProperty = DependencyProperty.Register(
            "PolyLineColor",
            typeof(Color),
            typeof(PolyLineControl),
            new PropertyMetadata(Colors.Red,
                                  new PropertyChangedCallback(OnPolyLineColorPropertyChanged)));

        public Color PolyLineColor
        {
            get { return (Color)this.GetValue(PolyLineColorProperty); }
            set { this.SetValue(PolyLineColorProperty, value); }
        }

        protected static void OnPolyLineColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyLineControl g = d as PolyLineControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion

        #region PolyLineStrokeWidth
        public static readonly DependencyProperty PolyLineStrokeWidthProperty = DependencyProperty.Register(
            "PolyLineStrokeWidth",
            typeof(float),
            typeof(PolyLineControl),
            new PropertyMetadata(2f,
                                  new PropertyChangedCallback(OnPolyLineStrokeWidthPropertyChanged)));

        public float PolyLineStrokeWidth
        {
            get { return (float)this.GetValue(PolyLineStrokeWidthProperty); }
            set { this.SetValue(PolyLineStrokeWidthProperty, value); }
        }

        protected static void OnPolyLineStrokeWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PolyLineControl g = d as PolyLineControl;
            g?.canvasControl?.Invalidate();
        }
        #endregion


    }
}
