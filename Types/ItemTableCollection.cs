//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class ItemTableCollection : ObservableCollection<ItemTable>
    {
        public void Remove(string myTableName)
        {
            ItemTable firstItem = this.First<ItemTable>((item) => item.TableName == myTableName);
            if (null != firstItem)
            {
                this.Remove(firstItem);
            }
        }

        public ItemTable this[string myTableName]
        {
            get
            {
                return this.FirstOrDefault<ItemTable>((item) => item.TableName == myTableName);
            }
        }

        public bool Contains(string myTableName)
        {
            ItemTable itemTable = this.FirstOrDefault<ItemTable>((item) => item.TableName == myTableName);

            return (null != itemTable) && (itemTable.TableName == myTableName);
        }

    }
}
