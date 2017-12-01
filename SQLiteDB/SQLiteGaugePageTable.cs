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
    public class SQLiteGaugePageTable : SQLiteVesselTableBase<ItemRow, Int64>, IGaugePageTable
    {
        public SQLiteGaugePageTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "PageId";
            TableName = "GaugePageTable";
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "PageId         INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "ChangeDate     DATETIME NOT NULL, \n " +
                    "IsVisible      BOOL     NOT NULL, \n " +
                    "PageName       TEXT     NOT NULL, \n " +
                    "Position       INTEGER  NOT NULL, \n" +
                    "PropertyBag    TEXT     NOT NULL  \n " +
                    ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "PageId, " +
                    "ChangeDate, " +
                    "IsVisible, " +
                    "PageName, " +
                    "Position, " +
                    "PropertyBag " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.UtcNow);

            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@IsVisible", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsVisible")));
            statement.Bind("@PageName", itemRow.Field<string>("PageName"));
            statement.Bind("@Position", itemRow.Field<Int32>("Position"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
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
            statement.Bind("@IsVisible", SQLiteDB.Utilities.BooleanSQLite(itemRow.Field<bool>("IsVisible")));
            statement.Bind("@PageName", itemRow.Field<string>("PageName"));
            statement.Bind("@Position", itemRow.Field<Int32>("Position"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("ChangeDate", (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<bool>("IsVisible", (bool)((Int64)statement[02] != 0));
            itemRow.SetField<string>("PageName", (string)statement[03]);
            itemRow.SetField<Int32>("Position", (Int32)Convert.ToInt32(statement[04]));
            itemRow.SetField<string>("PropertyBag", (string)statement[05]);

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("ChangeDate", typeof(DateTime));
            itemTable.Columns.Add("IsVisible", typeof(bool));
            itemTable.Columns.Add("PageName", typeof(string));
            itemTable.Columns.Add("Position", typeof(Int32));
            itemTable.Columns.Add("PropertyBag", typeof(string));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["ChangeDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["IsVisible"].DefaultValue = true;
            itemTable.Columns["PageName"].DefaultValue = string.Empty;
            itemTable.Columns["Position"].DefaultValue = 0;
            itemTable.Columns["PropertyBag"].DefaultValue = new PropertyBag().JsonSerialize();
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName + "\n" +
                          "( \n" +
                                "ChangeDate, \n" +
                                "IsVisible,  \n" +
                                "PageName,   \n" +
                                "Position,   \n" +
                                "PropertyBag \n" +
                          ") \n" +
                          "VALUES \n" +
                          "( \n" +
                                "@ChangeDate, \n" +
                                "@IsVisible,  \n" +
                                "@PageName,   \n" +
                                "@Position,   \n" +
                                "@PropertyBag \n" +
                          ") ";
        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     ChangeDate  = @ChangeDate, \n" +
                   "     IsVisible   = @IsVisible,  \n" +
                   "     PageName    = @PageName,   \n" +
                   "     Position    = @Position,   \n" +
                   "     PropertyBag = @PropertyBag \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }

}
