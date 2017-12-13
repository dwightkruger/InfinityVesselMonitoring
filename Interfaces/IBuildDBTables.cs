//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Autofac;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IBuildDBTables
    {
        Task Build();
        string Directory { get; set; }
        string DatabaseName { get; set; }
        IAISTable AISTable { get; }
        IDeviceTable DeviceTable { get; }
        IEventsTable EventsTable { get; }
        IGaugePageTable GaugePageTable { get; }
        IGaugeTable GaugeTable { get; }
        ISensorTable SensorTable { get; }
        ISensorDataTable SensorDataTable { get; }
        IVesselDB VesselDB { get; }
        IVesselSettingsTable VesselSettingsTable { get; }

        IContainer Container { get; set; }

    }
}
