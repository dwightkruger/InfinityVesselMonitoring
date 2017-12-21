//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityVesselMonitoringSoftware.Settings.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfinityVesselMonitoringSoftware.Settings.Views
{
    public sealed partial class VesselSettingsPage : Page
    {
        public VesselSettingsPage()
        {
            this.InitializeComponent();
        }

        public VesselSettingsViewModel ViewModel
        {
            get { return this.VM; }
        }

    }
}
