//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Controls.Converters;
using InfinityVesselMonitoringSoftware.Settings.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Extensions;
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
    public partial class SettingsHomeView : UserControl
    {
        private static ImageSource c_shipDark;
        private static ImageSource c_shipLight;
        private static ImageSource c_shipRed;
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
            // BUGBUG - we should figure out how to recolor an image on the fly.
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
            c_DarkBrush  = new SolidColorBrush(Colors.DarkGray);
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
            // If the theme colors change, we need to update the bitmaps and the backgrounds on the menu items.
            // BUGBUG we shold determine how to recolor images and not keep a copy of each color combination.
            Messenger.Default.Register<Color>(this, "OnThemeColorsChanged", (newColor) =>
            {
                if (newColor == Colors.White)
                {
                    VesselSettingsButton.Source = c_shipDark;
                    SensorsButton.Source        = c_sensorDark;
                    PagesButton.Source          = c_pagesDark;
                    DatabaseButton.Source       = c_databaseDark;

                    this.IsSelectedColor        = c_DarkBrush;
                }
                else if (newColor == Colors.Red)
                {
                    VesselSettingsButton.Source = c_shipRed;
                    SensorsButton.Source        = c_sensorRed;
                    PagesButton.Source          = c_pagesRed;
                    DatabaseButton.Source       = c_databaseRed;

                    this.IsSelectedColor        = c_NightBrush;
                }
                else
                {
                    VesselSettingsButton.Source = c_shipLight;
                    SensorsButton.Source        = c_sensorLight;
                    PagesButton.Source          = c_pagesLight;
                    DatabaseButton.Source       = c_databaseLight;

                    this.IsSelectedColor       = c_LightBrush;
                }
            });

            // Bind each of the TextBlock forground colors so that when someone moves between dark/light/night
            // moded the text blocks automatically update/
            //Binding textBlockBinding = new Binding();
            //textBlockBinding.Source = App.VesselSettings;
            //textBlockBinding.Path = new PropertyPath("ThemeForegroundColor");
            //textBlockBinding.Converter = c_ctscbc;

            //foreach (var child in MainViewbox.FindDescendants<TextBlock>())
            //{
            //    //child.SetBinding(TextBlock.ForegroundProperty, textBlockBinding);
            //}

            // Bind the background color for each of the toggle buttons so that we the theme changes, the background
            // color changes.
            Binding isSelectedColorBinding = new Binding();
            isSelectedColorBinding.Source = this;
            isSelectedColorBinding.Path = new PropertyPath("IsSelectedColor");

            foreach (var child in MainViewbox.FindDescendants<ImageToggleButton>())
            {
                child.SetBinding(ImageToggleButton.IsSelectedColorProperty, isSelectedColorBinding);
            }
        }

        public SettingsHomeViewModel ViewModel
        {
            get { return this.VM; }
        }

        #region IsSelectedColor
        public static readonly DependencyProperty IsSelectedColorProperty = DependencyProperty.Register(
            "IsSelectedColor",
            typeof(SolidColorBrush),
            typeof(SettingsHomeView),
            new PropertyMetadata(new SolidColorBrush(Colors.Magenta),
                                 new PropertyChangedCallback(OnIsSelectedColorPropertyChanged)));

        public SolidColorBrush IsSelectedColor
        {
            get { return (SolidColorBrush)this.GetValue(IsSelectedColorProperty); }
            set { this.SetValue(IsSelectedColorProperty, value); }
        }

        protected static void OnIsSelectedColorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsHomeView g = d as SettingsHomeView;
        }
        #endregion

    }
}
