//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Autofac;
using Windows.UI.Xaml.Controls;

namespace InfinityGroup.VesselMonitoring.Globals
{
    public class Globals
    {
        public static IContainer Container { get; set; }
        public static MediaPlayerElement MediaPlayer { get; set; }
    }
}
