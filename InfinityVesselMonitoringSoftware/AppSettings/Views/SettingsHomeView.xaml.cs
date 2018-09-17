//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Controls;
using InfinityGroup.VesselMonitoring.Controls.Converters;
using InfinityVesselMonitoringSoftware.AppSettings.ViewModels;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Foundation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfinityVesselMonitoringSoftware.AppSettings.Views
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
        private static ImageSource c_exitDark;
        private static ImageSource c_exitLight;
        private static ImageSource c_exitRed;
        private static ImageSource c_fullscreenDark;
        private static ImageSource c_fullscreenLight;
        private static ImageSource c_fullscreenRed;

        private static SolidColorBrush c_LightBrush;
        private static SolidColorBrush c_DarkBrush;
        private static SolidColorBrush c_NightBrush;
        private static ColorToSolidColorBrushConverter c_ctscbc = new ColorToSolidColorBrushConverter();

        static SettingsHomeView()
        {
            // BUGBUG - we should figure out how to recolor an image on the fly.
            c_shipDark        = ImageFromUri("ms-appx:///Graphics/Ship-dark.png");
            c_shipLight       = ImageFromUri("ms-appx:///Graphics/Ship-light.png");
            c_shipRed         = ImageFromUri("ms-appx:///Graphics/Ship-red.png");
            c_databaseDark    = ImageFromUri("ms-appx:///Graphics/Database-dark.png");
            c_databaseLight   = ImageFromUri("ms-appx:///Graphics/Database-light.png");
            c_databaseRed     = ImageFromUri("ms-appx:///Graphics/Database-red.png");
            c_pagesDark       = ImageFromUri("ms-appx:///Graphics/Pages-dark.png");
            c_pagesLight      = ImageFromUri("ms-appx:///Graphics/Pages-light.png");
            c_pagesRed        = ImageFromUri("ms-appx:///Graphics/Pages-red.png");
            c_sensorDark      = ImageFromUri("ms-appx:///Graphics/Sensor-dark.png");
            c_sensorLight     = ImageFromUri("ms-appx:///Graphics/Sensor-light.png");
            c_sensorRed       = ImageFromUri("ms-appx:///Graphics/Sensor-red.png");
            c_exitDark        = ImageFromUri("ms-appx:///Graphics/Exit-dark.png");
            c_exitLight       = ImageFromUri("ms-appx:///Graphics/Exit-light.png");
            c_exitRed         = ImageFromUri("ms-appx:///Graphics/Exit-red.png");
            c_fullscreenDark  = ImageFromUri("ms-appx:///Graphics/Fullscreen-dark.png");
            c_fullscreenLight = ImageFromUri("ms-appx:///Graphics/Fullscreen-light.png");
            c_fullscreenRed   = ImageFromUri("ms-appx:///Graphics/Fullscreen-red.png");

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
                this.ResetImageColors();
            });

            // Bind the background color for each of the toggle buttons so that we the theme changes, the background
            // color changes.
            Binding isSelectedColorBinding = new Binding();
            isSelectedColorBinding.Source = this;
            isSelectedColorBinding.Path = new PropertyPath("IsSelectedColor");

            foreach (var child in MainViewbox.FindDescendants<ImageToggleButton>())
            {
                child.SetBinding(ImageToggleButton.IsSelectedColorProperty, isSelectedColorBinding);
            }

            this.ResetImageColors();
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

        private void ResetImageColors()
        {
            if (App.VesselSettings.ThemeForegroundColor == Colors.White)
            {
                //await RecolorImage(c_redEffect, "ms-appx:///Graphics/Fullscreen-light.png", VesselSettingsButton.SwapChainPanel);

                VesselSettingsButton.Source = c_shipDark;
                SensorsButton.Source = c_sensorDark;
                PagesButton.Source = c_pagesDark;
                DatabaseButton.Source = c_databaseDark;
                ExitButton.Source = c_exitDark;
                FullscreenButton.Source = c_fullscreenDark;

                this.IsSelectedColor = c_DarkBrush;
            }
            else if (App.VesselSettings.ThemeForegroundColor == Colors.Red)
            {
                //await RecolorImage(Colors.Red, new Uri("ms-appx:///Graphics/Ship-light.png"), VesselSettingsButton.Image);

                VesselSettingsButton.Source = c_shipRed;
                SensorsButton.Source = c_sensorRed;
                PagesButton.Source = c_pagesRed;
                DatabaseButton.Source = c_databaseRed;
                FullscreenButton.Source = c_fullscreenRed;
                ExitButton.Source = c_exitRed;

                this.IsSelectedColor = c_NightBrush;
            }
            else
            {
                //await RecolorImage(Colors.Red, new Uri("ms-appx:///Graphics/Ship-light.png"), VesselSettingsButton.Image);

                VesselSettingsButton.Source = c_shipLight;
                SensorsButton.Source = c_sensorLight;
                PagesButton.Source = c_pagesLight;
                DatabaseButton.Source = c_databaseLight;
                FullscreenButton.Source = c_fullscreenLight;
                ExitButton.Source = c_exitLight;

                this.IsSelectedColor = c_LightBrush;
            }
        }

        //async private Task RecolorImage(ColorAdjustEffect colorAdjustEffect, string imagePath, SwapChainPanel swapChainPanel)
        //{
        //}

        /// <summary>
        /// Allow the user to set the app to fully screen mode, thus hiding the title bar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            this.FullscreenButton.IsSelected = !this.FullscreenButton.IsSelected;

            // Specify the startup mode to be full screen.
            if (this.FullscreenButton.IsSelected.Value)
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
            }
            else
            {
                Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().ExitFullScreenMode();
            }
        }
    }

    public static class BufferExtensions
    {
        //internal class AsyncOperationBufferProvider : IBufferProvider
        //{
        //    private readonly Task<IBuffer> m_bufferTask;

        //    public AsyncOperationBufferProvider(Task<IBuffer> bufferTask)
        //    {
        //        m_bufferTask = bufferTask;
        //    }

        //    public IAsyncOperation<IBuffer> GetAsync()
        //    {
        //        return m_bufferTask.AsAsyncOperation();
        //    }
        //}

        ///// <summary>
        ///// Adapts the Task&lt;IBuffer&gt; to work as an IBufferProvider suitable for BufferProviderImageSource.
        ///// </summary>
        ///// <param name="bufferTask">An asynchronous task that will result in an IBuffer containing an image.</param>
        ///// <returns>An IBufferProvider.</returns>
        //public static IBufferProvider AsBufferProvider(this Task<IBuffer> bufferTask)
        //{
        //    return new AsyncOperationBufferProvider(bufferTask);
        //}

        /// <summary>
        /// Adapts the IAsyncOperation&lt;IBuffer&gt; to work as an IBufferProvider suitable for BufferProviderImageSource.
        /// </summary>
        /// <param name="bufferAsyncOperation">An asynchronous operation that will result in an IBuffer containing an image.</param>
        /// <returns>An IBufferProvider.</returns>
        //public static IBufferProvider AsBufferProvider(this IAsyncOperation<IBuffer> bufferAsyncOperation)
        //{
        //    return new AsyncOperationBufferProvider(bufferAsyncOperation.AsTask());
        //}
    }
}
