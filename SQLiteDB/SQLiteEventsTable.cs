//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using SQLitePCL;
using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteEventsTable : SQLiteVesselTableBase<ItemRow, Int64>, IEventsTable
    {
        public SQLiteEventsTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "EventId";
            TableName = "EventsTable";
        }

        override public void CreateTable()
        {
            if (IsReadOnly) return;

            try
            {
                base.CreateTable();
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                throw;
            }

            // Create indexes
            try
            { 
                string createDateTimeIndex = "CREATE INDEX IF NOT EXISTS EventsTableIndex_DateTime ON \n " +
                            TableName + "\n" +
                            " ( \n" +
                                " EventDate ASC \n" +
                            " )";
                using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(createDateTimeIndex))
                {
                    statement.Step();
                }

                string createSensorIdIndex = "CREATE INDEX IF NOT EXISTS EventsTableIndex_SensorIdDateTime ON \n " +
                        TableName + "\n" +
                        " ( \n" +
                            " SensorId ASC, \n" +
                            " EventDate ASC \n" +
                        " )";
                using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(createSensorIdIndex))
                {
                    statement.Step();
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                throw;
            }
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "EventId         INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "EventDate       DATETIME NOT NULL, \n " +
                    "EventCode       INTEGER  NOT NULL, \n " +
                    "EventPriority   INTEGER  NOT NULL, \n " +
                    "Latitude        DOUBLE   NOT NULL, \n " +
                    "Longitude       DOUBLE   NOT NULL, \n " +
                    "PropertyBag     TEXT     NOT NULL, \n " +
                    "SensorId        INTEGER  NOT NULL, \n " +
                    "Value           DOUBLE   NOT NULL, \n " +
                    "FOREIGN KEY(SensorId) REFERENCES SensorTable(SensorId) \n" +
                 ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "EventId, " +
                    "EventDate, " +
                    "EventCode, " +
                    "EventPriority, " +
                    "Latitude, " +
                    "Longitude, " +
                    "PropertyBag, " +
                    "SensorId, " +
                    "Value " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@EventDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("EventDate")));
            statement.Bind("@EventCode", itemRow.Field<Int32>("EventCode"));
            statement.Bind("@EventPriority", itemRow.Field<Int32>("EventPriority"));
            statement.Bind("@Latitude", itemRow.Field<double>("Latitude"));
            statement.Bind("@Longitude", itemRow.Field<double>("Longitude"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@EventDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("EventDate")));
            statement.Bind("@EventCode", itemRow.Field<Int32>("EventCode"));
            statement.Bind("@EventPriority", itemRow.Field<Int32>("EventPriority"));
            statement.Bind("@Latitude", itemRow.Field<double>("Latitude"));
            statement.Bind("@Longitude", itemRow.Field<double>("Longitude"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("EventDate", (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<Int32>("EventCode", (Int32)Convert.ToInt32(statement[02]));
            itemRow.SetField<Int32>("EventPriority", (Int32)Convert.ToInt32(statement[03]));
            itemRow.SetField<double>("Latitude", (double)Convert.ToDouble(statement[04]));
            itemRow.SetField<double>("Longitude", (double)Convert.ToDouble(statement[05]));
            itemRow.SetField<string>("PropertyBag", (string)statement[06]);
            itemRow.SetField<Int64>("SensorId", (Int64)Convert.ToInt64(statement[07]));
            itemRow.SetField<double>("Value", (double)Convert.ToDouble(statement[08]));

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("EventDate", typeof(DateTime));
            itemTable.Columns.Add("EventCode", typeof(Int32));
            itemTable.Columns.Add("EventPriority", typeof(Int32));
            itemTable.Columns.Add("Latitude", typeof(double));
            itemTable.Columns.Add("Longitude", typeof(double));
            itemTable.Columns.Add("PropertyBag", typeof(string));
            itemTable.Columns.Add("SensorId", typeof(Int64));
            itemTable.Columns.Add("Value", typeof(float));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["EventDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["EventCode"].DefaultValue = 0;
            itemTable.Columns["EventPriority"].DefaultValue = 0;
            itemTable.Columns["Latitude"].DefaultValue = 0D;
            itemTable.Columns["Longitude"].DefaultValue = 0D;
            itemTable.Columns["PropertyBag"].DefaultValue = new PropertyBag().JsonSerialize();
            itemTable.Columns["SensorId"].DefaultValue = -1L;
            itemTable.Columns["Value"].DefaultValue = 0;
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                          "( " +
                                "EventDate, " +
                                "EventCode, " +
                                "EventPriority, " +
                                "Latitude, " +
                                "Longitude, " +
                                "PropertyBag, " +
                                "SensorId, " +
                                "Value " +
                          ") " +
                          "VALUES " +
                          "( " +
                                "@EventDate, " +
                                "@EventCode, " +
                                "@EventPriority, " +
                                "@Latitude, " +
                                "@Longitude, " +
                                "@PropertyBag, " +
                                "@SensorId, " +
                                "@Value " +
                          ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     EventDate     = @EventDate,      \n" +
                   "     EventCode     = @EventCode,      \n" +
                   "     EventPriority = @EventPriority,  \n" +
                   "     Latitude      = @Latitude,       \n" +
                   "     Longitude     = @Longitude,      \n" +
                   "     PropertyBag   = @PropertyBag,    \n" +
                   "     SensorId      = @SensorId,       \n" +
                   "     Value         = @Value           \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                    "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }

}
