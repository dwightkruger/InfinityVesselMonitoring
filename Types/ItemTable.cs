//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class ItemTable
    {
        public ItemTable() : this(string.Empty)
        {
        }

        public ItemTable(string myTableName)
        {
            this.TableName = myTableName;
            this.Rows = new ItemRowCollection();
            this.Columns = new ItemColumnCollection();
        }

        public void AcceptChanges()
        {
            int i = 0;
            while (i < this.Rows.Count)
            {
                ItemRow row = this.Rows[i];
                if (row.RowState == ItemRowState.Deleted)
                {
                    this.Rows.Remove(row);
                }
                else
                {
                    row.AcceptChanges();
                    i++;
                }
            }
        }

        public void Clear()
        {
            this.Rows.Clear();
        }

        [DataMember(Name = "Columns")]
        public ItemColumnCollection Columns { get; set; }

        [DataMember(Name = "ItemSet")]
        public ItemSet ItemSet { get; set; }

        public ItemRow NewRow()
        {
            ItemRow row = new ItemRow();
            row.Table = this;

            foreach (ItemColumn column in this.Columns)
            {
                row.PropertyBlob.Add(column.ColumnName);
                row.SetField(column.ColumnName, column.DefaultValue);
            }

            row.AcceptChanges();

            row.RowState = ItemRowState.Detached;

            return row;
        }

        [DataMember(Name = "PrimaryKey")]
        public ItemColumn[] PrimaryKey { get; set; }

        public void RejectChanges()
        {
            int i = 0;
            while (i < this.Rows.Count)
            {
                ItemRow row = this.Rows[i];
                if (row.RowState == ItemRowState.Added)
                {
                    this.Rows.Remove(row);
                }
                else
                {
                    row.RejectChanges();
                    i++;
                }
            }
        }

        [DataMember(Name = "ItemRowCollection")]
        public ItemRowCollection Rows { get; set; }

        [DataMember(Name = "TableName")]
        public string TableName { get; set; }

        public string JsonSerialize()
        {
            return JsonConvert.SerializeObject(this,
                Formatting.Indented,
                new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
        }

        static public ItemTable JsonDeseralize(string json)
        {
            ItemTable restore = JsonConvert.DeserializeObject<ItemTable>(json);

            foreach (ItemRow row in restore.Rows)
            {
                row.Table = restore;
            }

            return restore;
        }
    }

}
