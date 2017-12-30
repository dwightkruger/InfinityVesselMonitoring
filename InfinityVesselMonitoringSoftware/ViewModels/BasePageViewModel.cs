//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityVesselMonitoringSoftware.Gauges;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;

namespace InfinityVesselMonitoringSoftware.ViewModels
{
    public class BasePageViewModel : ObservableObject
    {
        private SensorType _sensorType = SensorType.Unknown;
        private bool _inEditMode = false;
        private IGaugePageItem _gaugePageItem;

        public BasePageViewModel()
        {
            this.GaugeItemList = new ObservableCollection<IGaugeItem>();
            App.SensorCollection.CollectionChanged += SensorCollection_CollectionChanged;
        }

        /// <summary>
        /// When the sensor collection changes, a new sensor has appeared or deleted. We need to position it on the page
        /// if it is the type of sensor we are displaying on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void SensorCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        List<IGaugeItem> addedGaugeItems= new List<IGaugeItem>();
                        foreach (ISensorItem sensorItem in e.NewItems)
                        {
                            if (sensorItem.SensorType == this.SensorType)
                            {
                                IGaugeItem gaugeItem = new GaugeItem(this.GaugePageItem.PageId);
                                gaugeItem.TextFontColor = App.VesselSettings.ThemeForegroundColor;
                                gaugeItem.GaugeColor = App.VesselSettings.ThemeForegroundColor;
                                gaugeItem.GaugeType = GaugeTypeEnum.LeftArcGauge;
                                gaugeItem.SensorId = sensorItem.SensorId;
                                gaugeItem.Units = sensorItem.SensorUnits;
                                await gaugeItem.BeginCommit();

                                App.GaugeItemCollection.Add(gaugeItem);
                                addedGaugeItems.Add(gaugeItem);
                                this.GaugeItemList.Add(gaugeItem);
                            }
                        }

                        if (addedGaugeItems.Count > 0)
                        {
                            Messenger.Default.Send<List<IGaugeItem>>(addedGaugeItems, "AddGaugeItemList");
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    {
                        // Find all of the gauge items on this page for the sensors being deleted.
                        List<IGaugeItem> deletedGaugeItems = new List<IGaugeItem>();
                        foreach (ISensorItem sensorItem in e.OldItems)
                        {
                            var gaugeItemQuery =
                                    from gaugeItem in this.GaugeItemList
                                    where gaugeItem.SensorId == sensorItem.SensorId
                                    select gaugeItem;

                            if (gaugeItemQuery.Count<IGaugeItem>() > 0)
                            {
                                deletedGaugeItems.AddRange(gaugeItemQuery.ToList<IGaugeItem>());
                            }
                        }

                        if (deletedGaugeItems.Count<IGaugeItem>() > 0)
                        {
                            Messenger.Default.Send<List<IGaugeItem>>(deletedGaugeItems.ToList<IGaugeItem>(), "RemoveGaugeItemList");
                            deletedGaugeItems.ForEach(item => this.GaugeItemList.Remove(item));
                        }
                    }
                    break;

                case NotifyCollectionChangedAction.Replace:
                break;

                case NotifyCollectionChangedAction.Reset:
                    this.GaugeItemList.Clear();
                break;
            }
        }

        public IVesselSettings VesselSettings
        {
            get { return App.VesselSettings; }
        }

        public bool InEditMode
        {
            get { return _inEditMode; }
            set { Set(() => InEditMode, ref _inEditMode, value); }
        }

        public SensorType SensorType
        {
            get { return _sensorType; }
            set
            {
                Set<SensorType>(() => SensorType, ref _sensorType, value);
                this.BuildGaugeItemList();
            }
        }

        public ObservableCollection<IGaugeItem> GaugeItemList
        {
            get; set;
        }

        public IGaugePageItem GaugePageItem
        {
            get { return _gaugePageItem; }
            set { Set<IGaugePageItem>(() => GaugePageItem, ref _gaugePageItem, value); }
        }

        /// <summary>
        /// Build a list of gauge items associated with the gauge page. We can subsequently update the UI.
        /// </summary>
        private void BuildGaugeItemList()
        {
                this.GaugeItemList.Clear();

                Task.Run(async () => 
                { 
                    this.GaugeItemList = await App.GaugeItemCollection.BeginFindAllBySensorType(this.SensorType);
                }).Wait();
        }
    }
}
