//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

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
        private static Brush c_handleColor = new SolidColorBrush(Colors.White);
        private static Brush c_transparentColor = new SolidColorBrush(Colors.Transparent);
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
        private const int c_handleHeight = 12;
        private CoreCursor _defaultCursor;
        private static CoreCursor _nwCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.SizeNorthwestSoutheast, 0);

        public Adorner(IGaugeItem gaugeItem)
        {
            _gaugeItem = gaugeItem;

            Rectangle rectangle = new Rectangle()
            {
                Fill = c_transparentColor,
                Stroke = c_handleColor,
                StrokeThickness = 4,
                Height = _gaugeItem.GaugeHeight,
                Width = _gaugeItem.GaugeWidth,
            };

            _grid.Children.Add(rectangle);

            _nwHandle = CreateHandle(HorizontalAlignment.Left,   VerticalAlignment.Top,    new Point(1,1));
            _nHandle  = CreateHandle(HorizontalAlignment.Center, VerticalAlignment.Top,    new Point(0,1));
            _neHandle = CreateHandle(HorizontalAlignment.Right,  VerticalAlignment.Top,    new Point(1,1));
            _eHandle  = CreateHandle(HorizontalAlignment.Right,  VerticalAlignment.Center, new Point(1,0));
            _seHandle = CreateHandle(HorizontalAlignment.Right,  VerticalAlignment.Bottom, new Point(1,1));
            _sHandle  = CreateHandle(HorizontalAlignment.Center, VerticalAlignment.Bottom, new Point(0,1));
            _swHandle = CreateHandle(HorizontalAlignment.Left,   VerticalAlignment.Bottom, new Point(1,1));
            _wHandle  = CreateHandle(HorizontalAlignment.Left,   VerticalAlignment.Center, new Point(1,0));

            _popup.Child = _grid;
            _popup.GotFocus += Handle_GotFocus;
            _popup.LostFocus += Handle_LostFocus;

            Binding gaugeLeftBinding = new Binding();
            gaugeLeftBinding.Source = _gaugeItem;
            gaugeLeftBinding.Path = new PropertyPath("GaugeLeft");
            _popup.SetBinding(Popup.HorizontalOffsetProperty, gaugeLeftBinding);

            Binding gaugeTopBinding = new Binding();
            gaugeTopBinding.Source = _gaugeItem;
            gaugeTopBinding.Path = new PropertyPath("GaugeTop");
            _popup.SetBinding(Popup.VerticalOffsetProperty, gaugeTopBinding);

            _defaultCursor = Window.Current.CoreWindow.PointerCursor;
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

        private Rectangle CreateHandle(
            HorizontalAlignment ha,
            VerticalAlignment va,
            Point allowedDelta)
        {
            Rectangle handle = new Rectangle()
            {
                Fill = c_handleColor,
                Stroke = c_handleColor,
                StrokeThickness = 1,
                Height = c_handleHeight,
                Width = c_handleHeight,
                HorizontalAlignment = ha,
                VerticalAlignment = va,
                Tag = allowedDelta,
                IsHitTestVisible = true,
            };

            // Establish the event handlers for changing the size of this object
            //handle.ManipulationMode = ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            //handle.ManipulationStarted += Handle_ManipulationStarted;
            //handle.ManipulationCompleted += Handle_ManipulationCompleted;
            //handle.ManipulationDelta += Handle_ManipulationDelta;
            handle.GotFocus += Handle_GotFocus;
            handle.LostFocus += Handle_LostFocus;

            _handleList.Add(handle);

            _grid.Children.Add(handle);

            return handle;
        }

        private void Handle_LostFocus(object sender, RoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = _defaultCursor;
        }

        /// <summary>
        /// When the handle gets focus, change the mouse pointer appropritately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Handle_GotFocus(object sender, RoutedEventArgs e)
        {
            Rectangle handle = sender as Rectangle;         // Get the handle being moved
            Point allowedDeleta = (Point)handle.Tag;        // Get the allowed X/Y deltas for this handle

            Window.Current.CoreWindow.PointerCursor = _nwCursor;
        }

        private void Handle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            Rectangle handle = sender as Rectangle;         // Get the handle being moved
            Point allowedDeleta = (Point)handle.Tag;        // Get the allowed X/Y deltas for this handle

            if (allowedDeleta.X > 0)
            {
                _gaugeItem.GaugeLeft += e.Delta.Translation.X * e.Delta.Scale;
                _gaugeItem.GaugeWidth += e.Delta.Translation.X * e.Delta.Scale;
            }

            if (allowedDeleta.Y > 0)
            {
                _gaugeItem.GaugeTop += e.Delta.Translation.Y * e.Delta.Scale;
                _gaugeItem.GaugeHeight += e.Delta.Translation.Y * e.Delta.Scale;
            }
        }

        private void Handle_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = _defaultCursor;
        }

        private void Handle_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            Rectangle handle = sender as Rectangle;         // Get the handle being moved
            Point allowedDeleta = (Point)handle.Tag;        // Get the allowed X/Y deltas for this handle

            Window.Current.CoreWindow.PointerCursor = _nwCursor;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
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
