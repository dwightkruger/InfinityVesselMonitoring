//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Autofac;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteBuildDBTables : IBuildDBTables
    {
        async public Task Build()
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

        public string Directory { get; set; }
        public string DatabaseName { get; set; }
        public IAISTable AISTable { get; private set; }
        public IDeviceTable DeviceTable { get; private set; }
        public IEventsTable EventsTable { get; private set; }
        public IGaugePageTable GaugePageTable { get; private set; }
        public IGaugeTable GaugeTable { get; private set; }
        public ISensorTable SensorTable { get; private set; }
        public ISensorDataTable SensorDataTable { get; private set; }
        public IVesselDB VesselDB { get; private set; }
        public IVesselSettingsTable VesselSettingsTable { get; private set; }

        public IContainer Container { get; set; }
    }
}
