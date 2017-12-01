//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Types;
using System;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface ISensorDataTable : IVesselTable
    {
        void Add(Int64 mySensorID, DateTime myTimeUtc, double myValue, bool myIsOnline);

        void GetHistoryDataTableByDateRange(int sensorID, DateTime startTimeUtc, DateTime endTimeUtc, Action<ItemTable> callback);

        void GetLastDataPoint(Int64 sensorID, Action<DateTime, double, bool> callback);

        void Truncate(double maxSize);

    }
}
