//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Gauges.TankGaugeLeft
{
    public sealed partial class TankGaugeLeftView : UserControl
    {
        public TankGaugeLeftView()
        {
            this.InitializeComponent();
        }

        public TankGaugeLeftViewModel ViewModel
        {
            get { return this.VM; }
            set { this.VM = value; }
        }
    }
}
