//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Utilities;
using SQLitePCL;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteSensorTable : SQLiteVesselTableBase<ItemRow, Int64>, ISensorTable
    {
        public SQLiteSensorTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "SensorId";
            TableName = "SensorTable";
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + " \n" +
                "(\n" +
                    "SensorId             INTEGER       NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "ChangeDate           DATETIME      NOT NULL, \n " +
                    "Description          TEXT          NOT NULL, \n " +
                    "DeviceId             INTEGER       NOT NULL, \n " +
                    "GaugeUnits           INTEGER       NOT NULL, \n " +
                    "HighAlarmValue       FLOAT         NOT NULL, \n " +
                    "HighWarningValue     FLOAT         NOT NULL, \n " +
                    "IsCalibrated         BOOLEAN       NOT NULL, \n " +
                    "IsEnabled            BOOLEAN       NOT NULL, \n " +
                    "IsHighAlarmEnabled   BOOLEAN       NOT NULL, \n " +
                    "IsHighWarningEnabled BOOLEAN       NOT NULL, \n " +
                    "IsLowAlarmEnabled    BOOLEAN       NOT NULL, \n " +
                    "IsLowWarningEnabled  BOOLEAN       NOT NULL, \n " +
                    "IsOnline             BOOLEAN       NOT NULL, \n " +
                    "IsPersisted          BOOLEAN       NOT NULL, \n " +
                    "IsVirtual            BOOLEAN       NOT NULL, \n " +
                    "Location             TEXT          NOT NULL, \n " +
                    "LowAlarmValue        FLOAT         NOT NULL, \n " +
                    "LowWarningValue      FLOAT         NOT NULL, \n " +
                    "MaxValue             FLOAT         NOT NULL, \n " +
                    "MinValue             FLOAT         NOT NULL, \n " +
                    "Name                 TEXT          NOT NULL, \n " +
                    "NominalValue         FLOAT         NOT NULL, \n " +
                    "PersistDataPoints    BOOLEAN       NOT NULL, \n " +
                    "PGN                  INTEGER       NOT NULL, \n " +
                    "PortNumber           INTEGER       NOT NULL, \n " +
                    "Priority             INTEGER       NOT NULL, \n " +
                    "PropertyBag          TEXT          NOT NULL, \n " +
                    "Resolution           INTEGER       NOT NULL, \n " +
                    "SensorType           INTEGER       NOT NULL, \n " +
                    "SensorUnits          INTEGER       NOT NULL, \n " +
                    "SensorUsage          INTEGER       NOT NULL, \n " +
                    "SerialNumber         TEXT          NOT NULL, \n " +
                    "ShowNominalValue     BOOLEAN       NOT NULL, \n " +
                    "Throttle             INTEGER       NOT NULL, \n " +
                    "FOREIGN KEY(DeviceId) REFERENCES DeviceTable(DeviceId) \n" +
                ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT                    \n " +
                    "SensorId,             \n " +
                    "ChangeDate,           \n " +
                    "Description,          \n " +
                    "DeviceId,             \n " +
                    "GaugeUnits,           \n" +
                    "HighAlarmValue,       \n " +
                    "HighWarningValue,     \n " +
                    "IsCalibrated,         \n " +
                    "IsEnabled,            \n " +
                    "IsHighAlarmEnabled,   \n " +
                    "IsHighWarningEnabled, \n " +
                    "IsLowAlarmEnabled,    \n " +
                    "IsLowWarningEnabled,  \n " +
                    "IsOnline,             \n " +
                    "IsPersisted,          \n " +
                    "IsVirtual,            \n " +
                    "Location,             \n " +
                    "LowAlarmValue,        \n " +
                    "LowWarningValue,      \n " +
                    "MaxValue,             \n " +
                    "MinValue,             \n " +
                    "Name,                 \n " +
                    "NominalValue,         \n " +
                    "PersistDataPoints,    \n " +
                    "PGN,                  \n " +
                    "PortNumber,           \n " +
                    "Priority,             \n " +
                    "PropertyBag,          \n " +
                    "Resolution,           \n " +
                    "SensorType,           \n " +
                    "SensorUnits,          \n " +
                    "SensorUsage,          \n " +
                    "SerialNumber,         \n " +
                    "ShowNominalValue,     \n " +
                    "Throttle              \n " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@Description", itemRow.Field<string>("Description"));
            statement.Bind("@DeviceId", itemRow.Field<Int64>("DeviceId"));
            statement.Bind("@GaugeUnits", Convert.ToInt32(itemRow.Field<UnitType>("GaugeUnits")));
            statement.Bind("@HighAlarmValue", itemRow.Field<double>("HighAlarmValue"));
            statement.Bind("@HighWarningValue", itemRow.Field<double>("HighWarningValue"));
            statement.Bind("@IsCalibrated", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsCalibrated")));
            statement.Bind("@IsEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsEnabled")));
            statement.Bind("@IsHighAlarmEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsHighAlarmEnabled")));
            statement.Bind("@IsHighWarningEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsHighWarningEnabled")));
            statement.Bind("@IsLowAlarmEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsLowAlarmEnabled")));
            statement.Bind("@IsLowWarningEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsLowWarningEnabled")));
            statement.Bind("@IsOnline", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsOnline")));
            statement.Bind("@IsPersisted", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsPersisted")));
            statement.Bind("@IsVirtual", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsVirtual")));
            statement.Bind("@Location", itemRow.Field<string>("Location"));
            statement.Bind("@LowAlarmValue", itemRow.Field<double>("LowAlarmValue"));
            statement.Bind("@LowWarningValue", itemRow.Field<double>("LowWarningValue"));
            statement.Bind("@MaxValue", itemRow.Field<double>("MaxValue"));
            statement.Bind("@MinValue", itemRow.Field<double>("MinValue"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@NominalValue", itemRow.Field<double>("NominalValue"));
            statement.Bind("@PersistDataPoints", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("PersistDataPoints")));
            statement.Bind("@PGN", itemRow.Field<Int32>("PGN"));
            statement.Bind("@PortNumber", itemRow.Field<Int32>("PortNumber"));
            statement.Bind("@Priority", itemRow.Field<Int32>("Priority"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@Resolution", itemRow.Field<Int32>("Resolution"));
            statement.Bind("@SensorType", Convert.ToInt32(itemRow.Field<SensorType>("SensorType")));
            statement.Bind("@SensorUnits", Convert.ToInt32(itemRow.Field<UnitType>("SensorUnits")));
            statement.Bind("@SensorUsage", Convert.ToInt32(itemRow.Field<SensorUsage>("SensorUsage")));
            statement.Bind("@SerialNumber", itemRow.Field<string>("SerialNumber"));
            statement.Bind("@ShowNominalValue", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("ShowNominalValue")));
            statement.Bind("@Throttle", itemRow.Field<Int32>("Throttle"));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@Description", itemRow.Field<string>("Description"));
            statement.Bind("@DeviceId", itemRow.Field<Int64>("DeviceId"));
            statement.Bind("@GaugeUnits", Convert.ToInt32(itemRow.Field<UnitType>("GaugeUnits")));
            statement.Bind("@HighAlarmValue", itemRow.Field<double>("HighAlarmValue"));
            statement.Bind("@HighWarningValue", itemRow.Field<double>("HighWarningValue"));
            statement.Bind("@IsCalibrated", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsCalibrated")));
            statement.Bind("@IsEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsEnabled")));
            statement.Bind("@IsHighAlarmEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsHighAlarmEnabled")));
            statement.Bind("@IsHighWarningEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsHighWarningEnabled")));
            statement.Bind("@IsLowAlarmEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsLowAlarmEnabled")));
            statement.Bind("@IsLowWarningEnabled", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsLowWarningEnabled")));
            statement.Bind("@IsOnline", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsOnline")));
            statement.Bind("@IsPersisted", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsPersisted")));
            statement.Bind("@IsVirtual", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsVirtual")));
            statement.Bind("@Location", itemRow.Field<string>("Location"));
            statement.Bind("@LowAlarmValue", itemRow.Field<double>("LowAlarmValue"));
            statement.Bind("@LowWarningValue", itemRow.Field<double>("LowWarningValue"));
            statement.Bind("@MaxValue", itemRow.Field<double>("MaxValue"));
            statement.Bind("@MinValue", itemRow.Field<double>("MinValue"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@NominalValue", itemRow.Field<double>("NominalValue"));
            statement.Bind("@PersistDataPoints", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("PersistDataPoints")));
            statement.Bind("@PGN", itemRow.Field<Int32>("PGN"));
            statement.Bind("@PortNumber", itemRow.Field<Int32>("PortNumber"));
            statement.Bind("@Priority", itemRow.Field<Int32>("Priority"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@Resolution", itemRow.Field<Int32>("Resolution"));
            statement.Bind("@SensorType", Convert.ToInt32(itemRow.Field<SensorType>("SensorType")));
            statement.Bind("@SensorUnits", Convert.ToInt32(itemRow.Field<UnitType>("SensorUnits")));
            statement.Bind("@SensorUsage", Convert.ToInt32(itemRow.Field<SensorUsage>("SensorUsage")));
            statement.Bind("@SerialNumber", itemRow.Field<string>("SerialNumber"));
            statement.Bind("@ShowNominalValue", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("ShowNominalValue")));
            statement.Bind("@Throttle", itemRow.Field<Int32>("Throttle"));
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("ChangeDate", typeof(DateTime));
            itemTable.Columns.Add("Description", typeof(string));
            itemTable.Columns.Add("DeviceId", typeof(Int64));
            itemTable.Columns.Add("GaugeUnits", typeof(UnitType));
            itemTable.Columns.Add("HighAlarmValue", typeof(double));
            itemTable.Columns.Add("HighWarningValue", typeof(double));
            itemTable.Columns.Add("IsCalibrated", typeof(bool));
            itemTable.Columns.Add("IsEnabled", typeof(bool));
            itemTable.Columns.Add("IsHighAlarmEnabled", typeof(bool));
            itemTable.Columns.Add("IsHighWarningEnabled", typeof(bool));
            itemTable.Columns.Add("IsLowAlarmEnabled", typeof(bool));
            itemTable.Columns.Add("IsLowWarningEnabled", typeof(bool));
            itemTable.Columns.Add("IsPersisted", typeof(bool));
            itemTable.Columns.Add("IsVirtual", typeof(bool));
            itemTable.Columns.Add("Location", typeof(string));
            itemTable.Columns.Add("LowAlarmValue", typeof(double));
            itemTable.Columns.Add("LowWarningValue", typeof(double));
            itemTable.Columns.Add("MaxValue", typeof(double));
            itemTable.Columns.Add("MinValue", typeof(double));
            itemTable.Columns.Add("Name", typeof(string));
            itemTable.Columns.Add("NominalValue", typeof(double));
            itemTable.Columns.Add("PersistDataPoints", typeof(bool));
            itemTable.Columns.Add("PGN", typeof(Int32));
            itemTable.Columns.Add("PortNumber", typeof(Int32));
            itemTable.Columns.Add("Priority", typeof(Int32));
            itemTable.Columns.Add("PropertyBag", typeof(string));
            itemTable.Columns.Add("Resolution", typeof(Int32));
            itemTable.Columns.Add("SensorType", typeof(SensorType));
            itemTable.Columns.Add("SensorUnits", typeof(UnitType));
            itemTable.Columns.Add("SensorUsage", typeof(SensorUsage));
            itemTable.Columns.Add("SerialNumber", typeof(string));
            itemTable.Columns.Add("ShowNominalValue", typeof(bool));
            itemTable.Columns.Add("Throttle", typeof(Int32));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["ChangeDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["Description"].DefaultValue = "Description";
            itemTable.Columns["DeviceId"].DefaultValue = -1;
            itemTable.Columns["GaugeUnits"].DefaultValue = UnitType.Uninitialized;
            itemTable.Columns["HighAlarmValue"].DefaultValue = 95D;
            itemTable.Columns["HighWarningValue"].DefaultValue = 90D;
            itemTable.Columns["IsCalibrated"].DefaultValue = false;
            itemTable.Columns["IsEnabled"].DefaultValue = false;
            itemTable.Columns["IsHighAlarmEnabled"].DefaultValue = false;
            itemTable.Columns["IsHighWarningEnabled"].DefaultValue = false;
            itemTable.Columns["IsLowAlarmEnabled"].DefaultValue = false;
            itemTable.Columns["IsLowWarningEnabled"].DefaultValue = false;
            itemTable.Columns["IsPersisted"].DefaultValue = true;
            itemTable.Columns["IsVirtual"].DefaultValue = false;
            itemTable.Columns["Location"].DefaultValue = "Location";
            itemTable.Columns["LowAlarmValue"].DefaultValue = 5D;
            itemTable.Columns["LowWarningValue"].DefaultValue = 10D;
            itemTable.Columns["MaxValue"].DefaultValue = 100D;
            itemTable.Columns["MinValue"].DefaultValue = 0D;
            itemTable.Columns["Name"].DefaultValue = "Name";
            itemTable.Columns["NominalValue"].DefaultValue = 50D;
            itemTable.Columns["PersistDataPoints"].DefaultValue = true;
            itemTable.Columns["PGN"].DefaultValue = 0;
            itemTable.Columns["PortNumber"].DefaultValue = 0;
            itemTable.Columns["Priority"].DefaultValue = -1;
            itemTable.Columns["PropertyBag"].DefaultValue = new PropertyBag().JsonSerialize();
            itemTable.Columns["Resolution"].DefaultValue = 2;
            itemTable.Columns["SensorType"].DefaultValue = SensorType.Unknown;
            itemTable.Columns["SensorUnits"].DefaultValue = UnitType.Uninitialized;
            itemTable.Columns["SensorUsage"].DefaultValue = SensorUsage.Other;
            itemTable.Columns["SerialNumber"].DefaultValue = string.Empty;
            itemTable.Columns["ShowNominalValue"].DefaultValue = false;
            itemTable.Columns["Throttle"].DefaultValue = 2000;
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("ChangeDate", (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<string>("Description", (string)statement[02]);
            itemRow.SetField<Int64>("DeviceId", (Int64)Convert.ToInt64(statement[03]));
            itemRow.SetField<UnitType>("GaugeUnits", (UnitType)Convert.ToInt32(statement[04]));
            itemRow.SetField<double>("HighAlarmValue", (double)Convert.ToDouble(statement[05]));
            itemRow.SetField<double>("HighWarningValue", (double)Convert.ToDouble(statement[06]));
            itemRow.SetField<bool>("IsCalibrated", (bool)((Int64)statement[07] != 0));
            itemRow.SetField<bool>("IsEnabled", (bool)((Int64)statement[08] != 0));
            itemRow.SetField<bool>("IsHighAlarmEnabled", (bool)((Int64)statement[09] != 0));
            itemRow.SetField<bool>("IsHighWarningEnabled", (bool)((Int64)statement[10] != 0));
            itemRow.SetField<bool>("IsLowAlarmEnabled", (bool)((Int64)statement[11] != 0));
            itemRow.SetField<bool>("IsLowWarningEnabled", (bool)((Int64)statement[12] != 0));
            itemRow.SetField<bool>("IsOnline", (bool)((Int64)statement[13] != 0));
            itemRow.SetField<bool>("IsPersisted", (bool)((Int64)statement[14] != 0));
            itemRow.SetField<bool>("IsVirtual", (bool)((Int64)statement[15] != 0));
            itemRow.SetField<string>("Location", (string)statement[16]);
            itemRow.SetField<double>("LowAlarmValue", (double)Convert.ToDouble(statement[17]));
            itemRow.SetField<double>("LowWarningValue", (double)Convert.ToDouble(statement[18]));
            itemRow.SetField<double>("MaxValue", (double)Convert.ToDouble(statement[19]));
            itemRow.SetField<double>("MinValue", (double)Convert.ToDouble(statement[20]));
            itemRow.SetField<string>("Name", (string)statement[21]);
            itemRow.SetField<double>("NominalValue", (double)Convert.ToDouble(statement[22]));
            itemRow.SetField<bool>("PersistDataPoints", (bool)((Int64)statement[23] != 0));
            itemRow.SetField<Int32>("PGN", (Int32)Convert.ToInt32(statement[24]));
            itemRow.SetField<Int32>("PortNumber", (Int32)Convert.ToInt32(statement[25]));
            itemRow.SetField<Int32>("Priority", (Int32)Convert.ToInt32(statement[26]));
            itemRow.SetField<string>("PropertyBag", (string)statement[27]);
            itemRow.SetField<Int32>("Resolution", (Int32)Convert.ToInt32(statement[28]));
            itemRow.SetField<SensorType>("SensorType", (SensorType)Convert.ToInt32(statement[29]));
            itemRow.SetField<UnitType>("SensorUnits", (UnitType)Convert.ToInt32(statement[30]));
            itemRow.SetField<SensorUsage>("SensorUsage", (SensorUsage)Convert.ToInt32(statement[31]));
            itemRow.SetField<string>("SerialNumber", (string)statement[32]);
            itemRow.SetField<bool>("ShowNominalValue", (bool)((Int64)statement[33] != 0));
            itemRow.SetField<Int32>("Throttle", (Int32)Convert.ToInt32(statement[34]));

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                "( " +
                    "ChangeDate,           " +
                    "Description,          " +
                    "DeviceId,             " +
                    "GaugeUnits,           " +
                    "HighAlarmValue,       " +
                    "HighWarningValue,     " +
                    "IsCalibrated,         " +
                    "IsEnabled,            " +
                    "IsHighAlarmEnabled,   " +
                    "IsHighWarningEnabled, " +
                    "IsLowAlarmEnabled,    " +
                    "IsLowWarningEnabled,  " +
                    "IsOnline,             " +
                    "IsPersisted,          " +
                    "IsVirtual,            " +
                    "Location,             " +
                    "LowAlarmValue,        " +
                    "LowWarningValue,      " +
                    "MaxValue,             " +
                    "MinValue,             " +
                    "Name,                 " +
                    "NominalValue,         " +
                    "PersistDataPoints,    " +
                    "PGN,                  " +
                    "PortNumber,           " +
                    "Priority,             " +
                    "PropertyBag,          " +
                    "Resolution,           " +
                    "SensorType,           " +
                    "SensorUnits,          " +
                    "SensorUsage,          " +
                    "SerialNumber,         " +
                    "ShowNominalValue,     " +
                    "Throttle              " +
                ") " +
                "VALUES " +
                "( " +
                    "@ChangeDate,           " +
                    "@Description,          " +
                    "@DeviceId,             " +
                    "@GaugeUnits,           " +
                    "@HighAlarmValue,       " +
                    "@HighWarningValue,     " +
                    "@IsCalibrated,         " +
                    "@IsEnabled,            " +
                    "@IsHighAlarmEnabled,   " +
                    "@IsHighWarningEnabled, " +
                    "@IsLowAlarmEnabled,    " +
                    "@IsLowWarningEnabled,  " +
                    "@IsOnline,             " +
                    "@IsPersisted,          " +
                    "@IsVirtual,            " +
                    "@Location,             " +
                    "@LowAlarmValue,        " +
                    "@LowWarningValue,      " +
                    "@MaxValue,             " +
                    "@MinValue,             " +
                    "@Name,                 " +
                    "@NominalValue,         " +
                    "@PersistDataPoints,    " +
                    "@PGN,                  " +
                    "@PortNumber,           " +
                    "@Priority,             " +
                    "@PropertyBag,          " +
                    "@Resolution,           " +
                    "@SensorType,           " +
                    "@SensorUnits,          " +
                    "@SensorUsage,          " +
                    "@SerialNumber,         " +
                    "@ShowNominalValue,     " +
                    "@Throttle              " +
                ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return
                "UPDATE " + TableName + " \n" +
                " SET \n" +
                    "ChangeDate           = @ChangeDate,           \n" +
                    "Description          = @Description,          \n" +
                    "DeviceId             = @DeviceId,             \n" +
                    "GaugeUnits           = @GaugeUnits,           \n" +
                    "HighAlarmValue       = @HighAlarmValue,       \n" +
                    "HighWarningValue     = @HighWarningValue,     \n" +
                    "IsCalibrated         = @IsCalibrated,         \n" +
                    "IsEnabled            = @IsEnabled,            \n" +
                    "IsHighAlarmEnabled   = @IsHighAlarmEnabled,   \n" +
                    "IsHighWarningEnabled = @IsHighWarningEnabled, \n" +
                    "IsLowAlarmEnabled    = @IsLowAlarmEnabled,    \n" +
                    "IsLowWarningEnabled  = @IsLowWarningEnabled,  \n" +
                    "IsOnline             = @IsOnline,             \n" +
                    "IsPersisted          = @IsPersisted,          \n" +
                    "IsVirtual            = @IsVirtual,            \n" +
                    "Location             = @Location,             \n" +
                    "LowAlarmValue        = @LowAlarmValue,        \n" +
                    "LowWarningValue      = @LowWarningValue,      \n" +
                    "MaxValue             = @MaxValue,             \n" +
                    "MinValue             = @MinValue,             \n" +
                    "Name                 = @Name,                 \n" +
                    "NominalValue         = @NominalValue,         \n" +
                    "PersistDataPoints    = @PersistDataPoints,    \n" +
                    "PGN                  = @PGN,                  \n" +
                    "PortNumber           = @PortNumber,           \n" +
                    "Priority             = @Priority,             \n" +
                    "PropertyBag          = @PropertyBag,          \n" +
                    "Resolution           = @Resolution,           \n" +
                    "SensorType           = @SensorType,           \n" +
                    "SensorUnits          = @SensorUnits,          \n" +
                    "SensorUsage          = @SensorUsage,          \n" +
                    "SerialNumber         = @SerialNumber,         \n" +
                    "ShowNominalValue     = @ShowNominalValue,     \n" +
                    "Throttle             = @Throttle              \n" +
                "WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }

        override protected string GetDeleteItemSql()
        {
            return
                "DELETE FROM " + TableName +
                "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }
}
