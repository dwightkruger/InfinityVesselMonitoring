//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    public class GaugeItemCollection : ObservableCollection<IGaugeItem>
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public GaugeItemCollection()
        {
        }

        async public Task<IGaugeItem> BeginAddGauge(IGaugeItem item)
        {
            IGaugeItem newGaugeItem = null;

            // Is this item already in our list?
            newGaugeItem = FindFirstByPageId(item.GaugeId);

            // If not in the list already, add it.
            if (null == newGaugeItem)
            {
                newGaugeItem = item;
                using (var releaser = await _lock.WriterLockAsync())
                {
                    base.Add(newGaugeItem);
                }

                await newGaugeItem.BeginCommit();
            }

            return newGaugeItem;
        }

        public async Task BeginCommitAll()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                foreach (IGaugeItem item in this)
                {
                    await item.BeginCommit();
                }
            }
        }

        public IGaugeItem FindFirstByPageId(Int64 myPageId)
        {
            IGaugeItem result = null;

            Task.Run(async () =>
            {
                using (var releaser = await _lock.ReaderLockAsync())
                {
                    result = this.FirstOrDefault<IGaugeItem>(item => item.PageId == myPageId);
                }
            }).Wait();

            return result;
        }

        public List<IGaugeItem> FindAllByPageId(Int64 myPageId)
        {
            List<IGaugeItem> results = null;

            Task.Run(async () =>
            {
                using (var releaser = await _lock.ReaderLockAsync())
                {
                    IEnumerable<IGaugeItem> query = this.Where((item) => item.PageId == myPageId);

                    if (query.Count<IGaugeItem>() > 0)
                        results = query.ToList<IGaugeItem>();
                    else
                        results = new List<IGaugeItem>();
                }
            }).Wait();

            return results;
        }

        public async Task BeginLoad()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                this.Clear();

                foreach (ItemRow row in BuildDBTables.GaugeTable.Rows)
                {
                    IGaugeItem gaugeItem = new GaugeItem(row);

                    this.Add(gaugeItem);
                }
            }
        }

        async public Task BeginRemovePage(IGaugeItem myGaugeItem)
        {
            if (null != myGaugeItem)
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    base.Remove(myGaugeItem);
                }

                await myGaugeItem.BeginDelete();
            }
        }
    }
}
