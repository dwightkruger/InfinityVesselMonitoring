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
using Microsoft.Graphics.Canvas.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VesselMonitoringSuite.Sensors;
using VesselMonitoringSuite.ViewModels;
using VesselMonitoringSuite.Views;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

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
        public SensorCollection SensorCollection { get; set; }
        public GaugePageCollection GaugePageCollection { get; set; }
        public GaugeItemCollection GaugeItemCollection { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            //Globals.MediaPlayer = this.MediaPlayer;

            DispatcherHelper.Initialize();

            // Track all unhandled exceptions
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            Application.Current.UnhandledException += ApplicationUnhandledException;

            Telemetry.TrackEvent("Application Started");
            Telemetry.Flush();

            BuildDBTables.Directory = ApplicationData.Current.TemporaryFolder.Path;
            BuildDBTables.DatabaseName = "InfinityGroupVesselMonitoring";
            var x = new BuildDBTables();
            Task.Run(() => { x.DoIt().Wait(); }).Wait();

            //arcGauge00.Divisions = 7; arcGauge00.MinorTicsPerMajorTic = 5;  arcGauge00.MediumTicsPerMajorTic = 0;
            //arcGauge01.Divisions = 4; arcGauge01.MinorTicsPerMajorTic = 10; arcGauge01.MediumTicsPerMajorTic = 0;
            //arcGauge02.Divisions = 4; arcGauge02.MinorTicsPerMajorTic = 10; arcGauge02.MediumTicsPerMajorTic = 0;
            //arcGauge03.Divisions = 7; arcGauge03.MinorTicsPerMajorTic = 5;  arcGauge03.MediumTicsPerMajorTic = 0;
            //arcGauge04.Divisions = 4; arcGauge04.MinorTicsPerMajorTic = 10; arcGauge04.MediumTicsPerMajorTic = 0;
            //arcGauge05.Divisions = 4; arcGauge05.MinorTicsPerMajorTic = 10; arcGauge05.MediumTicsPerMajorTic = 0;
            //arcGauge06.Divisions = 6; arcGauge06.MinorTicsPerMajorTic = 10; arcGauge06.MediumTicsPerMajorTic = 0;
            //arcGauge07.Divisions = 5; arcGauge07.MinorTicsPerMajorTic = 5;  arcGauge07.MediumTicsPerMajorTic = 0;
            //arcGauge08.Divisions = 6; arcGauge08.MinorTicsPerMajorTic = 5;  arcGauge08.MediumTicsPerMajorTic = 0;
            //arcGauge09.Divisions = 5; arcGauge09.MinorTicsPerMajorTic = 10; arcGauge09.MediumTicsPerMajorTic = 0;

            //arcGauge00.Resolution = 0;
            //arcGauge01.Resolution = 0;
            //arcGauge02.Resolution = 0;
            //arcGauge03.Resolution = 0;
            //arcGauge04.Resolution = 0;
            //arcGauge05.Resolution = 0;
            //arcGauge06.Resolution = 0;
            //arcGauge07.Resolution = 0;
            //arcGauge08.Resolution = 0;
            //arcGauge09.Resolution = 0;

            //arcGauge00.IsOnline = true;
            //arcGauge01.IsOnline = true;
            //arcGauge02.IsOnline = true;
            //arcGauge03.IsOnline = true;
            //arcGauge04.IsOnline = true;
            //arcGauge05.IsOnline = true;
            //arcGauge06.IsOnline = true;
            //arcGauge07.IsOnline = true;
            //arcGauge08.IsOnline = true;
            //arcGauge09.IsOnline = true;

            //arcGauge00.MinValue = 0; arcGauge00.MaxValue = 35; arcGauge00.Units = "RPM\nx100";
            //arcGauge01.MinValue = 0; arcGauge01.MaxValue = 100; arcGauge01.Units = "Oil PSI";
            //arcGauge02.MinValue = 0; arcGauge02.MaxValue = 100; arcGauge02.Units = "Engine\nTemp.";
            //arcGauge03.MinValue = 0; arcGauge03.MaxValue = 35; arcGauge03.Units = "RPM\nx100";
            //arcGauge04.MinValue = 0; arcGauge04.MaxValue = 100; arcGauge04.Units = "Oil PSI";
            //arcGauge05.MinValue = 0; arcGauge05.MaxValue = 100; arcGauge05.Units = "Engine\nTemp.";
            //arcGauge06.MinValue = 200; arcGauge06.MaxValue = 260; arcGauge06.Units = "V";
            //arcGauge07.MinValue = 0; arcGauge07.MaxValue = 250; arcGauge07.Units = "Amps";
            //arcGauge08.MinValue = 18; arcGauge08.MaxValue = 30; arcGauge08.Units = "V";
            //arcGauge09.MinValue = 0; arcGauge09.MaxValue = 12; arcGauge09.Units = "Amps";

            //arcGauge00.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge01.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge02.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge03.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge04.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge05.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Visible;
            //arcGauge06.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            //arcGauge07.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            //arcGauge08.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Collapsed;
            //arcGauge09.GaugeOutlineVisibility = Windows.UI.Xaml.Visibility.Collapsed;

            //arcGauge01.MiddleCircleDelta = arcGauge02.MiddleCircleDelta = 45;
            //arcGauge01.InnerCircleDelta = arcGauge02.InnerCircleDelta = 20;

            //arcGauge04.MiddleCircleDelta = arcGauge05.MiddleCircleDelta = 45;
            //arcGauge04.InnerCircleDelta = arcGauge05.InnerCircleDelta = 20;

            //arcGauge06.MiddleCircleDelta = arcGauge08.MiddleCircleDelta = 70;
            //arcGauge06.InnerCircleDelta = arcGauge08.InnerCircleDelta = 20;

            //arcGauge07.MiddleCircleDelta = arcGauge09.MiddleCircleDelta = 60;
            //arcGauge07.InnerCircleDelta = arcGauge09.InnerCircleDelta = 15;

            //arcGauge00.ValueFontSize = arcGauge03.ValueFontSize = 18;
            //arcGauge00.UnitsFontSize = arcGauge03.UnitsFontSize = 14;

            //arcGauge01.ValueFontSize = arcGauge02.ValueFontSize = 12;
            //arcGauge04.ValueFontSize = arcGauge05.ValueFontSize = 12;
            //arcGauge01.UnitsFontSize = arcGauge02.UnitsFontSize = 12;
            //arcGauge04.UnitsFontSize = arcGauge05.UnitsFontSize = 12;


            //arcGauge01.majorTicLength = arcGauge02.majorTicLength = 12;
            //arcGauge04.majorTicLength = arcGauge05.majorTicLength = 12;

            //arcGauge06.ValueFontSize = arcGauge08.ValueFontSize = 14;
            //arcGauge06.majorTicLength = arcGauge08.majorTicLength = 12;
            //arcGauge06.UnitsFontSize = arcGauge08.UnitsFontSize = 14;

            //arcGauge07.ValueFontSize = arcGauge09.ValueFontSize = 12;
            //arcGauge07.majorTicLength = arcGauge09.majorTicLength = 12;
            //arcGauge07.UnitsFontSize = arcGauge09.UnitsFontSize = 12;

            ////tankGaugeLeft00.GaugeHeight = 330; tankGaugeLeft00.GaugeWidth = 140;
            ////tankGaugeLeft01.GaugeHeight = 330; tankGaugeLeft01.GaugeWidth = 140;
            ////tankGaugeRight00.GaugeHeight = 330; tankGaugeRight00.GaugeWidth = 140;
            ////tankGaugeRight01.GaugeHeight = 330; tankGaugeRight01.GaugeWidth = 140;

            ////tankGaugeLeft00.ValueFontSize = 11;
            ////tankGaugeLeft01.ValueFontSize = 11;
            ////tankGaugeRight00.ValueFontSize = 11;
            ////tankGaugeRight01.ValueFontSize = 11;

            ////tankGaugeLeft00.Resolution = 0;
            ////tankGaugeLeft01.Resolution = 0;
            ////tankGaugeRight00.Resolution = 0;
            ////tankGaugeRight01.Resolution = 0;

            //tankGaugeLeft00.TankType = TankType.DieselTank;
            //tankGaugeLeft01.TankType = TankType.FreshwaterTank;
            //tankGaugeRight00.TankType = TankType.BlackwaterTank;
            //tankGaugeRight01.TankType = TankType.DieselTank;

            //tankGaugeLeft00.Value = 60;
            //tankGaugeLeft01.Value = 60;
            //tankGaugeRight00.Value = 60;
            //tankGaugeRight01.Value = 60;

            //textControl00.Text = "Fuel\nPort";
            ////textControl00.TextFontSize = 13;
            //textControl00.TextFontColor = Colors.CornflowerBlue;
            //textControl00.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            //textControl00.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            ////textControl00.Left = 65;
            ////textControl00.Top = 10;

            //textControl01.Text = "Fresh\nWater";
            ////textControl01.TextFontSize = 13;
            //textControl01.TextFontColor = Colors.CornflowerBlue;
            //textControl01.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            //textControl01.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            ////textControl01.Left = 95;
            ////textControl01.Top = 10;

            //textControl02.Text = "Black\nWater";
            ////textControl02.TextFontSize = 13;
            //textControl02.TextFontColor = Colors.CornflowerBlue;
            //textControl02.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            //textControl02.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            ////textControl02.Left = 20;
            ////textControl02.Top = 10;

            //textControl03.Text = "Fuel\nStbd";
            ////textControl03.TextFontSize = 13;
            //textControl03.TextFontColor = Colors.CornflowerBlue;
            //textControl03.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            //textControl03.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            ////textControl03.Left = 40;
            ////textControl03.Top = 10;

            //PolyLineControl00.Points.Add(new Point(470, 524));
            //PolyLineControl00.Points.Add(new Point(520, 524));
            //PolyLineControl00.Points.Add(new Point(510, 524));
            //PolyLineControl00.Points.Add(new Point(510, 490));
            //PolyLineControl00.Points.Add(new Point(020, 490));
            //PolyLineControl00.Points.Add(new Point(020, 859));
            //PolyLineControl00.Points.Add(new Point(180, 859));
            //PolyLineControl00.Points.Add(new Point(220, 750));
            //PolyLineControl00.Points.Add(new Point(512, 640));
            //PolyLineControl00.Points.Add(new Point(512, 610));
            //PolyLineControl00.Points.Add(new Point(494, 610));
            //PolyLineControl00.Points.Add(new Point(544, 610));
            //PolyLineControl00.PolyLineColor = Colors.CornflowerBlue;
            //PolyLineControl00.PolyLineStrokeWidth = 5;

            //textControl04.Text = "AC\nPower";
            //textControl04.TextFontSize = 18;
            //textControl04.TextFontColor = Colors.CornflowerBlue;
            //textControl04.TextHorizontalAlignment = CanvasHorizontalAlignment.Center;
            //textControl04.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            //textControl04.X = 510;
            //textControl04.Y = 537;

            //_valueTimer = new Timer(ValueTimerTic, 0, 5000, 2000);

            this.BuildGaugePages();

            Windows.Storage.StorageFile sampleFile = null;
            //Task.Run(async () =>
            //{
            //    sampleFile = await
            //    StorageFile.GetFileFromPathAsync(@"E:\Visual Studio Projects\InfinityVesselMonitoringSoftware\InfinityVesselMonitoringSoftware\bin\x86\Debug\Alert.wav");
            //}).Wait();

            Task.Run(async () =>
            {
                Uri uri = new Uri("ms-appx:///Alert.wav");
                sampleFile = await
                StorageFile.GetFileFromApplicationUriAsync(uri);
            }).Wait();


            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromStorageFile(sampleFile);
            mediaPlayer.Play();
        }

        private void ValueTimerTic(object stateInfo)
        {
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                //arcGauge00.Value = randu.Next((int)arcGauge00.MinValue, (int)arcGauge00.MaxValue); 
                //arcGauge01.Value = randu.Next((int)arcGauge01.MinValue, (int)arcGauge01.MaxValue); 
                //arcGauge02.Value = randu.Next((int)arcGauge02.MinValue, (int)arcGauge02.MaxValue); 
                //arcGauge03.Value = randu.Next((int)arcGauge03.MinValue, (int)arcGauge03.MaxValue); 
                //arcGauge04.Value = randu.Next((int)arcGauge04.MinValue, (int)arcGauge04.MaxValue); 
                //arcGauge05.Value = randu.Next((int)arcGauge05.MinValue, (int)arcGauge05.MaxValue); 
                //arcGauge06.Value = randu.Next((int)arcGauge06.MinValue, (int)arcGauge06.MaxValue); 
                //arcGauge07.Value = randu.Next((int)arcGauge07.MinValue, (int)arcGauge07.MaxValue); 
                //arcGauge08.Value = randu.Next((int)arcGauge08.MinValue, (int)arcGauge08.MaxValue); 
                //arcGauge09.Value = randu.Next((int)arcGauge09.MinValue, (int)arcGauge09.MaxValue); 

                //tankGaugeLeft00.Value = randu.Next((int)tankGaugeLeft00.MinValue, (int)tankGaugeLeft00.MaxValue);
                //tankGaugeLeft01.Value = randu.Next((int)tankGaugeLeft01.MinValue, (int)tankGaugeLeft01.MaxValue);

                //tankGaugeRight00.Value = randu.Next((int)tankGaugeRight00.MinValue, (int)tankGaugeRight00.MaxValue);
                //tankGaugeRight01.Value = randu.Next((int)tankGaugeRight01.MinValue, (int)tankGaugeRight01.MaxValue);
            });
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

        /// <summary>
        /// The window is closing. We need to flush out all remaining AppInsights telemetry.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Telemetry.Flush(); // only for desktop apps

            // Allow time for flushing:
            //System.Threading.Thread.Sleep(1000);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //this.MainGrid.Height = e.NewSize.Height;
            //this.MainGrid.Width = e.NewSize.Width;
        }

        bool _isSwiped = false;

        /// <summary>
        /// Process swipe actions left, right, up & down.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SwipeableGrid_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            const float minDistance = 20;

            if (e.IsInertial && !_isSwiped)
            {
                var swipedDistanceX = e.Cumulative.Translation.X;
                var swipedDistanceY = e.Cumulative.Translation.Y;

                // Pick the larger of the X/Y swipe distance
                if (Math.Abs(swipedDistanceX) > Math.Abs(swipedDistanceY))
                {
                    swipedDistanceY = 0;
                }
                else
                {
                    swipedDistanceX = 0;
                }

                if (Math.Abs(swipedDistanceX) <= minDistance && Math.Abs(swipedDistanceY) <= minDistance) return;

                if (swipedDistanceX > minDistance)
                {
                    //SwipeableTextBlock.Text = "Right Swiped (" + swipedDistanceX.ToString("F0") + "," + swipedDistanceY.ToString("F0") + ")";
                }
                else if (swipedDistanceX < -minDistance)
                {
                    //SwipeableTextBlock.Text = "Left Swiped (" + swipedDistanceX.ToString("F0") + "," + swipedDistanceY.ToString("F0") + ")";
                }
                else if (swipedDistanceY < minDistance)
                {
                    //SwipeableTextBlock.Text = "Up Swiped (" + swipedDistanceX.ToString("F0") + "," + swipedDistanceY.ToString("F0") + ")";
                }
                else if (swipedDistanceY > -minDistance)
                {
                    //SwipeableTextBlock.Text = "Down Swiped (" + swipedDistanceX.ToString("F0") + "," + swipedDistanceY.ToString("F0") + ")";
                }

                _isSwiped = true;
            }
        }

        private void SwipeableGrid_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            _isSwiped = false;
        }

        async Task PopulateGaugePageCollection()
        {
            IGaugeTable gaugeTable = BuildDBTables.GaugeTable;
            await gaugeTable.BeginEmpty();

            IGaugePageTable gaugePageTable = BuildDBTables.GaugePageTable;
            await gaugePageTable.BeginEmpty();

            this.GaugePageCollection = new GaugePageCollection();

            IGaugePageItem gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Engines";
            gaugePageItem.Position = 0;
            await this.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Tanks";
            gaugePageItem.Position = 1;
            await this.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "AC Electrical";
            gaugePageItem.Position = 2;
            await this.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "DC Electrical";
            gaugePageItem.Position = 3;
            await this.GaugePageCollection.BeginAddPage(gaugePageItem);

            gaugePageItem = new GaugePageItem();
            gaugePageItem.PageName = "Navigation";
            gaugePageItem.Position = 4;
            await this.GaugePageCollection.BeginAddPage(gaugePageItem);

            BuildDBTables.GaugePageTable.Load();
            this.GaugePageCollection = new GaugePageCollection();
            await this.GaugePageCollection.BeginLoad();
        }

        async Task PopulateGaugeCollection()
        {
            IGaugeTable gaugeTable = BuildDBTables.GaugeTable;
            await gaugeTable.BeginEmpty();

            this.GaugeItemCollection = new GaugeItemCollection();

            // gauge 0
            IGaugeItem gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 0;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 1
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
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
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 2
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
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
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 3
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 0;
            gaugeItem.GaugeLeft = 1199;
            gaugeItem.GaugeHeight = 380;
            gaugeItem.GaugeWidth = 380;
            gaugeItem.Divisions = 7;
            gaugeItem.MinorTicsPerMajorTic = 5;
            gaugeItem.MediumTicsPerMajorTic = 0;
            gaugeItem.ValueFontSize = 18;
            gaugeItem.UnitsFontSize = 14;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 4
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
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
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // gauge 5
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
            gaugeItem.PageId = this.GaugePageCollection[0].PageId;
            gaugeItem.SensorId = 0;
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
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 0
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftTankGauge;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 540;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 1
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.LeftTankGauge;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 680;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 2
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.RightTankGauge;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 760;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // tank gauge 3
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.RightTankGauge;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.SensorId = 0;
            gaugeItem.GaugeTop = 50;
            gaugeItem.GaugeLeft = 870;
            gaugeItem.GaugeHeight = 330;
            gaugeItem.GaugeWidth = 140;
            gaugeItem.Resolution = 0;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 0
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.Text = "Fuel\nPort";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 595;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 1
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.Text = "Fresh\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 735;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 2
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.Text = "Black\nWater";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 820;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            // text control 3
            gaugeItem = new GaugeItem();
            gaugeItem.GaugeType = GaugeTypeEnum.TextControl;
            gaugeItem.PageId = this.GaugePageCollection[1].PageId;
            gaugeItem.Text = "Fuel\nStbd";
            gaugeItem.GaugeTop = 10;
            gaugeItem.GaugeLeft = 925;
            gaugeItem.TextFontSize = 13;
            gaugeItem.TextFontColor = Colors.CornflowerBlue;
            gaugeItem.TextHorizontalAlignment = CanvasHorizontalAlignment.Left;
            gaugeItem.TextVerticalAlignment = CanvasVerticalAlignment.Top;
            await this.GaugeItemCollection.BeginAddGauge(gaugeItem);

            BuildDBTables.GaugeTable.Load();
            this.GaugeItemCollection = new GaugeItemCollection();
            await this.GaugeItemCollection.BeginLoad();
        }

        async private void BuildGaugePages()
        {
            await this.PopulateGaugePageCollection();
            await this.PopulateGaugeCollection();

            this.MainPivot.Items.Clear();

            // For each gaugepage, build the view and view model
            foreach (IGaugePageItem item in this.GaugePageCollection)
            {
                GaugePageViewModel viewModel = new GaugePageViewModel();
                viewModel.GaugePageItem = item;

                GaugePageView view = new GaugePageView();
                view.ViewModel = viewModel;

                PivotItem pivotItem = new PivotItem();
                pivotItem.Header = item.PageName;

                // Put the view into a Canvas
                Canvas canvas = new Canvas();
                canvas.Children.Add(view);

                pivotItem.Content = canvas;

                this.MainPivot.Items.Add(pivotItem);

                // Get all of the gauges for this page and tell the page to build itself.
                List<IGaugeItem> gaugeItemList = this.GaugeItemCollection.FindAllByPageId(item.PageId);
                if ((null != gaugeItemList) && (gaugeItemList.Count > 0))
                {
                    Messenger.Default.Send<List<IGaugeItem>>(gaugeItemList, "BuildGaugeItemList");
                }
            }
        }
    }
}
