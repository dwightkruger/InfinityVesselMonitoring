/////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InfinityVesselMonitoringSoftware.Gauges
{
    public class GaugeItemCollection : ObservableCollection<IGaugeItem>
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();

        public GaugeItemCollection()
        {
        }

        /// <summary>
        /// Do not call this function. Call await BeginAdd instead.
        /// </summary>
        /// <param name="sensorItem"></param>
        public void Add(ISensorItem sensorItem)
        {
            throw new NotImplementedException("Use BeginAdd instead");
        }


        async public Task<IGaugeItem> BeginAdd(IGaugeItem item)
        {
            IGaugeItem newGaugeItem = null;

            using (var releaser = await _lock.WriterLockAsync())
            { 
                // Is this item already in our list?
                newGaugeItem = this.FirstOrDefault<IGaugeItem>(gaugeItem => gaugeItem.GaugeId == item.GaugeId);

                // If not in the list already, add it.
                if (null == newGaugeItem)
                {
                    newGaugeItem = item;
                    base.Add(newGaugeItem);

                    await newGaugeItem.BeginCommit();
                }
            }

            return newGaugeItem;
        }

        public async Task BeginCommitAll()
        {
            using (var releaser = await _lock.ReaderLockAsync())
            {
                foreach (IGaugeItem item in this)
                {
                    await item.BeginCommit();
                }
            }
        }

        async public Task<IGaugeItem> BeginFindFirstByPageId(Int64 myPageId)
        {
            IGaugeItem result = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                result = this.FirstOrDefault<IGaugeItem>(item => item.PageId == myPageId);
            }

            return result;
        }

        async public Task<List<IGaugeItem>> BeginFindAllByPageId(Int64 myPageId)
        {
            List<IGaugeItem> results = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                IEnumerable<IGaugeItem> query = this.Where((item) => item.PageId == myPageId);

                if (query.Count<IGaugeItem>() > 0)
                    results = query.ToList<IGaugeItem>();
                else
                    results = new List<IGaugeItem>();
            }

            return results;
        }

        async public Task<ObservableCollection<IGaugeItem>> BeginFindAllBySensorType(SensorType sensorType)
        {
            ObservableCollection<IGaugeItem> results = new ObservableCollection<IGaugeItem>();

            using (var releaser = await _lock.ReaderLockAsync())
            {
                var query = 
                    from gaugeItem in this 
                    join sensor in App.SensorCollection on gaugeItem.SensorId equals sensor.SensorId
                    where sensor.SensorType == sensorType
                    select gaugeItem;

                if (query.Count<IGaugeItem>() > 0)
                {
                    query.ToList<IGaugeItem>().ForEach(item => results.Add(item));
                }
            }

            return results;
        }

        async public Task<IGaugeItem> BeginFindFirstBySensorType(SensorType sensorType)
        {
            IGaugeItem results = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                var query =
                    from gaugeItem in this
                    join sensor in App.SensorCollection on gaugeItem.SensorId equals sensor.SensorId
                    where sensor.SensorType == sensorType
                    select gaugeItem;

                if (query.Count<IGaugeItem>() > 0)
                    results = query.FirstOrDefault<IGaugeItem>();
            }

            return results;
        }

        async public Task<List<IGaugeItem>> BeginFindAllBySensorUsageList(List<SensorUsage> sensorUsageList)
        {
            List<IGaugeItem> results = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                var query =
                    from gaugeItem in this
                    join sensor in App.SensorCollection on gaugeItem.SensorId equals sensor.SensorId
                    where sensorUsageList.Contains(sensor.SensorUsage)
                    select gaugeItem;

                if (query.Count<IGaugeItem>() > 0)
                    results = query.ToList<IGaugeItem>();
                else
                    results = new List<IGaugeItem>();
            }

            return results;
        }

        async public Task<IGaugeItem> BeginFindFirstBySensorUsageList(List<SensorUsage> sensorUsageList)
        {
            IGaugeItem results = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                var query =
                    from gaugeItem in this
                    join sensor in App.SensorCollection on gaugeItem.SensorId equals sensor.SensorId
                    where sensorUsageList.Contains(sensor.SensorUsage)
                    select gaugeItem;

                if (query.Count<IGaugeItem>() > 0)
                    results = query.FirstOrDefault<IGaugeItem>();
            }

            return results;
        }

        public async Task BeginLoad()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                this.Clear();

                foreach (ItemRow row in App.BuildDBTables.GaugeTable.Rows)
                {
                    IGaugeItem gaugeItem = new GaugeItem(row);

                    gaugeItem.TextFontColor = App.VesselSettings.ThemeForegroundColor;
                    gaugeItem.GaugeColor = App.VesselSettings.ThemeForegroundColor;

                    this.Add(gaugeItem);
                }
            }
        }

        async public Task BeginRemoveGaugeItem(IGaugeItem myGaugeItem)
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

        /// <summary>
        /// Empty the collection of gauge items and the backing SQL store
        /// </summary>
        async public Task BeginEmpty()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                await App.BuildDBTables.GaugeTable.BeginEmpty();
                App.BuildDBTables.GaugeTable.Load();
                base.Clear();
            }
        }
    }
}
