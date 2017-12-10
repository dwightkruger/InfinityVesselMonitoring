//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityVesselMonitoringSoftware;
using System;
using System.Collections.Generic;
using System.Globalization;
using VesselMonitoringSuite.ViewModels;
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
                    ISensorItem sensor = App.SensorCollection.FindBySensorId(item.SensorId);

                    if (null != sensor)
                    {
                        sensor.IsOnline = true;
                        sensor.DemoMode = true;

                        switch (item.GaugeType)
                        {
                            case GaugeTypeEnum.LeftArcGauge:
                                // Build the gauge
                                ArcGaugeLeft arcGaugeLeft = new ArcGaugeLeft();
                                arcGaugeLeft.GaugeItem = item;
                                arcGaugeLeft.SensorItem = sensor;

                                // Add it to the page
                                this.MainCanvas.Children.Add(arcGaugeLeft);
                                break;

                            case GaugeTypeEnum.LeftTankGauge:
                                // Build the gauge
                                TankGaugeLeft tankGaugeLeft = new TankGaugeLeft();
                                tankGaugeLeft.GaugeItem = item;
                                tankGaugeLeft.SensorItem = sensor;

                                // Add it to the page
                                this.MainCanvas.Children.Add(tankGaugeLeft);

                                break;

                            case GaugeTypeEnum.RightArcGauge: break;

                            case GaugeTypeEnum.RightTankGauge:
                                {
                                    // Build the gauge
                                    TankGaugeRight tankGaugeRight = new TankGaugeRight();
                                    tankGaugeRight.GaugeItem = item;
                                    tankGaugeRight.SensorItem = sensor;

                                    // Add it to the page
                                    this.MainCanvas.Children.Add(tankGaugeRight);
                                }
                                break;

                            case GaugeTypeEnum.TextControl:
                                {
                                    // Build the gauge
                                    TextControl textControl = new TextControl();
                                    textControl.GaugeItem = item;
                                    textControl.SensorItem = sensor;

                                    // Add it to the page
                                    this.MainCanvas.Children.Add(textControl);
                                }
                                break;

                            case GaugeTypeEnum.TextGauge: break;
                        }
                    }
                }
            });
        }

        public GaugePageViewModel ViewModel
        {
            get { return this.VM; }
            set { this.VM = value; }
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
