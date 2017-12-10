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
                    "TimeUTC         DATETIME NOT NULL, \n " +
                    "Value           DOUBLE   NOT NULL, \n " +
                    "IsOnline        BIT      NOT NULL, \n " +
                    "Bucket          INTEGER  NOT NULL, \n " +
                    "FOREIGN KEY(SensorId) REFERENCES SensorTable(SensorId) \n" +
                 ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT "        +
                    "Id, "       +
                    "SensorId, " +
                    "TimeUTC, "  +
                    "Value, "    +
                    "IsOnline, " +
                    "Bucket "    +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@TimeUTC", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("TimeUTC")));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
            statement.Bind("@IsOnline", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsOnline")));
            statement.Bind("@Bucket", itemRow.Field<byte>("Bucket"));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@SensorId", itemRow.Field<Int64>("SensorId"));
            statement.Bind("@TimeUTC", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("TimeUTC")));
            statement.Bind("@Value", itemRow.Field<double>("Value"));
            statement.Bind("@IsOnline", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsOnline")));
            statement.Bind("@Bucket", itemRow.Field<byte>("Bucket"));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<Int64>("SensorId", (Int64)Convert.ToInt64(statement[01]));
            itemRow.SetField<DateTime>("TimeUTC", (DateTime)DateTime.Parse((string)statement[02]));
            itemRow.SetField<double>("Value", (double)Convert.ToDouble(statement[03]));
            itemRow.SetField<bool>("IsOnline", (bool)((Int64)statement[04] != 0));
            itemRow.SetField<byte>("Bucket", (byte)Convert.ToByte(statement[05]));

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("SensorId", typeof(Int64));
            itemTable.Columns.Add("TimeUTC", typeof(DateTime));
            itemTable.Columns.Add("Value", typeof(double));
            itemTable.Columns.Add("IsOnline", typeof(bool));
            itemTable.Columns.Add("Bucket", typeof(byte));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["SensorId"].DefaultValue = -1L;
            itemTable.Columns["TimeUTC"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["Value"].DefaultValue = 0D;
            itemTable.Columns["IsOnline"].DefaultValue = false;
            itemTable.Columns["Bucket"].DefaultValue = 0xFF;
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                          "( " +
                                "SensorId, " +
                                "TimeUTC, "  +
                                "Value, "    +
                                "IsOnline, " +
                                "Bucket "    +
                          ") " +
                          "VALUES " +
                          "( " +
                                "@SensorId, " +
                                "@TimeUTC, "  +
                                "@Value, "    +
                                "@IsOnline, " +
                                "@Bucket "    +
                          ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     SensorId = @SensorId,  \n" +
                   "     TimeUTC  = @TimeUTC,   \n" +
                   "     Value    = @Value,     \n" +
                   "     IsOnline = @Online,    \n" +
                   "     Bucket   = @Bucket     \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                    "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }

        async public Task BeginAdd(long mySensorID, DateTime myTimeUtc, double myValue, bool myIsOnline, byte myBucket)
        {
            ItemRow row = null;

            lock (Lock)
            {
                row = this.ItemTable.NewRow();
                row.SetField<DateTime>("TimeUTC", myTimeUtc);
                row.SetField<double>("Value", myValue);
                row.SetField<bool>("IsOnline", myIsOnline);
                row.SetField<byte>("Bucket", myBucket);

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

        async public Task BeginGetHistoryDataTableByDateRange(int sensorId, DateTime startTimeUtc, DateTime endTimeUtc, Action<ItemTable> callback)
        {
            string query =
                "SELECT * " +
                " FROM " + TableName +
                " WHERE " +
                    " SensorId = @SensorId AND " +
                    " TimeUTC >= @StartTimeUtc AND " +
                    " TimeUTC <= @EndTimeUtc" +
                " ORDER BY TIME ASC";

            ItemTable results = null;

            await Task.Run((Action)(() =>
            {
                results = new ItemTable();
                results.Columns.Add(PrimaryKeyName, typeof(Int64));
                results.Columns.Add("SensorId", typeof(Int64));
                results.Columns.Add("TimeUTC", typeof(DateTime));
                results.Columns.Add("Value", typeof(double));
                results.Columns.Add("IsOnline", typeof(bool));
                results.Columns.Add("Bucket", typeof(byte));

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
                        row.SetField<DateTime>("TimeUTC", (DateTime)DateTime.Parse((string)statement[02]));
                        row.SetField<double>("Value", (double)statement[03]);
                        row.SetField<bool>("IsOnline", (bool)statement[04]);
                        row.SetField<byte>("Bucket", (byte)statement[05]);

                        results.Rows.Add(row);
                        row.AcceptChanges();
                    }
                }
            }));

            callback(results);
        }

        /// <summary>
        /// Return the last observation written for the sensorId specified.
        /// </summary>
        /// <param name="sensorId"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        async public Task BeginGetLastDataPoint(long sensorId, Action<DateTime, double, bool, byte> callback)
        {
            string query =
                "SELECT TimeUTC, Value, IsOnline, Bucket " +
                " FROM " + TableName +
                " WHERE " +
                    " SensorId = @SensorId " +
                " ORDER BY TimeUTC DESC " +
                " LIMIT 1";

            // Set the default values.
            DateTime timeUtc = DateTime.Now.ToUniversalTime();
            double value = 0.0;
            bool isOnline = false;
            byte bucket = 0xFF;

            await Task.Run((Action)(() =>
            {
                using (var statement = sqlConnection.Prepare(query))
                {
                    statement.Bind("@SensorId", sensorId);

                    statement.Step();

                    // If data was returned copy it into the return parameters.
                    if (statement.DataCount >= 4)
                    {
                        timeUtc = DateTime.Parse((string)statement[0]);
                        value = (double)statement[1];
                        isOnline = (bool)statement[2];
                        bucket = (byte)statement[3];
                    }
                }
            }));

            callback(timeUtc, value, isOnline, bucket);
        }

        async public Task BeginTruncate(double maxSize)
        {
            if (IsReadOnly) return;

            string strDelete =
                "DELETE " +
                    " FROM " + TableName +
                    " WHERE Id In " +
                        " ( SELECT Id " +
                        "   FROM " + TableName +
                        "   ORDER BY [Id] ASC) " +
                        "   LIMIT 20000";

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
