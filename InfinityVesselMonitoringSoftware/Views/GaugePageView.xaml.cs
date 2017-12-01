//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Gauges;
using InfinityGroup.VesselMonitoring.Gauges.ArcGaugeLeft;
using InfinityGroup.VesselMonitoring.Gauges.TankGaugeLeft;
using InfinityGroup.VesselMonitoring.Gauges.TankGaugeRight;
using InfinityGroup.VesselMonitoring.Gauges.TextControl;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.Collections.Generic;
using VesselMonitoringSuite.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace VesselMonitoringSuite.Views
{
    public sealed partial class GaugePageView : UserControl
    {
        public GaugePageView()
        {
            this.InitializeComponent();

            Messenger.Default.Register<List<IGaugeItem>>(this, "BuildGaugeItemList", (gaugeItemList) =>
            {
                if (null == gaugeItemList) return;
                if (gaugeItemList[0].PageId != this.ViewModel.GaugePageItem.PageId) return;

                this.MainCanvas.Children.Clear();
                foreach (IGaugeItem item in gaugeItemList)
                {
                    var sensorId = item.SensorId;

                    switch (item.GaugeType)
                    {
                        case GaugeTypeEnum.LeftArcGauge:
                            // Build the gauge
                            ArcGaugeLeftViewModel arcGaugeLeftViewModel = new ArcGaugeLeftViewModel();
                            arcGaugeLeftViewModel.GaugeItem = item;
                            ArcGaugeLeftView arcGaugeLeft = new ArcGaugeLeftView();
                            arcGaugeLeft.ViewModel = arcGaugeLeftViewModel;

                            // Add it to the page
                            this.MainCanvas.Children.Add(arcGaugeLeft);
                            break;

                        case GaugeTypeEnum.LeftTankGauge:
                            // Build the gauge
                            TankGaugeLeftViewModel tankGaugeLeftViewModel = new TankGaugeLeftViewModel();
                            tankGaugeLeftViewModel.GaugeItem = item;
                            TankGaugeLeftView tankGaugeLeft = new TankGaugeLeftView();
                            tankGaugeLeft.ViewModel = tankGaugeLeftViewModel;

                            // Add it to the page
                            this.MainCanvas.Children.Add(tankGaugeLeft);

                            break;

                        case GaugeTypeEnum.RightArcGauge: break;

                        case GaugeTypeEnum.RightTankGauge:
                            {
                                // Build the gauge
                                TankGaugeRightViewModel tankGaugeRightViewModel = new TankGaugeRightViewModel();
                                tankGaugeRightViewModel.GaugeItem = item;
                                TankGaugeRightView tankGaugeRight = new TankGaugeRightView();
                                tankGaugeRight.ViewModel = tankGaugeRightViewModel;

                                // Add it to the page
                                this.MainCanvas.Children.Add(tankGaugeRight);
                            }
                            break;

                        case GaugeTypeEnum.TextControl:
                            {
                                // Build the gauge
                                TextControlViewModel textControlViewModel = new TextControlViewModel();
                                textControlViewModel.GaugeItem = item;
                                TextControlView textControl = new TextControlView();
                                textControl.ViewModel = textControlViewModel;

                                // Add it to the page
                                this.MainCanvas.Children.Add(textControl);
                            }
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
    }
}
