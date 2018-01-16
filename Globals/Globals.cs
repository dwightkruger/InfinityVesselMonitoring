//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Autofac;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;

namespace InfinityGroup.VesselMonitoring.Globals
{
    public class Globals
    {
        public static ResourceLoader ResourceLoader = Windows.ApplicationModel.Resources.ResourceLoader.GetForCurrentView();
        public static Size ScreenSize;
    }
}
