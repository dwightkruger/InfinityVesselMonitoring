//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityVesselMonitoringSoftware.Settings.ViewModels;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace InfinityVesselMonitoringSoftware.Settings.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsHomeView : Page
    {
        public SettingsHomeView()
        {
            this.InitializeComponent();
        }

        public SettingsHomeViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
