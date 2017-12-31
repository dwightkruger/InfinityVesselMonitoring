//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace InfinityGroup.VesselMonitoring.Controls
{
    public class Adorner : IDisposable
    {
        private Popup _popup = new Popup();
        private Grid _grid = new Grid();
        private IGaugeItem _gaugeItem;
        private Rectangle _nwHandle;
        private Rectangle _nHandle;
        private Rectangle _neHandle;
        private Rectangle _eHandle;
        private Rectangle _seHandle;
        private Rectangle _sHandle;
        private Rectangle _swHandle;
        private Rectangle _wHandle;
        private List<Rectangle> _handleList = new List<Rectangle>();
        private const int c_handleHeight = 16;
        private Rectangle _boundingRectangle;

        public Adorner() 
        {
            this.HandleColor = (SolidColorBrush)Application.Current.Resources["ApplicationForegroundThemeBrush"];

            _boundingRectangle = new Rectangle()
            {
                Name = "AdornerBoundingRectangle",
                Stroke = this.HandleColor,
                StrokeThickness = 4,
            };

            _grid.Children.Add(_boundingRectangle);

            _nwHandle = CreateHandle(HorizontalAlignment.Left, VerticalAlignment.Top, new Point(-1, -1), CoreCursorType.SizeNorthwestSoutheast);
            _nHandle = CreateHandle(HorizontalAlignment.Center, VerticalAlignment.Top, new Point(0, -1), CoreCursorType.SizeNorthSouth);
            _neHandle = CreateHandle(HorizontalAlignment.Right, VerticalAlignment.Top, new Point(1, -1), CoreCursorType.SizeNortheastSouthwest);
            _eHandle = CreateHandle(HorizontalAlignment.Right, VerticalAlignment.Center, new Point(1, 0), CoreCursorType.SizeWestEast);
            _seHandle = CreateHandle(HorizontalAlignment.Right, VerticalAlignment.Bottom, new Point(1, 1), CoreCursorType.SizeNorthwestSoutheast);
            _sHandle = CreateHandle(HorizontalAlignment.Center, VerticalAlignment.Bottom, new Point(0, 1), CoreCursorType.SizeNorthSouth);
            _swHandle = CreateHandle(HorizontalAlignment.Left, VerticalAlignment.Bottom, new Point(-1, 1), CoreCursorType.SizeNortheastSouthwest);
            _wHandle = CreateHandle(HorizontalAlignment.Left, VerticalAlignment.Center, new Point(-1, 0), CoreCursorType.SizeWestEast);

            _popup.Child = _grid;

            Messenger.Default.Register<Color>(this, "OnThemeColorsChanged", (color) =>
            {
                this.HandleColor = new SolidColorBrush(color);

                _boundingRectangle.Stroke = this.HandleColor;
                foreach (Rectangle rect in _handleList)
                {
                    rect.Stroke = this.HandleColor;
                    rect.Fill   = this.HandleColor;
                }
            });
        }

        public IGaugeItem GaugeItem
        {
            set
            {
                _gaugeItem = value;

                Binding popupLeftBinding = new Binding();
                popupLeftBinding.Source = _gaugeItem;
                popupLeftBinding.Path = new PropertyPath("GaugeLeft");
                popupLeftBinding.Mode = BindingMode.TwoWay;
                _popup.SetBinding(Popup.HorizontalOffsetProperty, popupLeftBinding);

                Binding popupTopBinding = new Binding();
                popupTopBinding.Source = _gaugeItem;
                popupTopBinding.Path = new PropertyPath("GaugeTop");
                popupTopBinding.Mode = BindingMode.TwoWay;
                _popup.SetBinding(Popup.VerticalOffsetProperty, popupTopBinding);

                Binding popupHeightBinding = new Binding();
                popupHeightBinding.Source = _gaugeItem;
                popupHeightBinding.Path = new PropertyPath("GaugeHeight");
                popupHeightBinding.Mode = BindingMode.TwoWay;
                _popup.SetBinding(Popup.HeightProperty, popupHeightBinding);
                _boundingRectangle.SetBinding(Rectangle.HeightProperty, popupHeightBinding);

                Binding popupWidthBinding = new Binding();
                popupWidthBinding.Source = _gaugeItem;
                popupWidthBinding.Path = new PropertyPath("GaugeWidth");
                popupWidthBinding.Mode = BindingMode.TwoWay;
                _popup.SetBinding(Popup.WidthProperty, popupWidthBinding);
                _boundingRectangle.SetBinding(Rectangle.WidthProperty, popupWidthBinding);
            }
        }

        public bool IsOpen
        {
            get { return _popup.IsOpen; }
            set { _popup.IsOpen = value; }
        }

        public Popup  Popup
        {
            get { return _popup; }
        }

        private SolidColorBrush HandleColor { get; set; }


        private Rectangle CreateHandle(
            HorizontalAlignment ha,
            VerticalAlignment va,
            Point allowedDelta,
            CoreCursorType cursorType)
        {
            Rectangle handle = new Rectangle()
            {
                Name = "AdonerHandle",
                Stroke = this.HandleColor,
                Fill = this.HandleColor,
                StrokeThickness = 1,
                Height = c_handleHeight,
                Width = c_handleHeight,
                HorizontalAlignment = ha,
                VerticalAlignment = va,
                Tag = allowedDelta,
            };

            // Establish the event handlers for changing the size of this object
            handle.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            handle.ManipulationStarted += Handle_ManipulationStarted;
            handle.ManipulationCompleted += Handle_ManipulationCompleted;
            handle.ManipulationDelta += Handle_ManipulationDelta;

            Microsoft.Toolkit.Uwp.UI.Extensions.Mouse.SetCursor(handle, cursorType);

            _handleList.Add(handle);

            _grid.Children.Add(handle);

            return handle;
        }


        private void Handle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Rectangle handle = sender as Rectangle;         // Get the handle being moved
            Point allowedDeleta = (Point)handle.Tag;        // Get the allowed X/Y deltas for this handle

            if (allowedDeleta.X != 0)
            {
                double left = _gaugeItem.GaugeLeft + e.Delta.Translation.X * e.Delta.Scale;
                _gaugeItem.GaugeLeft = Math.Max(0, left);

                double width = _gaugeItem.GaugeWidth + e.Delta.Translation.X * e.Delta.Scale * allowedDeleta.X;
                _gaugeItem.GaugeWidth = Math.Max(10, width);
            }

            if (allowedDeleta.Y != 0)
            {
                double top = _gaugeItem.GaugeTop + e.Delta.Translation.Y * e.Delta.Scale;
                _gaugeItem.GaugeTop = Math.Max(0, top);

                double height = _gaugeItem.GaugeHeight + e.Delta.Translation.Y * e.Delta.Scale * allowedDeleta.Y;
                _gaugeItem.GaugeHeight = Math.Max(10, height);
            }
        }

        private void Handle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
        }

        private void Handle_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Messenger.Default.Unregister<Color>(this, "OnThemeColorsChanged");

                    foreach (Rectangle handle in _handleList)
                    {
                        handle.ManipulationStarted -= Handle_ManipulationStarted;
                        handle.ManipulationCompleted -= Handle_ManipulationCompleted;
                        handle.ManipulationDelta -= Handle_ManipulationDelta;

                        _grid.Children.Remove(handle);
                    }

                    _handleList = null;
                    _popup.IsOpen = false;
                }

                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
