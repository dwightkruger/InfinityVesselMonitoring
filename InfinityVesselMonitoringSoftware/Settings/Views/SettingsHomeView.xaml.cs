//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls.Converters;
using InfinityVesselMonitoringSoftware.Settings.ViewModels;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfinityVesselMonitoringSoftware.Settings.Views
{
    public sealed partial class SettingsHomeView : Page
    {
        private static BitmapImage c_shipDark;
        private static BitmapImage c_shipLight;
        private static BitmapImage c_databaseDark;
        private static BitmapImage c_databaseLight;
        private static BitmapImage c_pagesDark;
        private static BitmapImage c_pagesLight;
        private static BitmapImage c_sensorDark;
        private static BitmapImage c_sensorLight;
        private static ColorToSolidColorBrushConverter c_ctscbc = new ColorToSolidColorBrushConverter();

        static SettingsHomeView()
        {
            c_shipDark      = ImageFromUri("ms-appx:///Graphics/Ship-dark.png");
            c_shipLight     = ImageFromUri("ms-appx:///Graphics/Ship-light.png");
            c_databaseDark  = ImageFromUri("ms-appx:///Graphics/Ship-dark.png");
            c_databaseLight = ImageFromUri("ms-appx:///Graphics/Ship-light.png");
            c_pagesDark     = ImageFromUri("ms-appx:///Graphics/Pages-dark.png");
            c_pagesLight    = ImageFromUri("ms-appx:///Graphics/Pages-light.png");
            c_sensorDark    = ImageFromUri("ms-appx:///Graphics/Sensor-dark.png");
            c_sensorLight   = ImageFromUri("ms-appx:///Graphics/Sensor-light.png");
        }

        private static BitmapImage ImageFromUri(string uriString)
        {
            Uri uri = new Uri(uriString);
            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.UriSource = uri;

            return bitmapImage;
        }

        public SettingsHomeView()
        {
            this.InitializeComponent();
            this.Loaded += SettingsHomeView_Loaded;
        }

        private void SettingsHomeView_Loaded(object sender, RoutedEventArgs e)
        {
            // If the theme colors change, we need to update the bitmaps.
            Messenger.Default.Register<Color>(this, "OnThemeColorsChanged", (newColor) =>
            {
                if ((newColor == Colors.Red) || (newColor == Colors.White))
                {
                    VesselSettingsImage.Source = c_shipDark;
                    SensorImage.Source         = c_sensorDark;
                    PagesImage.Source          = c_pagesDark;
                    DatabaseImage.Source       = c_databaseDark;
                }
                else
                {
                    VesselSettingsImage.Source = c_shipLight;
                    SensorImage.Source         = c_sensorLight;
                    PagesImage.Source          = c_pagesLight;
                    DatabaseImage.Source       = c_databaseLight;
                }
            });
        }

        public SettingsHomeViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
