//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Autofac;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteBuildDBTables : IBuildDBTables
    {
        public SQLiteBuildDBTables()
        {
        }

        /// <summary>
        /// Create/load the database and enough tables to load the application startup parametes
        /// </summary>
        /// <returns></returns>
        async public Task BuildVesselSettings()
        {
            InfinityGroup.VesselMonitoring.SQLiteDB.Utilities.CreateDirectory(this.Directory);

            this.RegisterComponents();

            this.VesselDB = this.Container.Resolve<IVesselDB>();
            this.VesselDB.DatabaseFileName = Path.Combine(Directory, DatabaseName + ".db");
            this.VesselDB.Create();
            this.Builder.RegisterInstance<IVesselDB>(VesselDB).SingleInstance();

            this.VesselSettingsTable = this.Container.Resolve<IVesselSettingsTable>();
            await VesselSettingsTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.VesselSettingsTable.Load();
            this.Builder.RegisterInstance<IVesselSettingsTable>(VesselSettingsTable).SingleInstance();
        }

        /// <summary>
        /// Create and load all of the remaining db tables.
        /// </summary>
        /// <returns></returns>
        async public Task Build()
        {
            this.AISTable = this.Container.Resolve<IAISTable>();
            await this.AISTable.BeginCreateTable(() => { }, (ex) => { });
            this.AISTable.Load();
            this.Builder.RegisterInstance<IAISTable>(AISTable).SingleInstance();

            this.DeviceTable = this.Container.Resolve<IDeviceTable>();
            await this.DeviceTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.DeviceTable.Load();
            this.Builder.RegisterInstance<IDeviceTable>(DeviceTable).SingleInstance();

            this.EventsTable = this.Container.Resolve<IEventsTable>();
            await this.EventsTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.EventsTable.Load();
            this.Builder.RegisterInstance<IEventsTable>(EventsTable).SingleInstance();

            this.GaugeTable = this.Container.Resolve<IGaugeTable>();
            await this.GaugeTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.GaugeTable.Load();
            this.Builder.RegisterInstance<IGaugeTable>(GaugeTable).SingleInstance();

            this.GaugePageTable = this.Container.Resolve<IGaugePageTable>();
            await this.GaugePageTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.GaugePageTable.Load();
            this.Builder.RegisterInstance<IGaugePageTable>(GaugePageTable).SingleInstance();

            this.SensorTable = this.Container.Resolve<ISensorTable>();
            await this.SensorTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.SensorTable.Load();
            this.Builder.RegisterInstance<ISensorTable>(SensorTable).SingleInstance();

            this.SensorDataTable = this.Container.Resolve<ISensorDataTable>();
            await this.SensorDataTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.SensorDataTable.Load();
            this.Builder.RegisterInstance<ISensorDataTable>(SensorDataTable).SingleInstance();
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
        public Autofac.IContainer Container { get; set; }
        private ContainerBuilder Builder { get; set; }

        /// <summary>
        /// Register all of the DB components with the IOC container
        /// </summary>
        /// <returns></returns>        
        private void RegisterComponents()
        {
            this.Builder = new ContainerBuilder();
            this.Builder.RegisterType<SQLiteVesselDB>().As<IVesselDB>().SingleInstance();

            this.Builder.RegisterType<SQLiteAISTable>().As<IAISTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteDeviceTable>().As<IDeviceTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteEventsTable>().As<IEventsTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteGaugePageTable>().As<IGaugePageTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteGaugeTable>().As<IGaugeTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteSensorDataTable>().As<ISensorDataTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteSensorTable>().As<ISensorTable>().SingleInstance();
            this.Builder.RegisterType<SQLiteVesselSettingsTable>().As<IVesselSettingsTable>().SingleInstance();

            this.Container = this.Builder.Build();
        }
    }
}
