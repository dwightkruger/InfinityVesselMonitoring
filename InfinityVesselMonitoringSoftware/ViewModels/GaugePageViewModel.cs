//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityVesselMonitoringSoftware;
using InfinityVesselMonitoringSoftware.Gauges;
using System.Collections.Generic;

namespace VesselMonitoringSuite.ViewModels
{
    public class GaugePageViewModel : ObservableObject
    {
        private IGaugePageItem _gaugePageItem;
        private List<IGaugeItem> _gaugeItemList;
        private bool _inEditMode = false;

        public GaugePageViewModel()
        {
            _gaugeItemList = new List<IGaugeItem>();
            this.InEditMode = true;
        }

        public IVesselSettings VesselSettings
        {
            get { return App.VesselSettings; }
        }

        public IGaugePageItem GaugePageItem
        {
            get { return _gaugePageItem; }
            set
            {
                if (value == _gaugePageItem) return;
                _gaugePageItem = value;

                RaisePropertyChanged(() => GaugePageItem);
            }
        }

        public bool InEditMode
        {
            get { return _inEditMode; }
            set { Set(() => InEditMode, ref _inEditMode, value); }
        }
        private void BuildGaugeItemList()
        {
            if (null == _gaugePageItem) return;
            if (null == _gaugeItemList) return;

            _gaugeItemList.Clear();

            App.BuildDBTables.GaugeTable.BeginFindByGaugePageId(this.GaugePageItem.PageId, (System.Action<ItemTable>)((itemTable) =>
            {
                foreach (ItemRow row in itemTable.Rows)
                {
                    _gaugeItemList.Add((IGaugeItem)new GaugeItem(row));
                }

                Messenger.Default.Send<List<IGaugeItem>>(_gaugeItemList, "BuildGaugeItemList");
            }),
            (ex) =>
            {
                Telemetry.TrackException(ex);
            });

        }
    }
}
