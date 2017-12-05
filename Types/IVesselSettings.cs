//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

namespace InfinityGroup.VesselMonitoring.Types
{
    public interface IVesselSettings
    {
        string VesselName { get; set; }
        string FromEmailAddress { get; set; }
        string ToEmailAddress { get; set; }
    }
}
