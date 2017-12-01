//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IGaugeTable : IVesselTable
    {
        Task BeginFindByGaugePageId(Int64 gaugePageId, Action<ItemTable> sucessCallback, Action<Exception> failureCallback);
    }
}
