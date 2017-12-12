//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Gauges;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Utilities;
using InfinityVesselMonitoringSoftware;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VesselMonitoringSuite.Devices;
using VesselMonitoringSuite.Sensors;
using VesselMonitoringSuite.ViewModels;
using VesselMonitoringSuite.Views;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace VesselMonitoring
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        //Timer _valueTimer;
        //private Random randu = new Random();

        public MainPage()
        {
            this.InitializeComponent();

            DispatcherHelper.Initialize();

            // Track all unhandled exceptions
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.UnhandledException += ApplicationUnhandledException;

            Telemetry.TrackEvent("Application Started");
        }

    /// <summary>
    /// Log unhandled exceptions from the task scheduler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            Telemetry.TrackException(e.Exception);
            Telemetry.Flush(); // only for desktop apps
        }

        /// <summary>
        /// Log unhandled exceptions from the dispatcher.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void ApplicationUnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            Telemetry.TrackException(e.Exception);
            Telemetry.Flush(); // only for desktop apps

            e.Handled = true;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            //BuildDBTables.Directory = Environment.GetEnvironmentVariable("APPDATA") + typeof(App).ToString();
            BuildDBTables.Directory = ApplicationData.Current.LocalFolder.Path + @"\" + typeof(App).ToString();
            InfinityGroup.VesselMonitoring.SQLiteDB.Utilities.CreateDirectory(BuildDBTables.Directory);

            //BuildDBTables.Directory = ApplicationData.Current.TemporaryFolder.Path;
            BuildDBTables.DatabaseName = "InfinityGroupVesselMonitoring";

            Task.Run(async () => 
            {
                var x = new BuildDBTables();
                await x.DoIt();
            }).Wait();

            this.BuildGaugePages();

            Task.Run(async () => { await BuildDBTables.VesselSettingsTable.BeginEmpty(); }).Wait();

            //App.VesselSettings = new VesselSettings();
            //App.VesselSettings.VesselName = "MV Infinity";
            //App.VesselSettings.FromEmailAddress = "";
            //App.VesselSettings.FromEmailPassword = "";
            //App.VesselSettings.ToEmailAddress = "dwightkruger@mvinfinity.com";
            //App.VesselSettings.SMTPServerName = "smtp-mail.outlook.com";
            //App.VesselSettings.SMTPPort = 587;
            //App.VesselSettings.SMTPEncryptionMethod = 2; // SmtpConnectType.ConnectSTARTTLS

            //SendEmail.FromEmailAddress = App.VesselSettings.FromEmailAddress;
            //SendEmail.FromEmailPassword = App.VesselSettings.FromEmailPassword;
            //SendEmail.SMTPEncryptionMethod = App.VesselSettings.SMTPEncryptionMethod;
            //SendEmail.SMTPPort = App.VesselSettings.SMTPPort;
            //SendEmail.SMTPServerName = App.VesselSettings.SMTPServerName;

            //SendEmail.Send(App.VesselSettings.ToEmailAddress,
            //               App.VesselSettings.VesselName,
            //               "Test Email",
            //               "This is a test",
            //               "");
        }

        async Task PopulateGaugePageCollection()
        {
            IGaugeTable gaugeTable = BuildDBTables.GaugeTable;
            await gaugeTable.BeginEmpty();

            IGaugePageTable gaugePageTable = BuildDBTables.GaugePageTable;
            await gaugePageTable.BeginEmpty();

            App.GaugePageCollection = new GaugePageCollection();

            IGaugePageItem gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Engines";
            gaugePageItem.Position = 0;
            await App.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Tanks";
            gaugePageItem.Position = 1;
            await App.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "AC Electrical";
            gaugePageItem.Position = 2;
            await App.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "DC Electrical";
            gaugePageItem.Position = 3;
            await App.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Navigation";
            gaugePageItem.Position = 4;
            await App.GaugePageCollection.BeginAddPage(gaugePageItem);

            BuildDBTables.GaugePageTable.Load();
            App.GaugePageCollection = new GaugePageCollection();
            await App.GaugePageCollection.BeginLoad();
        }

        async Task PopulateDeviceCollection()
        {
            await BuildDBTables.DeviceTable.BeginEmpty();
            App.DeviceCollection.Clear();

            // Device 00
            IDeviceItem deviceItem = new DeviceItem();
            deviceItem.SerialNumber = Guid.NewGuid().ToString();
            deviceItem = App.DeviceCollection.Add(deviceItem);

            App.DeviceCollection.Clear();
            App.DeviceCollection.Load();
        }

        async Task PopulateSensorCollection()
        {
            await BuildDBTables.SensorTable.BeginEmpty();
            App.SensorCollection.Clear();

            // Sensor 00
            ISensorItem sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 01
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 02
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 03
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 04
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 05
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 06
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 07
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 08
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 09
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            // Sensor 10
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor = App.SensorCollection.Add(sensor);

            App.SensorCollection.Clear();
            App.SensorCollection.Load();
        }

        async Task PopulateGaugeCollection()
        {
            App.GaugeItemCollection.Clear();
            await BuildDBTables.GaugeTable.BeginEmpty();

            // gauge 0
            IGaugeItem gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[0].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 0;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 1
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[1].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 364;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 2
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[2].SensorId;
            gaugeItem.GaugeTop = 230;
            gaugeItem.GaugeLeft = 346;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 3
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[3].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 1199;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 4
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[4].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 1025;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 5
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[5].SensorId;
            gaugeItem.GaugeTop = 230;
            gaugeItem.GaugeLeft = 1042;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 0
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftTankGauge;
            gaugeItem.SensorId = App.SensorCollection[6].SensorId;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 540;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 1
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftTankGauge;
            gaugeItem.SensorId = App.SensorCollection[7].SensorId;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 680;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 2
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.RightTankGauge;
            gaugeItem.SensorId = App.SensorCollection[8].SensorId;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 760;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 3
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.RightTankGauge;
            gaugeItem.SensorId = App.SensorCollection[9].SensorId;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 870;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 0
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fuel\nPort";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 595;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 1
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fresh\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 735;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 2
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Black\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 820;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 3
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fuel\nStbd";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 925;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            BuildDBTables.GaugeTable.Load();
            App.GaugeItemCollection = new GaugeItemCollection();
            await App.GaugeItemCollection.BeginLoad();
        }

        async private void BuildGaugePages()
        {
            await this.PopulateDeviceCollection();
            await this.PopulateSensorCollection();
            await this.PopulateGaugePageCollection();
            await this.PopulateGaugeCollection();

            this.MainPivot.Items.Clear();

            // For each gaugepage, build the view and view model
            foreach (IGaugePageItem item in App.GaugePageCollection)
            {
                GaugePageViewModel viewModel = new GaugePageViewModel();
                viewModel.GaugePageItem = item;

                GaugePageView view = new GaugePageView();
                view.Rows = 3;
                view.Columns = 3;
                view.ViewModel = viewModel;

                Binding widthBinding = new Binding();
                widthBinding.Source = this.MainPivot;
                widthBinding.Path = new PropertyPath("ActualWidth");
                view.SetBinding(GaugePageView.WidthProperty, widthBinding);

                Binding heightBinding = new Binding();
                heightBinding.Source = this.MainPivot;
                heightBinding.Path = new PropertyPath("ActualHeight");
                view.SetBinding(GaugePageView.HeightProperty, heightBinding);

                PivotItem pivotItem = new PivotItem();
                pivotItem.Header = item.PageName;

                // Put the view into a Canvas
                Canvas canvas = new Canvas();
                canvas.Children.Add(view);

                pivotItem.Content = canvas;

                this.MainPivot.Items.Add(pivotItem);

                // Get all of the gauges for this page and tell the page to build itself.
                List<IGaugeItem> gaugeItemList = App.GaugeItemCollection.FindAllByPageId(item.PageId);
                if ((null != gaugeItemList) && (gaugeItemList.Count > 0))
                {
                    Messenger.Default.Send<List<IGaugeItem>>(gaugeItemList, "BuildGaugeItemList");
                }
            }
        }
    }
}
