//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    public class ArcGaugeLeftViewModel : ObservableObject
    {
        private IGaugeItem _gaugeItem;

        public const string GaugeItemPropertyName = "GaugeItem";

        public ArcGaugeLeftViewModel()
        {
        }

        public IGaugeItem GaugeItem
        {
            get { return _gaugeItem; }
            set
            {
                Set(GaugeItemPropertyName, ref _gaugeItem, value);
            }
        }
    }
}
