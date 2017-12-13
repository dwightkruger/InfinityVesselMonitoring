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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using VesselMonitoringSuite.ViewModels;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public GaugePageView()
        {
            this.InitializeComponent();

            //for (int row = 0; row < 3; row++)
            //{
            //    for (int col = 0; col < 3; col++)
            //    {
            //        GaugeItem gaugeItem = new GaugeItem();
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
            });
        }

        public GaugePageViewModel ViewModel
        {
            get { return this.VM; }
            set { this.VM = value; }
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

        private void MainCanvas_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {
        }
    }
}
