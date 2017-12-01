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
    public class ItemRowCollection : ObservableCollection<ItemRow>
    {
        public new void Add(ItemRow row)
        {
            row.RowState = ItemRowState.Added;
            base.Add(row);
        }

        public ItemRow Find(Int64 id, string primaryKey)
        {
            var result = this.FirstOrDefault<ItemRow>(item => item.Field<Int64>(primaryKey) == id);

            return result;
        }
    }
}
