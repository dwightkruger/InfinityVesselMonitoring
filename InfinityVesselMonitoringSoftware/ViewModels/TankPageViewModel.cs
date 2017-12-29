//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;
using Microsoft.Toolkit.Uwp.UI;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InfinityVesselMonitoringSoftware.ViewModels
{
    public class TankPageViewModel : ObservableObject
    {
        private SensorType _sensorType;
        private bool _inEditMode = false;

        public TankPageViewModel()
        {
            this.GaugeItemList = new AdvancedCollectionView();
            this.SensorType = SensorType.Unknown;
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

        public AdvancedCollectionView GaugeItemList
        {
            get; set;
        }

        /// <summary>
        /// Build a list of gauge items associated with the gauge page. We can subsequently update the UI.
        /// </summary>
        private void BuildGaugeItemList()
        {
            // Defer sending the INotifyPropertyChanged until we have finished buiding the list.
            using (this.GaugeItemList.DeferRefresh())
            {
                this.GaugeItemList.Clear();

                List<IGaugeItem> gaugeItemList = null;
                Task.Run(async () => 
                { 
                    gaugeItemList = await App.GaugeItemCollection.BeginFindAllBySensorType(this.SensorType);
                }).Wait();

                this.GaugeItemList.Add(gaugeItemList);
            }
        }
    }
}
