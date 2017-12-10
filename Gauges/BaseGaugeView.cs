//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Windows.UI.Xaml.Controls;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    public class BaseGaugeView : UserControl
    {
        public virtual BaseGaugeViewModel ViewModel { get; }
    }
}
