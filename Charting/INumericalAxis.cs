//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

namespace InfinityGroup.VesselMonitoring.Charting
{
    public interface INumericalAxis
    {
        double LowAlarmValue { get; set; }
        double LowWarningValue { get; set; }
        double HighAlarmValue { get; set; }
        double HighWarningValue { get; set; }

        double Maximum { get; set; }
        double Minimum { get; set; }
        double MajorStep { get; set; }
    }
}
