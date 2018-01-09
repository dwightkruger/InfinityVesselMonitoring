//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Net;
using System.Net.Http;

namespace InfinityVesselMonitoringSoftware.VideoCamera
{
    public class AxisVAPIXVersion3 : ObservableObject, IVideoCamera
    {
        static private string _panLeft  = "/axis-cgi/com/ptz.cgi?rpan=1";
        static private string _panRight = "/axis-cgi/com/ptz.cgi?rpan=-1";
        static private string _tiltUp   = "/axis-cgi/com/ptz.cgi?rtilt=-1";
        static private string _tiltDown = "/axis-cgi/com/ptz.cgi?rtilt=1";
        static private string _zoomIn   = "/axis-cgi/com/ptz.cgi?rzoom=-400";
        static private string _zoomOut  = "/axis-cgi/com/ptz.cgi?rzoom=400";

        private MjpegDecoder _mjpegDecoder;
        private IPAddress _ipAddress;
        private int _FPS;
        private VideoCameraResolution _videoCameraResolution;

        public void Initialize(IPAddress myCameraIPAddress)
        {
            this.IPAddress = myCameraIPAddress;
            this.FPS = 8;
            this.VideoCameraResolution = VideoCameraResolution.e1280x720;

            _mjpegDecoder = new MjpegDecoder();

            this.IsProcessingCommand = false;
        }

        public void Start()
        {
            if (_mjpegDecoder != null)
            {
                // Get the camera resolution as a string
                string resolution = Enum.GetName(typeof(VideoCameraResolution), _videoCameraResolution);
                resolution = resolution.TrimStart('e');

                _mjpegDecoder.ParseStream(new Uri("http://" + this.IPAddress + "/axis-cgi/mjpg/video.cgi?fps=" + FPS.ToString() + "&resolution=" + resolution));
            }

            this.IsProcessingCommand = false;
        }

        public EventHandler<FrameReadyEventArgs> FrameReady
        {
            get
            {
                return _mjpegDecoder.FrameReady;
            }

            set
            {
                _mjpegDecoder.FrameReady = value;
            }
        }

        public int FPS
        {
            get { return _FPS; }
            set { Set<int>(() => FPS, ref _FPS, value); }
        }

        public IPAddress IPAddress
        {
            get { return _ipAddress; }
            set { Set<IPAddress>(() => IPAddress, ref _ipAddress, value); }
        }

        public VideoCameraResolution VideoCameraResolution
        {
            get { return _videoCameraResolution; }
            set { Set<VideoCameraResolution>(() => VideoCameraResolution, ref _videoCameraResolution, value); }
        }

        public void Stop()
        {
            if (_mjpegDecoder != null)
            {
                _mjpegDecoder.StopStream();
            }

            this.IsProcessingCommand = false;
        }

        public void PanLeft()
        {
            this.SendCommand(_panLeft);
        }

        public void PanRight()
        {
            this.SendCommand(_panRight);
        }

        public void TiltUp()
        {
            this.SendCommand(_tiltUp);
        }

        public void TiltDown()
        {
            this.SendCommand(_tiltDown);
        }

        public void ZoomIn()
        {
            this.SendCommand(_zoomIn);
        }

        public void ZoomOut()
        {
            this.SendCommand(_zoomOut);
        }

        #region private
        private bool IsProcessingCommand { get; set; }

        async private void SendCommand(string message)
        {
            if (!this.IsProcessingCommand)
            {
                this.IsProcessingCommand = true;

                try
                {
                    HttpClient httpClient = new HttpClient();
                    HttpResponseMessage response = await httpClient.PostAsync(this.IPAddress.ToString(), new StringContent(message));
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }

                this.IsProcessingCommand = false;
            }
        }
        #endregion
    }
}
