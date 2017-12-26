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
    public sealed partial class SettingsHomeView : UserControl
    {
        private static ImageSource c_shipDark;
        private static ImageSource c_shipLight;        private static ImageSource c_shipRed;
        private static ImageSource c_databaseDark;
        private static ImageSource c_databaseLight;
        private static ImageSource c_databaseRed;
        private static ImageSource c_pagesDark;
        private static ImageSource c_pagesLight;
        private static ImageSource c_pagesRed;
        private static ImageSource c_sensorDark;
        private static ImageSource c_sensorLight;
        private static ImageSource c_sensorRed;
        private static SolidColorBrush c_LightBrush;
        private static SolidColorBrush c_DarkBrush;
        private static SolidColorBrush c_NightBrush;
        private static ColorToSolidColorBrushConverter c_ctscbc = new ColorToSolidColorBrushConverter();

        static SettingsHomeView()
        {
            c_shipDark      = ImageFromUri("ms-appx:///Graphics/Ship-dark.png");
            c_shipLight     = ImageFromUri("ms-appx:///Graphics/Ship-light.png");
            c_shipRed       = ImageFromUri("ms-appx:///Graphics/Ship-red.png");
            c_databaseDark  = ImageFromUri("ms-appx:///Graphics/Database-dark.png");
            c_databaseLight = ImageFromUri("ms-appx:///Graphics/Database-light.png");
            c_databaseRed   = ImageFromUri("ms-appx:///Graphics/Database-red.png");
            c_pagesDark     = ImageFromUri("ms-appx:///Graphics/Pages-dark.png");
            c_pagesLight    = ImageFromUri("ms-appx:///Graphics/Pages-light.png");
            c_pagesRed      = ImageFromUri("ms-appx:///Graphics/Pages-red.png");
            c_sensorDark    = ImageFromUri("ms-appx:///Graphics/Sensor-dark.png");
            c_sensorLight   = ImageFromUri("ms-appx:///Graphics/Sensor-light.png");
            c_sensorRed     = ImageFromUri("ms-appx:///Graphics/Sensor-red.png");

            c_LightBrush = new SolidColorBrush(Colors.LightGray);
            c_DarkBrush = new SolidColorBrush(Colors.DarkGray);
            c_NightBrush = new SolidColorBrush(Colors.DarkRed);
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
                if (newColor == Colors.White)
                {
                    VesselSettingsButton.Source = c_shipDark;
                    SensorsButton.Source        = c_sensorDark;
                    PagesButton.Source          = c_pagesDark;
                    DatabaseButton.Source       = c_databaseDark;

                    VesselSettingsButton.IsSelectedColor = c_DarkBrush;
                    SensorsButton.IsSelectedColor = c_DarkBrush;
                    PagesButton.IsSelectedColor = c_DarkBrush;
                    DatabaseButton.IsSelectedColor = c_DarkBrush;
                }
                else if (newColor == Colors.Red)
                {
                    VesselSettingsButton.Source = c_shipRed;
                    SensorsButton.Source        = c_sensorRed;
                    PagesButton.Source          = c_pagesRed;
                    DatabaseButton.Source       = c_databaseRed;

                    VesselSettingsButton.IsSelectedColor = c_NightBrush;
                    SensorsButton.IsSelectedColor = c_NightBrush;
                    PagesButton.IsSelectedColor = c_NightBrush;
                    DatabaseButton.IsSelectedColor = c_NightBrush;
                }
                else
                {
                    VesselSettingsButton.Source = c_shipLight;
                    SensorsButton.Source        = c_sensorLight;
                    PagesButton.Source          = c_pagesLight;
                    DatabaseButton.Source       = c_databaseLight;

                    VesselSettingsButton.IsSelectedColor = c_LightBrush;
                    SensorsButton.IsSelectedColor = c_LightBrush;
                    PagesButton.IsSelectedColor = c_LightBrush;
                    DatabaseButton.IsSelectedColor = c_LightBrush;
                }
            });

            Binding textBlockBinding = new Binding();
            textBlockBinding.Source = App.VesselSettings;
            textBlockBinding.Path = new PropertyPath("ThemeForegroundColor");
            textBlockBinding.Converter = new ColorToSolidColorBrushConverter();

            this.VesselSettingsButton.SetBinding(TextBlock.ForegroundProperty, textBlockBinding);
            this.SensorsButton.SetBinding(TextBlock.ForegroundProperty, textBlockBinding);
            this.PagesButton.SetBinding(TextBlock.ForegroundProperty, textBlockBinding);
            this.DatabaseButton.SetBinding(TextBlock.ForegroundProperty, textBlockBinding);
        }

        public SettingsHomeViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
