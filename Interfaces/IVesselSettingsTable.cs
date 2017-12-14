//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IVesselSettingsTable : IVesselTable
    {
        Task BeginRemove(string propertyName);
    }
}
