//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public enum DeviceType : int
    {
        Unknown = 0,
        NMEA2000 = 1,
        OneNet = 2,
        IPCamera = 3,
        Virtual = 4
    }

    public interface IDeviceItem
    {
        Task BeginCommit();
        DateTime ChangeDate { get; }
        string Description { get; set; }
        byte DeviceAddress { get; set; }
        long DeviceId { get; set; }
        DeviceType DeviceType { get; set; }
        string FirmwareVersion { get; set; }
        string HardwareVersion { get; set; }
        bool IsDirty { get; }
        bool IsOnline { get; set; }
        bool IsSwitchDevice { get; }
        bool IsVirtual { get; set; }
        string IPAddress { get; set; }
        DateTime LastUpdate { get; set; }
        string Location { get; set; }
        string Model { get; set; }
        string Name { get; set; }
        string Manufacturer { get; set; }
        List<UInt32> ReceivePGNList { get; set; }
        void Rollback();
        List<ISensorItem> Sensors { get; }
        string SerialNumber { get; set; }
        string SoftwareVersion { get; set; }
        List<UInt32> TransmitPGNList { get; set; }
    }
}
