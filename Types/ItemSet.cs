//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class ItemSet
    {
        public ItemSet() : this(string.Empty)
        {
        }

        public ItemSet(string myItemSetName)
        {
            this.ItemSetName = myItemSetName;
            this.Tables = new ItemTableCollection();

            this.Tables.CollectionChanged += Tables_CollectionChanged;
        }

        void Tables_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                foreach (ItemTable table in e.NewItems)
                {
                    table.ItemSet = this;
                }
            }
        }

        public void AcceptChanges()
        {
            foreach (ItemTable table in this.Tables)
            {
                table.AcceptChanges();
            }
        }

        public void Clear()
        {
            foreach (ItemTable table in this.Tables)
            {
                table.Clear();
            }
        }
        public string ItemSetName { get; set; }

        public ItemTableCollection Tables { get; set; }
    }
}
