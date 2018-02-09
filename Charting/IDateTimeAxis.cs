//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

using System;

namespace InfinityGroup.VesselMonitoring.Charting
{
    public interface IDateTimeAxis
    {
        DateTime Maximum { get; set; }
        DateTime Minimum { get; set; }
        TimeSpan MajorStep { get; set; }
    }
}
