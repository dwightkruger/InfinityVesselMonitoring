//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using SQLitePCL;
using System;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteAISTable : SQLiteVesselTableBase<ItemRow, Int64>, IAISTable
    {
        public SQLiteAISTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "MMSI";
            TableName = "AISTable";
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "MMSI            INTEGER  NOT NULL PRIMARY KEY, \n " +
                    "ChangeDate      DATETIME NOT NULL, \n " +
                    "Beam            FLOAT    NOT NULL, \n " +
                    "Callsign        TEXT     NOT NULL, \n " +
                    "Draft           FLOAT    NOT NULL, \n " +
                    "Length          FLOAT    NOT NULL, \n " +
                    "Name            TEXT     NOT NULL, \n " +
                    "PropertyBag     TEXT     NOT NULL  \n " +
                 ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "MMSI, " +
                    "ChangeDate, " +
                    "Beam, " +
                    "Callsign, " +
                    "Draft, " +
                    "Length, " +
                    "Name, " +
                    "PropertyBag " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.Now.ToUniversalTime());

            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@Beam", itemRow.Field<float>("Beam"));
            statement.Bind("@Callsign", itemRow.Field<string>("Callsign"));
            statement.Bind("@Draft", itemRow.Field<float>("Draft"));
            statement.Bind("@Length", itemRow.Field<float>("Length"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            statement.Bind("@" + PrimaryKeyName, key);
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.Now.ToUniversalTime());

            statement.Bind("@" + PrimaryKeyName, itemRow.Field<Int64>(PrimaryKeyName));
            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@Beam", itemRow.Field<float>("Beam"));
            statement.Bind("@Callsign", itemRow.Field<string>("Callsign"));
            statement.Bind("@Draft", itemRow.Field<float>("Draft"));
            statement.Bind("@Length", itemRow.Field<float>("Length"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("ChangeDate", (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<float>("Beam", (float)Convert.ToSingle(statement[02]));
            itemRow.SetField<string>("Callsign", (string)statement[03]);
            itemRow.SetField<float>("Draft", (float)Convert.ToSingle(statement[04]));
            itemRow.SetField<float>("Length", (float)Convert.ToSingle(statement[05]));
            itemRow.SetField<string>("Name", (string)statement[06]);
            itemRow.SetField<string>("PropertyBag", (string)statement[07]);

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("ChangeDate", typeof(DateTime));
            itemTable.Columns.Add("Beam", typeof(float));
            itemTable.Columns.Add("Callsign", typeof(string));
            itemTable.Columns.Add("Draft", typeof(float));
            itemTable.Columns.Add("Length", typeof(float));
            itemTable.Columns.Add("Name", typeof(string));
            itemTable.Columns.Add("PropertyBag", typeof(string));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["ChangeDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["Beam"].DefaultValue = 0f;
            itemTable.Columns["Callsign"].DefaultValue = string.Empty;
            itemTable.Columns["Draft"].DefaultValue = 0f;
            itemTable.Columns["Length"].DefaultValue = 0f;
            itemTable.Columns["Name"].DefaultValue = string.Empty;
            itemTable.Columns["PropertyBag"].DefaultValue = new PropertyBag().JsonSerialize();
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                    " ( " +
                        "MMSI, " +
                        "ChangeDate, " +
                        "Beam, " +
                        "Callsign, " +
                        "Draft, " +
                        "Length, " +
                        "Name, " +
                        "PropertyBag " +
                    " ) " +
                    " VALUES " +
                    " ( " +
                        "@MMSI, " +
                        "@ChangeDate, " +
                        "@Beam, " +
                        "@Callsign, " +
                        "@Draft, " +
                        "@Length, " +
                        "@Name, " +
                        "@PropertyBag " +
                    " ) ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     ChangeDate   = @ChangeDate,    \n" +
                   "     Beam         = @Beam,          \n" +
                   "     Callsign     = @Callsign,      \n" +
                   "     Draft        = @Draft,         \n" +
                   "     Length       = @Length,        \n" +
                   "     Name         = @Name,          \n" +
                   "     PropertyBag  = @PropertyBag    \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                    "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }

}
