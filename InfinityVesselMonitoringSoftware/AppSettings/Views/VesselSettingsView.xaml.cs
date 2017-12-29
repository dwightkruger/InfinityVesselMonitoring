//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Controls.Converters;
using InfinityVesselMonitoringSoftware.AppSettings.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfinityVesselMonitoringSoftware.AppSettings.Views
{
    public sealed partial class VesselSettingsView : UserControl
    {
        public VesselSettingsView()
        {
            this.InitializeComponent();
            this.Loaded += VesselSettingsView_Loaded;
        }

        private void VesselSettingsView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
        }

        public VesselSettingsViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
