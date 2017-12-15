//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityVesselMonitoringSoftware;
using InfinityVesselMonitoringSoftware.Editors.GaugePageEditor;
using InfinityVesselMonitoringSoftware.Gauges;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using VesselMonitoringSuite.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VesselMonitoringSuite.Views
{
    public sealed partial class GaugePageView : UserControl
    {
        /// <summary>
        /// A value indicating whether a dependency property change handler
        /// should ignore the next change notification.  This is used to reset
        /// the value of properties without performing any of the actions in
        /// their change handlers.
        /// </summary>
        private bool _ignorePropertyChange;
        private EditRibbonView _editRibbon;
        private ObservableCollection<IGaugeItem> _editGaugeItemList = new ObservableCollection<IGaugeItem>();
        private List<Adorner> _adornerList = new List<Adorner>();

        public GaugePageView()
        {
            this.InitializeComponent();
            this.PointerPressed += GaugePageView_PointerPressed;

            //for (int row = 0; row < 3; row++)
            //{
            //    for (int col = 0; col < 3; col++)
            //    {
            //        IGaugeItem gaugeItem = new GaugeItem(this.ViewModel.GaugePageItem.PageId);
            //        gaugeItem.GaugeHeight = gaugeItem.GaugeWidth = 300;

            //        ArcGaugeLeft arcGaugeLeft = new ArcGaugeLeft();
            //        arcGaugeLeft.GaugeItem = gaugeItem;
            //        arcGaugeLeft.IsOnline = true;
            //        this.MainCanvas.AddChildBaseGauge(arcGaugeLeft, row, col);
            //    }
            //}        

            Messenger.Default.Register<List<IGaugeItem>>(this, "BuildGaugeItemList", (gaugeItemList) =>
            {
                if (null == gaugeItemList) return;
                if (gaugeItemList[0].PageId != this.ViewModel.GaugePageItem.PageId) return;

                this.MainCanvas.Children.Clear();

                foreach (IGaugeItem item in gaugeItemList)
                {
                    switch (item.GaugeType)
                    {
                        case GaugeTypeEnum.LeftArcGauge:
                            BuildLeftArcGauge(item);
                            break;

                        case GaugeTypeEnum.LeftTankGauge:
                            BuildLeftTankGauge(item);
                            break;

                        case GaugeTypeEnum.RightArcGauge: break;

                        case GaugeTypeEnum.RightTankGauge:
                            BuildRightTankGauge(item);
                            break;

                        case GaugeTypeEnum.TextControl:
                            BuildTextControl(item);
                            break;

                        case GaugeTypeEnum.TextGauge: break;
                    }
                }

                _editRibbon = new EditRibbonView();
                _editRibbon.HorizontalAlignment = HorizontalAlignment.Left;
                _editRibbon.VerticalAlignment = VerticalAlignment.Bottom;
                _editRibbon.ViewModel.IsEditMode = true;
                _editRibbon.ViewModel.GaugeItemList = gaugeItemList;

                this.MainCanvas.Children.Add(_editRibbon);
            });
        }

        /// <summary>
        /// When an object on the screen is selected, put an an adorder with handles so the object can be
        /// resized, rotated, moved, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GaugePageView_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Image image = e.OriginalSource as Image;
            if (null != image)
            {
                BaseGauge baseGauge = image.DataContext as BaseGauge;
                if (null != baseGauge)
                {
                    IGaugeItem gaugeItem = baseGauge.GaugeItem;
                    if (e.KeyModifiers == Windows.System.VirtualKeyModifiers.Shift)
                    {
                        if (_editGaugeItemList.Contains(gaugeItem))
                        {
                            this.RemovePopupItem(gaugeItem);
                        }
                        else
                        {
                            this.AddPopupItem(gaugeItem);
                        }
                    }
                    else
                    {
                        bool addPopItem = !_editGaugeItemList.Contains(gaugeItem);
                        this.RemoveAllPopupItems();
                        if (addPopItem)
                        {
                            this.AddPopupItem(gaugeItem);
                        }
                    }
                }
                else
                {
                    this.RemoveAllPopupItems();
                }
            }
            else
            {
                this.RemoveAllPopupItems();
            }
        }

        private void RemoveAllPopupItems()
        {
            while (_editGaugeItemList.Count > 0)
            {
                this.RemovePopupItem(_editGaugeItemList[0]);
            }
        }

        private void RemovePopupItem(IGaugeItem gaugeItem)
        {
            int index = _editGaugeItemList.IndexOf(gaugeItem);
            _editGaugeItemList.RemoveAt(index);

            Adorner adorner = _adornerList[index];
            adorner.IsOpen = false;
            _adornerList.RemoveAt(index);
            this.MainCanvas.Children.Remove(adorner.Popup);
        }

        private void AddPopupItem(IGaugeItem gaugeItem)
        {
            _editGaugeItemList.Add(gaugeItem);

            //Rectangle rectangle = new Rectangle()
            //{
            //    Stroke = new SolidColorBrush(Colors.White),
            //    StrokeThickness = 4,
            //    Height = gaugeItem.GaugeHeight,
            //    Width = gaugeItem.GaugeWidth
            //};


            Adorner adorner = new Adorner(gaugeItem);
            adorner.IsOpen = true;

            //Popup popup = new Popup();
            //popup.Child = rectangle;

            //Binding gaugeLeftBinding = new Binding();
            //gaugeLeftBinding.Source = gaugeItem;
            //gaugeLeftBinding.Path = new PropertyPath("GaugeLeft");
            //popup.SetBinding(Popup.HorizontalOffsetProperty, gaugeLeftBinding);

            //Binding gaugeTopBinding = new Binding();
            //gaugeTopBinding.Source = gaugeItem;
            //gaugeTopBinding.Path = new PropertyPath("GaugeTop");
            //popup.SetBinding(Popup.VerticalOffsetProperty, gaugeTopBinding);

            //popup.IsOpen = true;
            //_adornerList.Add(popup);
            //this.MainCanvas.Children.Add(popup);

            _adornerList.Add(adorner);
            this.MainCanvas.Children.Add(adorner.Popup);
        }

        public GaugePageViewModel ViewModel
        {
            get { return this.VM; }
        }

        private void BuildLeftArcGauge(IGaugeItem gaugeItem)
        {
            BuildGauge(gaugeItem, (sensor) =>
            {
                // Build the gauge
                ArcGaugeLeft arcGaugeLeft = new ArcGaugeLeft();
                arcGaugeLeft.GaugeItem = gaugeItem;
                arcGaugeLeft.SensorItem = sensor;

                // Add it to the page
                this.MainCanvas.Children.Add(arcGaugeLeft);
            });
        }

        private void BuildLeftTankGauge(IGaugeItem gaugeItem)
        {
            BuildGauge(gaugeItem, (sensor) =>
            {
                // Build the gauge
                TankGaugeLeft tankGaugeLeft = new TankGaugeLeft();
                tankGaugeLeft.GaugeItem = gaugeItem;
                tankGaugeLeft.SensorItem = sensor;

                // Add it to the page
                this.MainCanvas.Children.Add(tankGaugeLeft);
            });
        }

        private void BuildRightTankGauge(IGaugeItem gaugeItem)
        {
            BuildGauge(gaugeItem, (sensor) => 
            {
                // Build the gauge
                TankGaugeRight tankGaugeRight = new TankGaugeRight();
                tankGaugeRight.GaugeItem = gaugeItem;
                tankGaugeRight.SensorItem = sensor;

                // Add it to the page
                this.MainCanvas.Children.Add(tankGaugeRight);
            });
        }

        private void BuildGauge(IGaugeItem gaugeItem, Action<ISensorItem> constructor)
        {
            ISensorItem sensor = App.SensorCollection.FindBySensorId(gaugeItem.SensorId);
            if (null != sensor)
            {
                sensor.IsOnline = true;
                sensor.DemoMode = true;

                constructor(sensor);
            }
        }

        private void BuildTextControl(IGaugeItem gaugeItem)
        {
            TextControl textControl = new TextControl();
            textControl.GaugeItem = gaugeItem;

            // Add it to the page
            this.MainCanvas.Children.Add(textControl);
        }

        #region public int Rows

        /// <summary>
        /// Gets or sets the number of rows that are in the grid.
        /// </summary>
        /// <returns>The number of rows that are in the grid. The default is 0.</returns>
        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                "Rows",
                typeof(int),
                typeof(GaugePageView),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        #endregion

        #region public int Columns

        /// <summary>
        /// Gets or sets the number of columns that are in the grid.
        /// </summary>
        /// <returns>The number of columns that are in the grid. The default is 0.</returns>
        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register(
                "Columns",
                typeof(int),
                typeof(GaugePageView),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        #endregion

        /// <summary>
        /// Validity check on row or column. For now, just check that it is positive. This code could be much simplified.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnRowsColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GaugePageView source = (GaugePageView)d;
            int value = (int)e.NewValue;

            // Ignore the change if requested
            if (source._ignorePropertyChange)
            {
                source._ignorePropertyChange = false;
                return;
            }

            if (value < 0)
            {
                // Reset the property to its original state before throwing
                source._ignorePropertyChange = true;
                source.SetValue(e.Property, (int)e.OldValue);

                string message = string.Format(
                    CultureInfo.InvariantCulture,
                    "Properties.Resources.GaugePageView_RowsColumnsChanged_InvalidValue",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }
    }
}
