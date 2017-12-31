//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityVesselMonitoringSoftware.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityVesselMonitoringSoftware.Views
{
    public sealed partial class BasePageView : UserControl
    {
        private bool _ignorePropertyChange;
        private ObservableCollection<IGaugeItem> _gaugeItemSelectedList = new ObservableCollection<IGaugeItem>();
        private List<Adorner> _adornerList = new List<Adorner>();
        private int _nextRow = 0;
        private int _nextCol = 0;

        public BasePageView()
        {
            this.InitializeComponent();
            this.Loaded += BasePageView_Loaded;
            this.PointerPressed += BasePageView_PointerPressed;
            this.KeyDown += MainCanvas_KeyDown;
            this.EditRibbon.ViewModel.SelectedGaugeItemList = _gaugeItemSelectedList;

            Messenger.Default.Register<List<IGaugeItem>>(this, "AddGaugeItemList", (gaugeItemList) =>
            {
                if (null == gaugeItemList) return;
                if (gaugeItemList[0].PageId != this.ViewModel.GaugePageItem.PageId) return;

                foreach (IGaugeItem gaugeItem in gaugeItemList)
                {
                    BaseGauge gauge = null;
                    switch (gaugeItem.GaugeType)
                    {
                        case GaugeTypeEnum.LeftArcGauge:
                            gauge = new ArcGaugeLeft();
                        break;

                        case GaugeTypeEnum.LeftTankGauge:
                            gauge = new TankGaugeLeft();
                        break;

                        case GaugeTypeEnum.RightArcGauge: break;

                        case GaugeTypeEnum.RightTankGauge:
                            gauge = new TankGaugeRight();
                        break;

                        case GaugeTypeEnum.TextControl:
                            gauge = new TextControl();
                        break;

                        case GaugeTypeEnum.TextGauge: break;
                    }

                    gauge.GaugeItem = gaugeItem;
                    gauge.SensorItem = App.SensorCollection.FindBySensorId(gaugeItem.SensorId);

                    // Calculate the row/col position of this gauge
                    this.CanvasGrid.AddChildBaseGauge(gauge as BaseGauge, _nextRow, _nextCol);

                    _nextCol++;
                    if (_nextCol >= this.Columns)
                    {
                        _nextRow++;
                        _nextCol = 0;
                    }

                    // Persist the calculated (X,Y) location of the gauge
                    Task.Run(async () => { await gaugeItem.BeginCommit(); }).Wait();
                }
            });

            // Register to receive lists of gauges to display on the screen. Each item in this list
            // already has an (X,Y) location specified,
            Messenger.Default.Register<List<IGaugeItem>>(this, "BuildGaugeItemList", (gaugeItemList) =>
            {
                if (null == gaugeItemList) return;
                if (gaugeItemList[0].PageId != this.ViewModel.GaugePageItem.PageId) return;

                this.CanvasGrid.Children.Clear();
                BaseGauge gauge = null;

                foreach (IGaugeItem item in gaugeItemList)
                {
                    switch (item.GaugeType)
                    {
                        case GaugeTypeEnum.LeftArcGauge:
                            gauge = this.BuildLeftArcGauge(item);
                            break;

                        case GaugeTypeEnum.LeftTankGauge:
                            gauge = this.BuildLeftTankGauge(item);
                            break;

                        case GaugeTypeEnum.RightArcGauge: break;

                        case GaugeTypeEnum.RightTankGauge:
                            gauge = this.BuildRightTankGauge(item);
                            break;

                        case GaugeTypeEnum.TextControl:
                            gauge = this.BuildTextControl(item);
                            break;

                        case GaugeTypeEnum.TextGauge:
                            break;
                    }
                }
            });
        }

        private void BasePageView_Loaded(object sender, RoutedEventArgs e)
        {
            // Register to receive keydown messages from parent controls.
            Messenger.Default.Register<Tuple<int, KeyRoutedEventArgs>>(this, "MainPagePivot_KeyDown", (tp) =>
            {
                // Was this keypress meant for this page?
                if (tp.Item1 == this.ViewModel.GaugePageItem.Position)
                {
                    MainCanvas_KeyDown(this, tp.Item2);
                }
            });
        }


        public BasePageViewModel ViewModel
        {
            get { return this.VM; }
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
                typeof(BasePageView),
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
                typeof(BasePageView),
                new PropertyMetadata(0, OnRowsColumnsChanged));

        private static void OnRowsColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BasePageView source = (BasePageView)d;
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
                    "Properties.Resources.BasePageView_RowsColumnsChanged_InvalidValue",
                    value);
                throw new ArgumentException(message, "value");
            }

            // The length properties affect measuring.
            source.InvalidateMeasure();
        }

        #endregion

        /// <summary>
        /// When an object on the screen is selected, put an an adorner with handles so the object can be
        /// resized, rotated, moved, etc.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BasePageView_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            this.CanvasGrid.ChildPositionComplete();

            Image image = e.OriginalSource as Image;
            if (null != image)
            {
                BaseGauge baseGauge = image.DataContext as BaseGauge;
                if (null != baseGauge)
                {
                    IGaugeItem gaugeItem = baseGauge.GaugeItem;
                    if (e.KeyModifiers == Windows.System.VirtualKeyModifiers.Shift)
                    {
                        if (_gaugeItemSelectedList.Contains(gaugeItem))
                        {
                            this.RemoveAdorner(gaugeItem);
                        }
                        else
                        {
                            this.AddAdorner(gaugeItem);
                        }
                    }
                    else
                    {
                        bool addPopItem = !_gaugeItemSelectedList.Contains(gaugeItem);
                        this.RemoveAllAdorners();
                        if (addPopItem)
                        {
                            this.AddAdorner(gaugeItem);
                        }
                    }
                }
                else
                {
                    this.RemoveAllAdorners();
                }
            }
            else
            {
                this.RemoveAllAdorners();
            }
        }

        #region Adorners
        private void RemoveAllAdorners()
        {
            while (_gaugeItemSelectedList.Count > 0)
            {
                this.RemoveAdorner(_gaugeItemSelectedList[0]);
            }
        }

        private void RemoveAdorner(IGaugeItem gaugeItem)
        {
            int index = _gaugeItemSelectedList.IndexOf(gaugeItem);
            _gaugeItemSelectedList.RemoveAt(index);

            Adorner adorner = _adornerList[index];
            adorner.IsOpen = false;
            _adornerList.RemoveAt(index);
            this.CanvasGrid.Children.Remove(adorner.Popup);
            adorner.Dispose();
        }

        private void AddAdorner(IGaugeItem gaugeItem)
        {
            _gaugeItemSelectedList.Add(gaugeItem);

            Adorner adorner = new Adorner();
            adorner.GaugeItem = gaugeItem;
            adorner.IsOpen = true;
            _adornerList.Add(adorner);
            this.CanvasGrid.Children.Add(adorner.Popup);
        }
        #endregion

        /// <summary>
        /// Process keydown events for cut, copy, paste, undo & redo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainCanvas_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            var ctrlKey = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            bool isCtrlKeyPressed = ctrlKey.HasFlag(CoreVirtualKeyStates.Down);

            switch (e.Key)
            {
                case VirtualKey.C:       // CTRL+C = Copy
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.CopyCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.V:       // CTRL+V = Paste
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.PasteCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.Z:       // CTRL+Z = Undo
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.UndoCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;

                case VirtualKey.Y:      // CTRL+Y = Redo
                    if (isCtrlKeyPressed)
                    {
                        this.EditRibbon.ViewModel.RedoCommand.Execute(null);
                        e.Handled = true;
                    }
                    break;
            }
        }

        private BaseGauge BuildLeftArcGauge(IGaugeItem gaugeItem)
        {
            ArcGaugeLeft arcGaugeLeft = new ArcGaugeLeft();
            arcGaugeLeft.GaugeItem = gaugeItem;

            BuildGauge(gaugeItem, (sensor) =>
            {
                arcGaugeLeft.SensorItem = sensor;
            });

            // Add it to the page
            this.CanvasGrid.Children.Add(arcGaugeLeft);
            return arcGaugeLeft;
        }

        private BaseGauge BuildLeftTankGauge(IGaugeItem gaugeItem)
        {
            TankGaugeLeft tankGaugeLeft = new TankGaugeLeft();
            tankGaugeLeft.GaugeItem = gaugeItem;

            BuildGauge(gaugeItem, (sensor) =>
            {
                tankGaugeLeft.SensorItem = sensor;
            });

            // Add it to the page
            this.CanvasGrid.Children.Add(tankGaugeLeft);
            return tankGaugeLeft;
        }

        private BaseGauge BuildRightTankGauge(IGaugeItem gaugeItem)
        {
            TankGaugeRight tankGaugeRight = new TankGaugeRight();
            tankGaugeRight.GaugeItem = gaugeItem;

            BuildGauge(gaugeItem, (sensor) =>
            {
                tankGaugeRight.SensorItem = sensor;
            });

            // Add it to the page
            this.CanvasGrid.Children.Add(tankGaugeRight);
            return tankGaugeRight;
        }

        private void BuildGauge(IGaugeItem gaugeItem, Action<ISensorItem> constructor)
        {
            ISensorItem sensor = App.SensorCollection.FindBySensorId(gaugeItem.SensorId);
            if (null != sensor)
            {
                sensor.IsDemoMode = true;
            }

            constructor(sensor);
        }

        private BaseGauge BuildTextControl(IGaugeItem gaugeItem)
        {
            TextControl textControl = new TextControl();
            textControl.GaugeItem = gaugeItem;

            // Add it to the page
            this.CanvasGrid.Children.Add(textControl);
            return textControl;
        }
    }
}
