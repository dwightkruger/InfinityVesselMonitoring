//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Utilities;
using InfinityVesselMonitoringSoftware;
using InfinityVesselMonitoringSoftware.Gauges;
using InfinityVesselMonitoringSoftware.Settings;
using InfinityVesselMonitoringSoftware.Settings.Views;
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VesselMonitoringSuite.Devices;
using VesselMonitoringSuite.Sensors;
using VesselMonitoringSuite.Views;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409
// Icons from https://www.flaticon.com
namespace VesselMonitoring
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private System.Threading.Semaphore _semaphore;

        public MainPage()
        {
            this.InitializeComponent();
            Size ds = DisplaySize.GetCurrentDisplaySize();

            const double titlebarHeight = 70;
            const double pivotHeaderHeight = 68;

            this.MainPageInnerGrid.Height = ds.Height - titlebarHeight - pivotHeaderHeight;
            this.MainPageInnerGrid.Width = ds.Width;

            DispatcherHelper.Initialize();

            // Track all unhandled exceptions
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.UnhandledException += ApplicationUnhandledException;

            Telemetry.TrackEvent("Application Started");

            // Specify the startup mode to be full screen.
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
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

        async private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Only allow for one instance of this application to be run on a given machine.
            bool isSemaphoneCreated = false;
            _semaphore = new System.Threading.Semaphore(1, 1, typeof(App).ToString(), out isSemaphoneCreated);
            if (!isSemaphoneCreated)
            {
                ContentDialog dialog = new ContentDialog()
                {
                    Title = "Duplicate Instance",
                    Content = "An instance of this program is already running.",
                    CloseButtonText = "Close Application"
                };

                await dialog.ShowAsync();

                Application.Current.Exit();
                return;
            }

            // Connect to the SQL database
            App.BuildDBTables = new SQLiteBuildDBTables();
            App.BuildDBTables.Directory = ApplicationData.Current.LocalFolder.Path + @"\" + typeof(App).ToString();
            InfinityGroup.VesselMonitoring.SQLiteDB.Utilities.CreateDirectory(App.BuildDBTables.Directory);
            App.BuildDBTables.DatabaseName = "InfinityGroupVesselMonitoring";
            await App.BuildDBTables.Build();

            this.BuildDemoGaugePages();

            await App.BuildDBTables.VesselSettingsTable.BeginEmpty();

            App.VesselSettings = new VesselSettingsItem();
            App.VesselSettings.VesselName = "MV Infinity";
            App.VesselSettings.FromEmailAddress = "";
            App.VesselSettings.FromEmailPassword = "";
            App.VesselSettings.ToEmailAddress = "dwightkruger@mvinfinity.com";
            App.VesselSettings.SMTPServerName = "smtp-mail.outlook.com";
            App.VesselSettings.SMTPPort = 587;
            App.VesselSettings.SMTPEncryptionMethod = 2; // SmtpConnectType.ConnectSTARTTLS
            App.VesselSettings.ThemeColor = ((SolidColorBrush) Application.Current.Resources["ApplicationForegroundThemeBrush"]).Color;

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

        async Task PopulateDemoGaugePageCollection()
        {
            await App.BuildDBTables.GaugeTable.BeginEmpty();
            await App.BuildDBTables.GaugePageTable.BeginEmpty();

            App.GaugePageCollection = new GaugePageCollection();

            IGaugePageItem gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Engines";
            gaugePageItem.Position = 0;
            await App.GaugePageCollection.BeginAdd(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Tanks";
            gaugePageItem.Position = 1;
            await App.GaugePageCollection.BeginAdd(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "AC Electrical";
            gaugePageItem.Position = 2;
            await App.GaugePageCollection.BeginAdd(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "DC Electrical";
            gaugePageItem.Position = 3;
            await App.GaugePageCollection.BeginAdd(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Navigation";
            gaugePageItem.Position = 4;
            await App.GaugePageCollection.BeginAdd(gaugePageItem);

            App.BuildDBTables.GaugePageTable.Load();
            App.GaugePageCollection = new GaugePageCollection();
            await App.GaugePageCollection.BeginLoad();
        }

        async Task PopulateDemoDeviceCollection()
        {
            await App.BuildDBTables.SensorTable.BeginEmpty();
            await App.BuildDBTables.DeviceTable.BeginEmpty();
            App.DeviceCollection.Clear();

            // Device 00
            IDeviceItem deviceItem = new DeviceItem();
            deviceItem.SerialNumber = Guid.NewGuid().ToString();
            deviceItem = await App.DeviceCollection.BeginAdd(deviceItem);

            App.DeviceCollection.Clear();
            await App.DeviceCollection.BeginLoad();
        }

        async Task PopulateDemoSensorCollection()
        {
            await App.BuildDBTables.SensorTable.BeginEmpty();
            await App.BuildDBTables.SensorDataTable.BeginEmpty();
            App.SensorCollection.Clear();

            // Sensor 00
            ISensorItem sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnitType = UnitType.Power;
            sensor.SensorUnits = Units.AmpHrs;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 01
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.Amps;
            sensor.SensorUnitType = UnitType.Current;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 02
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.Bar;
            sensor.SensorUnitType = UnitType.Pressure;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 03
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.Fahrenheit;
            sensor.SensorUnitType = UnitType.Temperature;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 04
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMeters;
            sensor.SensorUnitType = UnitType.Volume;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 05
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 06
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 07
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 08
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 09
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            // Sensor 10
            sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
            sensor.SerialNumber = Guid.NewGuid().ToString();
            sensor.SensorUnits = Units.CubicMetersPerHr;
            sensor.SensorUnitType = UnitType.VolumeFlow;
            sensor = await App.SensorCollection.BeginAdd(sensor);

            for (int i = 0; i < 100; i++)
            {
                sensor = new SensorItem(App.DeviceCollection[0].DeviceId);
                sensor.SerialNumber = Guid.NewGuid().ToString();
                sensor.SensorUnits = Units.CubicMetersPerHr;
                sensor.SensorUnitType = UnitType.VolumeFlow;
                sensor = await App.SensorCollection.BeginAdd(sensor);
            }

            App.SensorCollection.Clear();
            App.SensorCollection.Load();
        }

        async Task PopulateDemoGaugeCollection()
        {
            App.GaugeItemCollection.Clear();
            await App.BuildDBTables.GaugeTable.BeginEmpty();

            // gauge 0
            IGaugeItem gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[0].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 0;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            gaugeItem.Units = Units.AmpHrs;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 1
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[1].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 364;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            gaugeItem.Units = Units.Amps;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 2
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[2].SensorId;
            gaugeItem.GaugeTop = 230;
            gaugeItem.GaugeLeft = 346;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            gaugeItem.Units = Units.Bar;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 3
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[3].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 1199;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            gaugeItem.Units = Units.Celsius;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 4
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[4].SensorId;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 1025;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            gaugeItem.Units = Units.CubicMeters;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 5
            gaugeItem = new GaugeItem(App.GaugePageCollection[0].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.SensorId = App.SensorCollection[5].SensorId;
            gaugeItem.GaugeTop = 230;
            gaugeItem.GaugeLeft = 1042;
            gaugeItem.GaugeHeight = 190;
            gaugeItem.GaugeWidth = 190;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Divisions = 4;
            gaugeItem.MinorTicsPerMajorTic = 10;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 12;
            gaugeItem.UnitsFontSize = 12;
            gaugeItem.MajorTicLength = 12;
            gaugeItem.MiddleCircleDelta = 45;
            gaugeItem.InnerCircleDelta = 20;
            gaugeItem.Units = Units.CubicMetersPerHr;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 0
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.LeftTankGauge;
            gaugeItem.SensorId = App.SensorCollection[6].SensorId;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 540;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Units = Units.CubicMetersPerHr;
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
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Units = Units.CubicMetersPerHr;
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
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Units = Units.CubicMetersPerHr;
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
            gaugeItem.GaugeColor = App.VesselSettings.ThemeColor;
            gaugeItem.Units = Units.CubicMetersPerHr;
            gaugeItem.Resolution = 0;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 0
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fuel\nPort";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 595;
            gaugeItem.GaugeHeight = 60;
            gaugeItem.GaugeWidth = 60;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = App.VesselSettings.ThemeColor;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 1
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fresh\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 735;
            gaugeItem.GaugeHeight = 60;
            gaugeItem.GaugeWidth = 60;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = App.VesselSettings.ThemeColor;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 2
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Black\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 820;
            gaugeItem.GaugeHeight = 60;
            gaugeItem.GaugeWidth = 60;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = App.VesselSettings.ThemeColor;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 3
            gaugeItem = new GaugeItem(App.GaugePageCollection[1].PageId);
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.Text = "Fuel\nStbd";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 925;
            gaugeItem.GaugeHeight = 60;
            gaugeItem.GaugeWidth = 60;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = App.VesselSettings.ThemeColor;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await App.GaugeItemCollection.BeginAddGauge(gaugeItem);

            App.BuildDBTables.GaugeTable.Load();
            App.GaugeItemCollection = new GaugeItemCollection();
            await App.GaugeItemCollection.BeginLoad();
        }

        async private void BuildDemoGaugePages()
        {
            await this.PopulateDemoDeviceCollection();
            await this.PopulateDemoSensorCollection();
            await this.PopulateDemoGaugePageCollection();
            await this.PopulateDemoGaugeCollection();

            //App.BuildDBTables.DeviceTable.Load();
            //App.DeviceCollection.Clear();
            //await App.DeviceCollection.BeginLoad();

            //App.BuildDBTables.SensorTable.Load();
            //App.SensorCollection.Clear();
            //App.SensorCollection.Load();

            //App.BuildDBTables.GaugePageTable.Load();
            //App.GaugePageCollection.Clear();
            //await App.GaugePageCollection.BeginLoad();

            //App.BuildDBTables.GaugeTable.Load();
            //App.GaugeItemCollection.Clear();
            //await App.GaugeItemCollection.BeginLoad();

            this.MainPagePivot.Items.Clear();

            SettingsHomeView vesselSettingsHomeView = new SettingsHomeView();
            PivotItem vsPivotItem = new PivotItem();
            vsPivotItem.Header = "Settings";
            Canvas vscanvas = new Canvas();
            vscanvas.Children.Add(vesselSettingsHomeView);
            vsPivotItem.Content = vscanvas;
            this.MainPagePivot.Items.Add(vsPivotItem);

            // For each gauge page, build the view and view model
            foreach (IGaugePageItem item in App.GaugePageCollection)
            {
                GaugePageView view = new GaugePageView();
                view.Rows = 3;
                view.Columns = 3;
                view.ViewModel.GaugePageItem = item;

                Binding widthBinding = new Binding();
                widthBinding.Source = this.MainPagePivot;
                widthBinding.Path = new PropertyPath("ActualWidth");
                view.SetBinding(GaugePageView.WidthProperty, widthBinding);

                Binding heightBinding = new Binding();
                heightBinding.Source = this.MainPagePivot;
                heightBinding.Path = new PropertyPath("ActualHeight");
                view.SetBinding(GaugePageView.HeightProperty, heightBinding);

                // Put the view into a Canvas
                Canvas canvas = new Canvas();
                canvas.Children.Add(view);

                // Put the canvas into a PivotItem
                PivotItem pivotItem = new PivotItem();
                pivotItem.Header = item.PageName;
                pivotItem.Content = canvas;

                // Put the pivot item on the page
                this.MainPagePivot.Items.Add(pivotItem);

                // Get all of the gauges for this page and tell the page to build itself.
                List<IGaugeItem> gaugeItemList = await App.GaugeItemCollection.BeginFindAllByPageId(item.PageId);
                if ((null != gaugeItemList) && (gaugeItemList.Count > 0))
                {
                    Messenger.Default.Send<List<IGaugeItem>>(gaugeItemList, "BuildGaugeItemList");
                }
            }
        }

        /// <summary>
        /// Pivot controls trap key presses that we need to route to various pages. Pass them through via
        /// a message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainPagePivot_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (sender is Pivot)
            {
                Messenger.Default.Send<Tuple<int, KeyRoutedEventArgs>>(
                    Tuple.Create<int, KeyRoutedEventArgs>(MainPagePivot.SelectedIndex, e),
                    "MainPagePivot_KeyDown");
            }
        }

        public static async Task BeginShowNewWindow<TView>()
        {
            ApplicationView currentView = ApplicationView.GetForCurrentView();
            CoreApplicationView newView = CoreApplication.CreateNewView();
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Window newWindow = Window.Current;
                ApplicationView newAppView = ApplicationView.GetForCurrentView();
                newAppView.Title = "Sensor Editor";

                Frame frame = new Frame();
                frame.Navigate(typeof(TView), null);
                newWindow.Content = frame;
                newWindow.Activate();
    
                await ApplicationViewSwitcher.TryShowAsStandaloneAsync(
                    newAppView.Id,
                    ViewSizePreference.Custom,
                    currentView.Id,
                    ViewSizePreference.UseMinimum);
            });
        }
    }
}
