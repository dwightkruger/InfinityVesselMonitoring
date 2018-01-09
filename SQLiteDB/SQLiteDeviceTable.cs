//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using SQLitePCL;
using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteDeviceTable : SQLiteVesselTableBase<ItemRow, Int64>, IDeviceTable
    {
        public SQLiteDeviceTable(IVesselDB myVesselDB) : base(myVesselDB)
        {
            PrimaryKeyName = "DeviceId";
            TableName = "DeviceTable";
        }

        async public Task<int> TotalDevices()
        {
            string query = "SELECT COUNT(" + this.PrimaryKeyName + ") AS TotalDevices FROM " + this.TableName;
            return await base.TotalItems(query);
        }

        protected override string GetCreateTableSql()
        {
            return
                "CREATE TABLE IF NOT EXISTS " + TableName + "\n" +
                "(\n" +
                    "DeviceId        INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT, \n " +
                    "ChangeDate      DATETIME NOT NULL, \n " +
                    "DeviceAddress   TINYINT  NOT NULL, \n " +
                    "DeviceType      INTEGER  NOT NULL, \n " +
                    "Description     TEXT     NOT NULL, \n " +
                    "FirmwareVersion TEXT     NOT NULL, \n " +
                    "HardwareVersion TEXT     NOT NULL, \n " +
                    "LastUpdate      DATETIME NOT NULL, \n " +
                    "Location        TEXT     NOT NULL, \n " +
                    "Model           TEXT     NOT NULL, \n " +
                    "Name            TEXT     NOT NULL, \n " +
                    "Manufacturer    TEXT     NOT NULL, \n " +
                    "PropertyBag     TEXT     NOT NULL, \n " +
                    "ReceivePGNList  TEXT     NOT NULL, \n " +
                    "SerialNumber    TEXT     NOT NULL, \n " +
                    "SoftwareVersion TEXT     NOT NULL, \n " +
                    "TransmitPGNList TEXT     NOT NULL  \n " +
                    ") ";
        }

        protected override string GetSelectAllSql()
        {
            return
                "SELECT " +
                    "DeviceId, " +
                    "ChangeDate, " +
                    "DeviceAddress, " +
                    "DeviceType, " +
                    "Description, " +
                    "FirmwareVersion, " +
                    "HardwareVersion, " +
                    "LastUpdate, " +
                    "Location, " +
                    "Model, " +
                    "Name, " +
                    "Manufacturer, " +
                    "PropertyBag, " +
                    "ReceivePGNList, " +
                    "SerialNumber, " +
                    "SoftwareVersion, " +
                    "TransmitPGNList " +
                "FROM " + TableName;
        }


        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow itemRow)
        {
            itemRow.SetField<DateTime>("ChangeDate", DateTime.Now.ToUniversalTime());

            statement.Bind("@ChangeDate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("ChangeDate")));
            statement.Bind("@DeviceAddress", itemRow.Field<byte>("DeviceAddress"));
            statement.Bind("@DeviceType", itemRow.Field<Int32>("DeviceType"));
            statement.Bind("@Description", itemRow.Field<string>("Description"));
            statement.Bind("@FirmwareVersion", itemRow.Field<string>("FirmwareVersion"));
            statement.Bind("@HardwareVersion", itemRow.Field<string>("HardwareVersion"));
            statement.Bind("@LastUpdate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("LastUpdate")));
            statement.Bind("@Location", itemRow.Field<string>("Location"));
            statement.Bind("@Model", itemRow.Field<string>("Model"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@Manufacturer", itemRow.Field<string>("Manufacturer"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@ReceivePGNList", itemRow.Field<string>("ReceivePGNList"));
            statement.Bind("@SerialNumber", itemRow.Field<string>("SerialNumber"));
            statement.Bind("@SoftwareVersion", itemRow.Field<string>("SoftwareVersion"));
            statement.Bind("@TransmitPGNList", itemRow.Field<string>("TransmitPGNList"));
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
            statement.Bind("@DeviceAddress", itemRow.Field<byte>("DeviceAddress"));
            statement.Bind("@DeviceType", itemRow.Field<Int32>("DeviceType"));
            statement.Bind("@Description", itemRow.Field<string>("Description"));
            statement.Bind("@FirmwareVersion", itemRow.Field<string>("FirmwareVersion"));
            statement.Bind("@HardwareVersion", itemRow.Field<string>("HardwareVersion"));
            statement.Bind("@LastUpdate", SQLiteDB.Utilities.DateTimeSQLite(itemRow.Field<DateTime>("LastUpdate")));
            statement.Bind("@Location", itemRow.Field<string>("Location"));
            statement.Bind("@Model", itemRow.Field<string>("Model"));
            statement.Bind("@Name", itemRow.Field<string>("Name"));
            statement.Bind("@Manufacturer", itemRow.Field<string>("Manufacturer"));
            statement.Bind("@PropertyBag", itemRow.Field<string>("PropertyBag"));
            statement.Bind("@ReceivePGNList", itemRow.Field<string>("ReceivePGNList"));
            statement.Bind("@SerialNumber", itemRow.Field<string>("SerialNumber"));
            statement.Bind("@SoftwareVersion", itemRow.Field<string>("SoftwareVersion"));
            statement.Bind("@TransmitPGNList", itemRow.Field<string>("TransmitPGNList"));
        }

        protected override void LoadTableRow(ISQLiteStatement statement)
        {
            ItemRow itemRow = this.CreateRow();

            itemRow.SetField<Int64>(PrimaryKeyName, (Int64)Convert.ToInt64(statement[00]));
            itemRow.SetField<DateTime>("ChangeDate", (DateTime)DateTime.Parse((string)statement[01]));
            itemRow.SetField<byte>("DeviceAddress", (byte)Convert.ToByte(statement[02]));
            itemRow.SetField<DeviceType>("DeviceType", (DeviceType)Convert.ToInt32(statement[03]));
            itemRow.SetField<string>("Description", (string)statement[04]);
            itemRow.SetField<string>("FirmwareVersion", (string)statement[05]);
            itemRow.SetField<string>("HardwareVersion", (string)statement[06]);
            itemRow.SetField<DateTime>("LastUpdate", (DateTime)DateTime.Parse((string)statement[07]));
            itemRow.SetField<string>("Location", (string)statement[08]);
            itemRow.SetField<string>("Model", (string)statement[09]);
            itemRow.SetField<string>("Name", (string)statement[10]);
            itemRow.SetField<string>("Manufacturer", (string)statement[11]);
            itemRow.SetField<string>("PropertyBag", (string)statement[12]);
            itemRow.SetField<string>("ReceivePGNList", (string)statement[13]);
            itemRow.SetField<string>("SerialNumber", (string)statement[14]);
            itemRow.SetField<string>("SoftwareVersion", (string)statement[15]);
            itemRow.SetField<string>("TransmitPGNList", (string)statement[16]);

            this.ItemTable.Rows.Add(itemRow);
            itemRow.AcceptChanges();
        }

        protected override void CreateTableSchema(ItemTable itemTable)
        {
            itemTable.Columns.Add(PrimaryKeyName, typeof(Int64));
            itemTable.Columns.Add("ChangeDate", typeof(DateTime));
            itemTable.Columns.Add("DeviceAddress", typeof(byte));
            itemTable.Columns.Add("DeviceType", typeof(DeviceType));
            itemTable.Columns.Add("Description", typeof(string));
            itemTable.Columns.Add("FirmwareVersion", typeof(string));
            itemTable.Columns.Add("HardwareVersion", typeof(string));
            itemTable.Columns.Add("LastUpdate", typeof(DateTime));
            itemTable.Columns.Add("Location", typeof(string));
            itemTable.Columns.Add("Model", typeof(string));
            itemTable.Columns.Add("Name", typeof(string));
            itemTable.Columns.Add("Manufacturer", typeof(string));
            itemTable.Columns.Add("PropertyBag", typeof(string));
            itemTable.Columns.Add("ReceivePGNList", typeof(string));
            itemTable.Columns.Add("SerialNumber", typeof(string));
            itemTable.Columns.Add("SoftwareVersion", typeof(string));
            itemTable.Columns.Add("TransmitPGNList", typeof(string));

            itemTable.Columns[PrimaryKeyName].DefaultValue = -1L;
            itemTable.Columns["ChangeDate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["DeviceAddress"].DefaultValue = Convert.ToByte(0);
            itemTable.Columns["DeviceType"].DefaultValue = DeviceType.Unknown;
            itemTable.Columns["Description"].DefaultValue = string.Empty;
            itemTable.Columns["FirmwareVersion"].DefaultValue = string.Empty;
            itemTable.Columns["HardwareVersion"].DefaultValue = string.Empty;
            itemTable.Columns["LastUpdate"].DefaultValue = DateTime.Now.ToUniversalTime();
            itemTable.Columns["Location"].DefaultValue = string.Empty;
            itemTable.Columns["Model"].DefaultValue = string.Empty;
            itemTable.Columns["Name"].DefaultValue = string.Empty;
            itemTable.Columns["Manufacturer"].DefaultValue = string.Empty;
            itemTable.Columns["PropertyBag"].DefaultValue = new PropertyBag().JsonSerialize();
            itemTable.Columns["ReceivePGNList"].DefaultValue = string.Empty;
            itemTable.Columns["SerialNumber"].DefaultValue = string.Empty;
            itemTable.Columns["SoftwareVersion"].DefaultValue = string.Empty;
            itemTable.Columns["TransmitPGNList"].DefaultValue = string.Empty;
        }

        override protected string GetInsertItemSql()
        {
            return
                "INSERT INTO " + TableName +
                          "( " +
                                "ChangeDate, " +
                                "DeviceAddress, " +
                                "DeviceType, " +
                                "Description, " +
                                "FirmwareVersion, " +
                                "HardwareVersion, " +
                                "LastUpdate, " +
                                "Location, " +
                                "Model, " +
                                "Name, " +
                                "Manufacturer, " +
                                "PropertyBag, " +
                                "ReceivePGNList, " +
                                "SerialNumber, " +
                                "SoftwareVersion, " +
                                "TransmitPGNList  " +
                          ") " +
                          "VALUES " +
                          "( " +
                                "@ChangeDate, " +
                                "@DeviceAddress, " +
                                "@DeviceType, " +
                                "@Description, " +
                                "@FirmwareVersion, " +
                                "@HardwareVersion, " +
                                "@LastUpdate, " +
                                "@Location, " +
                                "@Model, " +
                                "@Name, " +
                                "@Manufacturer, " +
                                "@PropertyBag, " +
                                "@ReceivePGNList, " +
                                "@SerialNumber, " +
                                "@SoftwareVersion, " +
                                "@TransmitPGNList  " +
                          ") ";

        }

        override protected string GetUpdateItemSql()
        {
            return "UPDATE " + TableName + "\n" +
                   " SET \n" +
                   "     ChangeDate        = @ChangeDate,      \n" +
                   "     DeviceAddress     = @DeviceAddress,   \n" +
                   "     DeviceType        = @DeviceType,      \n" +
                   "     Description       = @Description,     \n" +
                   "     FirmwareVersion   = @FirmwareVersion, \n" +
                   "     HardwareVersion   = @HardwareVersion, \n" +
                   "     LastUpdate        = @LastUpdate,      \n" +
                   "     Location          = @Location,        \n" +
                   "     Model             = @Model,           \n" +
                   "     Name              = @Name,            \n" +
                   "     Manufacturer      = @Manufacturer,    \n" +
                   "     PropertyBag       = @PropertyBag,     \n" +
                   "     ReceivePGNList    = @ReceivePGNList,  \n" +
                   "     SerialNumber      = @SerialNumber,    \n" +
                   "     SoftwareVersion   = @SoftwareVersion, \n" +
                   "     TransmitPGNList   = @TransmitPGNList  \n" +
                   "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }


        override protected string GetDeleteItemSql()
        {
            return "DELETE FROM " + TableName +
                    "  WHERE " + PrimaryKeyName + " = @" + PrimaryKeyName;
        }
    }

}
