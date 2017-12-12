//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IEventItem
    {
        bool AlarmAcknowledged { get; set; }
        Task BeginCommit();
        EventCode EventCode { get; set; }
        DateTime EventDateTimeUTC { get; set; }
        Int64 EventId { get; }
        int EventPriority { get; set; }
        bool IsAlarmOn { get; }
        bool IsWarningOn { get; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        void Rollback();
        Int64 SensorId { get; set; }
        double Value { get; set; }
    }

    public enum EventCode : int
    {
        None = 0,
        Other,
        LowAlarmOn,
        LowAlarmOff,
        LowWarningOn,
        LowWarningOff,
        HighWarningOn,
        HighWarningOff,
        HighAlarmOn,
        HighAlarmOff,
        SwitchOnTooLongOn,
        SwitchOnTooLongOff,
        SwitchOffTooLongOn,
        SwitchOffTooLongOff,
        AlarmAcknowledged,
        ProgramStarted,
        ProgramStopped,
        AlarmEmailWarning,
        AlarmEmailSucceeded,
    }

}
