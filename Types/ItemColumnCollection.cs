//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class ItemColumnCollection : ObservableCollection<ItemColumn>
    {
        public void Add(string myColumnName, Type myColumnType)
        {
            ItemColumn column = new ItemColumn(myColumnName, myColumnType);

            base.Add(column);
        }

        public ItemColumn this[string columnName]
        {
            get
            {
                return this.FirstOrDefault<ItemColumn>((col) => col.ColumnName == columnName);
            }
        }

        public bool Contains(string columnName)
        {
            ItemColumn itemColumn = this.FirstOrDefault<ItemColumn>((col) => col.ColumnName == columnName);

            return (null != itemColumn) && (itemColumn.ColumnName == columnName);
        }

        public void Remove(string columnName)
        {
            ItemColumn itemColumn = this.FirstOrDefault<ItemColumn>((col) => col.ColumnName == columnName);

            if ((null != itemColumn) && (itemColumn.ColumnName == columnName))
            {
                this.Remove(itemColumn);
            }
        }
    }
}
