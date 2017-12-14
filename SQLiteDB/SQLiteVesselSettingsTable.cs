//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Interfaces;
using SQLitePCL;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteVesselSettingsTable : SQLiteVesselTableBase<ItemRow, Int64>, IVesselSettingsTable
    {
        public SQLiteVesselSettingsTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "KeyId";
            TableName = "VesselSettingsTable";
        }

        public async Task BeginRemove(string propertyName)
        {
            await Task.Run(() =>
            {
                string sqlCmd = "REMOVE FROM " + TableName + " \n" +
                                " WHERE Property = ' " + propertyName + "';";

                using (var statement = sqlConnection.Prepare(sqlCmd))
                {
                    statement.Step();
                }
            });
        }
        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + " \n" +
                "(\n" +
                    "KeyId      INTEGER       NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "Property   TEXT          NOT NULL CONSTRAINT CeProp_Unique UNIQUE, \n" +
                    "ChangeDate DATETIME      NOT NULL, \n" +
                    Constants.c_SystemInt64     + " INTEGER  NULL, \n" +
                    Constants.c_SystemDouble    + " FLOAT    NULL, \n" +
                    Constants.c_SystemString    + " NTEXT    NULL, \n" +
                    Constants.c_SystemByteArray + " BLOB     NULL, \n" +
                    Constants.c_SystemDateTime  + " DATETIME NULL, \n" +
                    Constants.c_SystemBoolean   + " BOOLEAN  NULL  \n" +
                ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "KeyId, " +
                    "Property, " +
                    "ChangeDate, " +
                     Constants.c_SystemInt64     + ", " +
                     Constants.c_SystemDouble    + ", " +
                     Constants.c_SystemString    + ", " +
                     Constants.c_SystemByteArray + ", " +
                     Constants.c_SystemDateTime  + ", " +
                     Constants.c_SystemBoolean   + "  " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@Property", itemRow.Field<string>("Property"));
            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@" + Constants.c_SystemInt64, itemRow.Field<Int64>(Constants.c_SystemInt64));
            statement.Bind("@" + Constants.c_SystemDouble, itemRow.Field<double>(Constants.c_SystemDouble));
            statement.Bind("@" + Constants.c_SystemString, itemRow.Field<string>(Constants.c_SystemString));
            statement.Bind("@" + Constants.c_SystemByteArray, itemRow.Field<byte[]>(Constants.c_SystemByteArray));
            statement.Bind("@" + Constants.c_SystemDateTime, SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>(Constants.c_SystemDateTime)));
            statement.Bind("@" + Constants.c_SystemBoolean, SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>(Constants.c_SystemBoolean)));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@Property", itemRow.Field<string>("Property"));
            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@" + Constants.c_SystemInt64, itemRow.Field<Int64>(Constants.c_SystemInt64));
            statement.Bind("@" + Constants.c_SystemDouble, itemRow.Field<double>(Constants.c_SystemDouble));
            statement.Bind("@" + Constants.c_SystemString, itemRow.Field<string>(Constants.c_SystemString));
            statement.Bind("@" + Constants.c_SystemByteArray, itemRow.Field<byte[]>(Constants.c_SystemByteArray));
            statement.Bind("@" + Constants.c_SystemDateTime, SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>(Constants.c_SystemDateTime)));
            statement.Bind("@" + Constants.c_SystemBoolean, SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>(Constants.c_SystemBoolean)));
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("Property", typeof(DateTime));
            itemTable.Columns.Add("ChangeDate", typeof(byte));
            itemTable.Columns.Add(Constants.c_SystemInt64, typeof(Int64));
            itemTable.Columns.Add(Constants.c_SystemDouble, typeof(double));
            itemTable.Columns.Add(Constants.c_SystemString, typeof(string));
            itemTable.Columns.Add(Constants.c_SystemByteArray, typeof(byte[]));
            itemTable.Columns.Add(Constants.c_SystemDateTime, typeof(DateTime));
            itemTable.Columns.Add(Constants.c_SystemBoolean, typeof(bool));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["Property"].DefaultValue = string.Empty;
            itemTable.Columns["ChangeDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns[Constants.c_SystemInt64].DefaultValue = Int64.MinValue;
            itemTable.Columns[Constants.c_SystemDouble].DefaultValue = double.MinValue;
            itemTable.Columns[Constants.c_SystemString].DefaultValue = string.Empty;
            itemTable.Columns[Constants.c_SystemByteArray].DefaultValue = new byte[0];
            itemTable.Columns[Constants.c_SystemDateTime].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns[Constants.c_SystemBoolean].DefaultValue = true;
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>("KeyId", (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<string>("Property", (string)statement[01]);
            itemRow.SetField<DateTime>("ChangeDate", (DateTime)DateTime.Parse((string)statement[02]));
            itemRow.SetField<Int64>(Constants.c_SystemInt64, (Int64)Convert.ToInt64(statement[03]));
            itemRow.SetField<double>(Constants.c_SystemDouble, (double)Convert.ToDouble(statement[04]));
            itemRow.SetField<string>(Constants.c_SystemString, (string)statement[05]);
            itemRow.SetField<byte[]>(Constants.c_SystemByteArray, (byte[])statement[06]);
            itemRow.SetField<DateTime>(Constants.c_SystemDateTime, (DateTime)DateTime.Parse((string)statement[07]));
            itemRow.SetField<bool>(Constants.c_SystemBoolean, (bool)((Int64)statement[08] != 0));

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                "( " +
                    "Property, " +
                    "ChangeDate, " +
                    Constants.c_SystemInt64     + ", " +
                    Constants.c_SystemDouble    + ", " +
                    Constants.c_SystemString    + ", " +
                    Constants.c_SystemByteArray + ", " +
                    Constants.c_SystemDateTime  + ", " +
                    Constants.c_SystemBoolean   + "  " +
                ") " +
                "VALUES " +
                "( " +
                    "@Property,   \n" +
                    "@ChangeDate, \n" +
                    "@" + Constants.c_SystemInt64     + ", \n" +
                    "@" + Constants.c_SystemDouble    + ", \n" +
                    "@" + Constants.c_SystemString    + ", \n" +
                    "@" + Constants.c_SystemByteArray + ", \n" +
                    "@" + Constants.c_SystemDateTime  + ", \n" +
                    "@" + Constants.c_SystemBoolean   + "  \n" +
                ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return
                "UPDATE " + TableName + "\n" +
                " SET \n" +
                    "Property " + " = @Property, \n" +
                    "ChangeDate " + " = @ChangeDate, \n" +
                    Constants.c_SystemInt64 + " = @"     + Constants.c_SystemInt64 + ", \n" +
                    Constants.c_SystemDouble + " = @"    + Constants.c_SystemDouble + ", \n" +
                    Constants.c_SystemString + " = @"    + Constants.c_SystemString + ", \n" +
                    Constants.c_SystemByteArray + " = @" + Constants.c_SystemByteArray + ", \n" +
                    Constants.c_SystemDateTime + " = @"  + Constants.c_SystemDateTime + ", \n" +
                    Constants.c_SystemBoolean + " = @"   + Constants.c_SystemBoolean + "  \n" +
                "  WHERE " + PrimaryKeyName + " = @"     + PrimaryKeyName;
        }

        override protected string GetDeleteItemSql()
        {
            return
                "DELETE FROM " + TableName +
                "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }
}
