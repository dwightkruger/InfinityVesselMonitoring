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
    public interface ISensorDataTable : IVesselTable
    {
        Task BeginAdd(Int64 mySensorID, DateTime myTimeUtc, double myValue, bool myIsOnline, byte myBucket);

        Task BeginGetHistoryDataTableByDateRange(int sensorID, DateTime startTimeUtc, DateTime endTimeUtc, Action<ItemTable> callback);

        Task BeginGetLastDataPoint(Int64 sensorID, Action<DateTime, double, bool, byte> callback);

        Task BeginTruncate(double maxSize);

        Task BeginDeleteSensorObservations(Int64 mySensorID);
    }
}
