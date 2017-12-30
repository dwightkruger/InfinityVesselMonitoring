//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Ioc;
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

            this.VesselDB = SimpleIoc.Default.GetInstance<IVesselDB>();
            this.VesselDB.DatabaseFileName = Path.Combine(Directory, DatabaseName + ".db");
            this.VesselDB.Create();

            this.VesselSettingsTable = SimpleIoc.Default.GetInstance<IVesselSettingsTable>();
            await VesselSettingsTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.VesselSettingsTable.Load();
        }

        /// <summary>
        /// Create and load all of the remaining db tables.
        /// </summary>
        /// <returns></returns>
        async public Task Build()
        {
            this.AISTable = SimpleIoc.Default.GetInstance<IAISTable>();
            await this.AISTable.BeginCreateTable(() => { }, (ex) => { });
            this.AISTable.Load();

            this.DeviceTable = SimpleIoc.Default.GetInstance<IDeviceTable>();
            await this.DeviceTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.DeviceTable.Load();

            this.EventsTable = SimpleIoc.Default.GetInstance<IEventsTable>();
            await this.EventsTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.EventsTable.Load();

            this.GaugeTable = SimpleIoc.Default.GetInstance<IGaugeTable>();
            await this.GaugeTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.GaugeTable.Load();

            this.GaugePageTable = SimpleIoc.Default.GetInstance<IGaugePageTable>();
            await this.GaugePageTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.GaugePageTable.Load();

            this.SensorTable = SimpleIoc.Default.GetInstance<ISensorTable>();
            await this.SensorTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.SensorTable.Load();

            this.SensorDataTable = SimpleIoc.Default.GetInstance<ISensorDataTable>();
            await this.SensorDataTable.BeginCreateTable(() => { }, (ex) => { Telemetry.TrackException(ex); });
            this.SensorDataTable.Load();
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

        /// <summary>
        /// Register all of the DB components with the IOC container
        /// </summary>
        /// <returns></returns>        
        private void RegisterComponents()
        {
            SimpleIoc.Default.Register<IVesselDB, SQLiteVesselDB>();
            SimpleIoc.Default.Register<IAISTable, SQLiteAISTable>();
            SimpleIoc.Default.Register<IDeviceTable, SQLiteDeviceTable>();
            SimpleIoc.Default.Register<IEventsTable, SQLiteEventsTable>();
            SimpleIoc.Default.Register<IGaugePageTable, SQLiteGaugePageTable>();
            SimpleIoc.Default.Register<IGaugeTable, SQLiteGaugeTable>();
            SimpleIoc.Default.Register<ISensorDataTable, SQLiteSensorDataTable>();
            SimpleIoc.Default.Register<ISensorTable, SQLiteSensorTable>();
            SimpleIoc.Default.Register<IVesselSettingsTable, SQLiteVesselSettingsTable>();
        }
    }
}
