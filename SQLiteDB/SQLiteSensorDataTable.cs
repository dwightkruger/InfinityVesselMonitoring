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
    public class SQLiteSensorDataTable : SQLiteVesselTableBase<ItemRow, Int64>, ISensorDataTable
    {
        public SQLiteSensorDataTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "Id";
            TableName = "SensorDataTable";
        }

        async override public Task BeginCreateTable(Action successCallback, Action<Exception> failureCallback)
        {
            if (IsReadOnly) successCallback();

            await base.BeginCreateTable(() =>
            {
                // Create indexes
                try
                {
                    string createIndex = "CREATE INDEX IF NOT EXISTS SensorDataTableIndex_SensorIdTimeIndex ON \n " +
                                TableName + "\n" +
                                " ( \n" +
                                    " SensorId ASC, \n" +
                                    " Time ASC      \n" +
                                " )";
                    using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(createIndex))
                    {
                        statement.Step();
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }
            },
            (ex) =>
            {
                Telemetry.TrackException(ex);
                failureCallback(ex);
            });

            successCallback();
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "Id              INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "SensorId        INTEGER  NOT NULL, \n " +
                    "Time            DATETIME NOT NULL, \n " +
                    "Value           DOUBLE   NOT NULL, \n " +
                    "Online          BIT      NOT NULL, \n " +
                    "FOREIGN KEY(SensorId) REFERENCES SensorTable(SensorId) \n" +
                 ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "Id, " +
                    "SensorId, " +
                    "Time, " +
                    "Value, " +
                    "Online  " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@Time", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("Time")));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
            statement.Bind("@Online", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("Online")));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@Time", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("Time")));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
            statement.Bind("@Online", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("Online")));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<Int64>("SensorId", (Int64)Convert.ToInt64(statement[01]));
            itemRow.SetField<DateTime>("Time", (DateTime)DateTime.Parse((string)statement[02]));
            itemRow.SetField<double>("Value", (double)Convert.ToDouble(statement[03]));
            itemRow.SetField<bool>("Online", (bool)((Int64)statement[04] != 0));

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("SensorId", typeof(Int64));
            itemTable.Columns.Add("Time", typeof(DateTime));
            itemTable.Columns.Add("Value", typeof(double));
            itemTable.Columns.Add("Online", typeof(bool));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["SensorId"].DefaultValue = -1L;
            itemTable.Columns["Time"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["Value"].DefaultValue = 0D;
            itemTable.Columns["Online"].DefaultValue = false;
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                          "( " +
                                "SensorId, " +
                                "Time, " +
                                "Value, " +
                                "Online " +
                          ") " +
                          "VALUES " +
                          "( " +
                                "@SensorId, " +
                                "@Time, " +
                                "@Value, " +
                                "@Online " +
                          ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     SensorId = @SensorId,  \n" +
                   "     Time     = @Time,      \n" +
                   "     Value    = @Value,     \n" +
                   "     Online   = @Online     \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                    "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }

        async public void Add(long mySensorID, DateTime myTimeUtc, double myValue, bool myIsOnline)
        {
            ItemRow row = null;

            lock (Lock)
            {
                row = this.ItemTable.NewRow();
                row.SetField<DateTime>("Time", myTimeUtc);
                row.SetField<double>("Value", myValue);
                row.SetField<bool>("IsOnline", myIsOnline);
                this.ItemTable.Rows.Add(row);
            }

            await this.BeginCommitRow(row,
                () =>
                {
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                });
        }

        async public void GetHistoryDataTableByDateRange(int sensorId, DateTime startTimeUtc, DateTime endTimeUtc, Action<ItemTable> callback)
        {
            string query =
                "SELECT * " +
                " FROM " + TableName +
                " WHERE " +
                    " SensorId = @SensorId AND " +
                    " Time >= @StartTimeUtc AND " +
                    " Time <= @EndTimeUtc" +
                " ORDER BY TIME ASC";

            ItemTable results = null;

            await Task.Run((Action)(() =>
            {
                results = new ItemTable();
                results.Columns.Add(PrimaryKeyName, typeof(Int64));
                results.Columns.Add("SensorId", typeof(Int64));
                results.Columns.Add("Time", typeof(DateTime));
                results.Columns.Add("Value", typeof(double));
                results.Columns.Add("Online", typeof(bool));

                using (var statement = sqlConnection.Prepare(query))
                {
                    statement.Bind("@SensorId", sensorId);
                    statement.Bind("@StartTimeUtc", SQLiteDB.Utilities.DateTimeSQLite(startTimeUtc));
                    statement.Bind("@EndTimeUtc", SQLiteDB.Utilities.DateTimeSQLite(endTimeUtc));

                    while (statement.Step() == SQLiteResult.ROW)
                    {
                        ItemRow row = results.NewRow();

                        row.SetField<Int64>(PrimaryKeyName, (Int64)statement[00]);
                        row.SetField<Int64>("SensorId", (Int64)statement[01]);
                        row.SetField<DateTime>("Time", (DateTime)DateTime.Parse((string)statement[02]));
                        row.SetField<double>("Value", (double)statement[03]);
                        row.SetField<bool>("Online", (bool)statement[04]);

                        results.Rows.Add(row);
                        row.AcceptChanges();
                    }
                }
            }));

            callback(results);
        }

        async public void GetLastDataPoint(long sensorId, Action<DateTime, double, bool> callback)
        {
            string query =
                "SELECT TOP 1 Time, Value, IsOnline " +
                " FROM " + TableName +
                " WHERE " +
                    " SensorId = @SensorId " +
                " ORDER BY Time DESC";

            DateTime timeUtc = DateTime.UtcNow;
            double value = 0;
            bool isOnline = false;

            await Task.Run((Action)(() =>
            {
                using (var statement = sqlConnection.Prepare(query))
                {
                    statement.Bind("@SensorId", sensorId);

                    statement.Step();

                    timeUtc = DateTime.Parse((string)statement[00]);
                    value = (double)statement[01];
                    isOnline = (bool)statement[02];
                }
            }));

            callback(timeUtc, value, isOnline);
        }

        async public void Truncate(double maxSize)
        {
            if (IsReadOnly) return;

            string strDelete =
                "DELETE " +
                    " FROM " + TableName +
                    " WHERE Id In " +
                        " ( SELECT TOP 20000 Id " +
                        "   FROM " + TableName +
                        "   ORDER BY [Id] ASC) ";

            await Task.Run((Action)(() =>
            {
                // Run the deletion in a loop until either we have reduced the size of the DB to
                // the desired size, or until we can no longer delete records.

                // Delete some rows
                while (Utilities.GetSize(sqlConnection) > maxSize)
                {
                    using (var statement = sqlConnection.Prepare(strDelete))
                    {
                        SQLiteResult result = statement.Step();
                    }
                }

                // Vacuum the database
            }));
        }

    }
}
