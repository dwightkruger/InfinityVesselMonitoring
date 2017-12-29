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

namespace InfinityVesselMonitoringSoftware.Gauges
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

        /// <summary>
        /// Do not call this function. Call await BeginAdd instead.
        /// </summary>
        /// <param name="gaugePageItem"></param>
        public new void Add(IGaugePageItem gaugePageItem)
        {
            throw new NotImplementedException("Use BeginAdd");
        }

        async public Task<IGaugePageItem> BeginAdd(IGaugePageItem item)
        {
            IGaugePageItem newGaugePageItem = null;

            using (var releaser = await _lock.WriterLockAsync())
            {
                // Is this item already in our list?
                newGaugePageItem = this.FirstOrDefault<IGaugePageItem>(gaugePage => gaugePage.PageId == item.PageId);

                // If not in the list already, add it.
                if (null == newGaugePageItem)
                {
                    newGaugePageItem = item;
                    base.Add(newGaugePageItem);

                    await newGaugePageItem.BeginCommit();
                }
            }

            return newGaugePageItem;
        }

        async public Task<IGaugePageItem> BeginFindByPageId(Int64 myPageId)
        {
            IGaugePageItem result = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                result = this.FirstOrDefault<IGaugePageItem>(item => item.PageId == myPageId);
            }

            return result;
        }

        public async Task BeginLoad()
        {
            using (var releaser = await _lock.ReaderLockAsync())
            {
                this.Clear();

                foreach (ItemRow row in App.BuildDBTables.GaugePageTable.Rows)
                {
                    IGaugePageItem gaugePageItem = new GaugePageItem(row);
                    base.Add(gaugePageItem);
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

        /// <summary>
        /// Empty the collection of gaugepages and the backing SQL store
        /// </summary>
        async public Task BeginEmpty()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                await App.BuildDBTables.GaugePageTable.BeginEmpty();
                App.BuildDBTables.GaugePageTable.Load();
                base.Clear();
            }
        }
    }
}
