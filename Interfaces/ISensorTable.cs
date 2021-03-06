﻿//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2015 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    


using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface ISensorTable : IVesselTable
    {
        Task<int> TotalSensors();
    }
}
