//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Autofac;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfinityGroup.VesselMonitoring.Globals;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class BuildDBTables
    {
        async public Task DoIt()
        { 
            var builder = new ContainerBuilder();
            builder.RegisterType<SQLiteVesselDB>().As<IVesselDB>().SingleInstance();

            builder.RegisterType<SQLiteAISTable>().As<IAISTable>().SingleInstance();
            builder.RegisterType<SQLiteDeviceTable>().As<IDeviceTable>().SingleInstance();
            builder.RegisterType<SQLiteEventsTable>().As<IEventsTable>().SingleInstance();
            builder.RegisterType<SQLiteGaugePageTable>().As<IGaugePageTable>().SingleInstance();
            builder.RegisterType<SQLiteGaugeTable>().As<IGaugeTable>().SingleInstance();
            builder.RegisterType<SQLiteSensorDataTable>().As<ISensorDataTable>().SingleInstance();
            builder.RegisterType<SQLiteSensorTable>().As<ISensorTable>().SingleInstance();
            builder.RegisterType<SQLiteVesselSettingsTable>().As<IVesselSettingsTable>().SingleInstance();

            Container = builder.Build();

            VesselDB = Container.Resolve<IVesselDB>();
            VesselDB.DatabaseFileName = Path.Combine(Directory, DatabaseName + ".db");
            VesselDB.Create();
            builder.RegisterInstance<IVesselDB>(VesselDB).SingleInstance();

            AISTable = Container.Resolve<IAISTable>();
            await AISTable.BeginCreateTable(() => { }, (ex) => { });
            AISTable.Load();
            builder.RegisterInstance<IAISTable>(AISTable).SingleInstance();

            DeviceTable = Container.Resolve<IDeviceTable>();
            await DeviceTable.BeginCreateTable(() => { }, (ex) => { });
            DeviceTable.Load();
            builder.RegisterInstance<IDeviceTable>(DeviceTable).SingleInstance();

            EventsTable = Container.Resolve<IEventsTable>();
            await EventsTable.BeginCreateTable(()=> { },(ex)=> { });
            EventsTable.Load();
            builder.RegisterInstance<IEventsTable>(EventsTable).SingleInstance();

            GaugeTable = Container.Resolve<IGaugeTable>();
            await GaugeTable.BeginCreateTable(() => { }, (ex) => { });
            GaugeTable.Load();
            builder.RegisterInstance<IGaugeTable>(GaugeTable).SingleInstance();

            GaugePageTable = Container.Resolve<IGaugePageTable>();
            await GaugePageTable.BeginCreateTable(() => { }, (ex) => { });
            GaugePageTable.Load();
            builder.RegisterInstance<IGaugePageTable>(GaugePageTable).SingleInstance();

            SensorTable = Container.Resolve<ISensorTable>();
            await SensorTable.BeginCreateTable(() => { }, (ex) => { });
            SensorTable.Load();
            builder.RegisterInstance<ISensorTable>(SensorTable).SingleInstance();

            SensorDataTable = Container.Resolve<ISensorDataTable>();
            await SensorDataTable.BeginCreateTable(() => { }, (ex) => { });
            SensorDataTable.Load();
            builder.RegisterInstance<ISensorDataTable>(SensorDataTable).SingleInstance();

            VesselSettingsTable = Container.Resolve<IVesselSettingsTable>();
            await VesselSettingsTable.BeginCreateTable(() => { }, (ex) => { });
            VesselSettingsTable.Load();
            builder.RegisterInstance<IVesselSettingsTable>(VesselSettingsTable).SingleInstance();
        }

        public static string Directory { get; set; }
        public static string DatabaseName { get; set; }
        public static IAISTable AISTable { get; private set; }
        public static IDeviceTable DeviceTable { get; private set; }
        public static IEventsTable EventsTable { get; private set; }
        public static IGaugePageTable GaugePageTable { get; private set; }
        public static IGaugeTable GaugeTable { get; private set; }
        public static ISensorTable SensorTable { get; private set; }
        public static ISensorDataTable SensorDataTable { get; private set; }
        public static IVesselDB VesselDB { get; private set; }
        public static IVesselSettingsTable VesselSettingsTable { get; private set; }

        public static IContainer Container { get; set; }
    }
}
