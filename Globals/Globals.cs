//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight.Messaging;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;

namespace InfinityGroup.VesselMonitoring.Globals
{
    public class Globals
    {
        public static ResourceLoader ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
        public static Size ScreenSize;
        public static Messenger SensorValueMessenger = new Messenger();
    }
}
