//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Net;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IVideoCamera
    {
        void Initialize(IPAddress myCameraIPAddress);
        EventHandler<FrameReadyEventArgs> FrameReady { get; set; }
        int FPS { get; set; }
        IPAddress IPAddress { get; }
        void PanLeft();
        void PanRight();
        void Start();
        void Stop();
        void TiltUp();
        void TiltDown();
        VideoCameraResolution VideoCameraResolution { get; set; }
        void ZoomIn();
        void ZoomOut();
    }

    public enum VideoCameraResolution : int
    {
        e1280x720 = 0,
        e800x600,
        e800x450,
        e640x480,
        e640x360,
        e480x360,
        e480x270,
        e320x240,
        e320x180
    }

    public class FrameReadyEventArgs : EventArgs
    {
        public byte[] FrameBuffer;
        public BitmapImage BitmapImage;
    }
}
