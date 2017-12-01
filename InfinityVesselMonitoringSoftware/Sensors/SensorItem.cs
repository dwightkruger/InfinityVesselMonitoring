//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;

namespace VesselMonitoringSuite.Sensors
{
    public class SensorItem : ObservableObject, ISensorItem
    {
        public DateTime ChangeDate => throw new NotImplementedException();

        public string Description { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int DeviceId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string FriendlyName => throw new NotImplementedException();

        public double HighAlarmValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double HighWarningValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsCalibrated { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsHighAlarmEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsHighWarningEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsLowAlarmEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool IsLowWarningEnabled { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsOnline => throw new NotImplementedException();

        public bool IsPersisted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsVirtual => throw new NotImplementedException();

        public string Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double LowAlarmValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double LowWarningValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double MaxValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double MinValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Name { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public double NominalValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool PersistDataPoints { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public uint PGN { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int PortNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Priority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int Resolution { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ItemRow Row => throw new NotImplementedException();

        public int SensorId => throw new NotImplementedException();

        public SensorType SensorType { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public UnitType SensorUnits { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public SensorUsage SensorUsage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public double SensorValue => throw new NotImplementedException();

        public string SerialNumber { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool ShowNominalValue { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TimeSpan Throttle { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public DateTime Time => throw new NotImplementedException();

        public void DisableSensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void EmptySensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void EnableSensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
