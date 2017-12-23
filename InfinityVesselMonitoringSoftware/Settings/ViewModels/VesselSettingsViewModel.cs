//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;

namespace InfinityVesselMonitoringSoftware.Settings.ViewModels
{
    public class VesselSettingsViewModel : ObservableObject
    {
        public VesselSettingsViewModel()
        {
        }

        public IVesselSettings VesselSettings
        {
            get { return App.VesselSettings; }
        }
    }
}
