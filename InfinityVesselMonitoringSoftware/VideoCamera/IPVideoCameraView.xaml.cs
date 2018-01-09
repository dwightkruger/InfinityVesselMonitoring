//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityVesselMonitoringSoftware.VideoCamera
{
    public partial class IPVideoCameraView : UserControl
    {
        private DateTime _lastUpdated = DateTime.MinValue;  // Last time camera updated
        private Timer _cameraDeadmanTimer;                  // Job to detect dead cameras
        private bool _isEnabled;                            // Is the camer enabled?
        private IPAddress _cameraIPAddress;                 // The video camera's up address

        public IPVideoCameraView()
        {
            this.InitializeComponent();
            this.VideoCamera = new AxisVAPIXVersion3();

            // Setup handlers for the touch and mouse events used to drag this window
            this.videoImage.ManipulationStarted += VideoImage_ManipulationStarted;
            this.videoImage.ManipulationDelta += VideoImage_ManipulationDelta;
            this.videoImage.ManipulationInertiaStarting += VideoImage_ManipulationInertiaStarting;

            NetworkChange.NetworkAddressChanged += NetworkChange_NetworkAddressChanged;

            this.Stop();
        }

        public void Start(IPAddress myCameraIPAddress)
        {
            _cameraIPAddress = myCameraIPAddress;
            this.VideoCamera.Initialize(_cameraIPAddress);
            this.VideoCamera.FrameReady += mjpeg_FrameReady;
            this.VideoCamera.Start();

            this.EnableCameraDeadmanTimer();

            _isEnabled = true;
        }

        /// <summary>
        /// Stop the camera feed
        /// </summary>
        public void Stop()
        {
            _isEnabled = false;
            this.VideoCamera.FrameReady -= mjpeg_FrameReady;
            this.VideoCamera.Stop();

            _lastUpdated = DateTime.Now;

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.noVideoFeedAvailableGrid.Visibility = Visibility.Visible;
                this.videoImage.Opacity = 0.3;
                this.DisableCameraDeadmanTimer();
            });
        }

        private void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            this.Stop();
            this.Start(_cameraIPAddress);
        }

        public void PanLeft()
        {
            this.VideoCamera.PanLeft();
        }

        public void PanRight()
        {
            this.VideoCamera.PanRight();
        }

        public void TiltUp()
        {
            this.VideoCamera.TiltUp();
        }

        public void TiltDown()
        {
            this.VideoCamera.TiltDown();
        }

        public void ZoomIn()
        {
            this.VideoCamera.ZoomIn();
        }

        public void ZoomOut()
        {
            this.VideoCamera.ZoomOut();
        }

        private IVideoCamera VideoCamera { get; set; }

        private void VideoImage_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            if (e.Delta.Scale < 0) this.ZoomOut();
            else if (e.Delta.Scale > 0) this.ZoomIn();

            if (e.Delta.Translation.X < 0) this.PanLeft();
            else if (e.Delta.Translation.X > 0) this.PanRight();

            if (e.Delta.Translation.Y < 0) this.TiltUp();
            else if (e.Delta.Translation.Y > 0) this.TiltDown();

            if (e.IsInertial)
            {
                e.Complete();
            }

            e.Handled = true;
        }

        private void VideoImage_ManipulationInertiaStarting(object sender, ManipulationInertiaStartingRoutedEventArgs e)
        {
            // Decrease the velocity of the Rectangle's movement by 
            // 10 inches per second every second.
            // (10 inches * 96 pixels per inch / 1000ms^2)
            e.TranslationBehavior.DesiredDeceleration = 10.0 * 96.0 / (1000.0 * 1000.0);

            // Decrease the velocity of the Rectangle's resizing by 
            // 0.1 inches per second every second.
            // (0.1 inches * 96 pixels per inch / (1000ms^2)
            e.ExpansionBehavior.DesiredDeceleration = 0.1 * 96 / (1000.0 * 1000.0);

            // Decrease the velocity of the Rectangle's rotation rate by 
            // 2 rotations per second every second.
            // (2 * 360 degrees / (1000ms^2)
            e.RotationBehavior.DesiredDeceleration = 720 / (1000.0 * 1000.0);

            e.Handled = true;
        }

        private void VideoImage_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Enable the camera deadman timer.
        /// </summary>
        public void EnableCameraDeadmanTimer()
        {
            _cameraDeadmanTimer = new System.Threading.Timer(
                this.CameraDeadmanTimerJob,
                null,
                10 * 1000,
                System.Threading.Timeout.Infinite);
        }

        /// <summary>
        /// Disable the camera deadman timers
        /// </summary>
        public void DisableCameraDeadmanTimer()
        {
            if (_cameraDeadmanTimer != null)
            {
                _cameraDeadmanTimer.Change(0, System.Threading.Timeout.Infinite);
            }
        }

        #region Sensor
        public static readonly DependencyProperty SensorProperty = DependencyProperty.Register(
            "Sensor",
            typeof(ISensorItem),
            typeof(IPVideoCameraView),
            new PropertyMetadata(null,
                                  new PropertyChangedCallback(OnSensorPropertyChanged)));

        public ISensorItem Sensor
        {
            get { return (ISensorItem)this.GetValue(SensorProperty); }
            set { this.SetValue(SensorProperty, value); }
        }

        protected static void OnSensorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            IPVideoCameraView g = d as IPVideoCameraView;
        }
        #endregion

        /// <summary>
        /// Periodically determine if we are still getting images from the video
        /// camera.  If not, display a message on the screen. This can come from
        /// a worker thread, so dispatch the changes to the UI thread.
        /// </summary>
        /// <param name="stateInfo"></param>
        async private void CameraDeadmanTimerJob(object stateInfo)
        {
            try
            {
                if (_isEnabled && (DateTime.Now - _lastUpdated) > TimeSpan.FromSeconds(4))
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        this.noVideoFeedAvailableGrid.Visibility = Visibility.Visible;
                        this.videoImage.Opacity = 0.3;
                    });
                }

                Telemetry.TrackEvent("Video camera at " +
                            _cameraIPAddress.ToString() +
                            " seems to have gone off-line. Trying to reconnect");

                this.VideoCamera.Stop();

                await Task.Delay(TimeSpan.FromSeconds(1));
                this.VideoCamera.Start();

                // Retry again in 30 seconds
                _lastUpdated = DateTime.Now.AddSeconds(30);
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }

            // Requeue this job in 10 seconds
            if ((_cameraDeadmanTimer != null) && _isEnabled)
            {
                _cameraDeadmanTimer.Change(10 * 1000, System.Threading.Timeout.Infinite);
            }
        }

        /// <summary>
        /// This is the callback when a new bitmap is available from the video camera
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mjpeg_FrameReady(object sender, FrameReadyEventArgs e)
        {
            this.videoImage.Source = e.BitmapImage;
            _lastUpdated = DateTime.Now;

            if (this.noVideoFeedAvailableGrid.Visibility == Visibility.Visible)
            {
                this.videoImage.Opacity = 1.0;
                this.noVideoFeedAvailableGrid.Visibility = Visibility.Collapsed;
            }
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.videoImage.Width  = e.NewSize.Width;
            this.videoImage.Height = e.NewSize.Height;
        }
    }
}
