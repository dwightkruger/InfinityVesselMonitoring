//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

namespace InfinityGroup.VesselMonitoring.Charting
{
    public enum AxisHorizontalLocation
    {
        Left = 0,
        RLight = 1
    }

    public enum AxisVerticalLocation
    {
        Top = 0,
        Bottom = 1,
    }

    public interface ICartesianAxis
    {
        AxisHorizontalLocation HorizontalLocation { get; set; }
        AxisVerticalLocation VerticalLocation { get; set; }
    }
}
