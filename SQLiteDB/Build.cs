//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteBuildDBTables : IBuildDBTables
    {
        async public Task BuildVesselSettings()
        {
            this.VesselDB = new SQLiteVesselDB();
            this.VesselDB.DatabaseFileName = Path.Combine(Directory, DatabaseName + ".db");
            this.VesselDB.Create();

            this.VesselSettingsTable = new SQLiteVesselSettingsTable(this.VesselDB);
            await VesselSettingsTable.BeginCreateTable(() => { }, (ex) => { });
            this.VesselSettingsTable.Load();
        }

        async public Task Build()
        {
            this.AISTable = new SQLiteAISTable(this.VesselDB);
            await AISTable.BeginCreateTable(() => { }, (ex) => { });
            this.AISTable.Load();

            this.DeviceTable = new SQLiteDeviceTable(this.VesselDB);
            await DeviceTable.BeginCreateTable(() => { }, (ex) => { });
            this.DeviceTable.Load();

            this.EventsTable = new SQLiteEventsTable(this.VesselDB);
            await EventsTable.BeginCreateTable(()=> { },(ex)=> { });
            this.EventsTable.Load();

            this.GaugePageTable = new SQLiteGaugePageTable(this.VesselDB);
            await GaugePageTable.BeginCreateTable(() => { }, (ex) => { });
            this.GaugePageTable.Load();

            this.GaugeTable = new SQLiteGaugeTable(this.VesselDB);
            await GaugeTable.BeginCreateTable(() => { }, (ex) => { });
            this.GaugeTable.Load();

            this.SensorTable = new SQLiteSensorTable(this.VesselDB);
            await SensorTable.BeginCreateTable(() => { }, (ex) => { });
            SensorTable.Load();

            this.SensorDataTable = new SQLiteSensorDataTable(this.VesselDB);
            await SensorDataTable.BeginCreateTable(() => { }, (ex) => { });
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
    }
}
