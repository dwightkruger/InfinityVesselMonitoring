//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    /// <summary>
    /// This class maintains a collection of all of the PAGES which contain gauges. It reads and writes the collection to 
    /// the backing SQL database so that our gauge pages can be persisted.
    /// </summary>
    public class GaugePageCollection : ObservableCollection<IGaugePageItem>
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public GaugePageCollection()
        {
        }

        async public Task<IGaugePageItem> BeginAddPage(IGaugePageItem item)
        {
            IGaugePageItem newGaugePageItem = null;

            // Is this item already in our list?
            newGaugePageItem = FindByPageId(item.PageId);

            // If not in the list already, add it.
            if (null == newGaugePageItem)
            {
                newGaugePageItem = item;
                using (var releaser = await _lock.WriterLockAsync())
                {
                    base.Add(newGaugePageItem);
                }

                await newGaugePageItem.BeginCommit();
            }

            return newGaugePageItem;
        }

        public async Task BeginCommitAll()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                foreach (IGaugePageItem item in this)
                {
                    await item.BeginCommit();
                }
            }
        }

        public IGaugePageItem FindByPageId(Int64 myPageId)
        {
            IGaugePageItem result = null;

            Task.Run(async () =>
            {
                using (var releaser = await _lock.ReaderLockAsync())
                {
                    result = this.FirstOrDefault<IGaugePageItem>(item => item.PageId == myPageId);
                }
            }).Wait();

            return result;
        }

        public async Task BeginLoad()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                this.Clear();

                foreach (ItemRow row in BuildDBTables.GaugePageTable.Rows)
                {
                    IGaugePageItem gaugePageItem = new GaugePageItem(row);
                    this.Add(gaugePageItem);
                }
            }
        }

        async public Task BeginRemovePage(IGaugePageItem myGaugePageItem)
        {
            if (null != myGaugePageItem)
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    base.Remove(myGaugePageItem);
                }

                await myGaugePageItem.BeginDelete();
            }
        }
    }
}
