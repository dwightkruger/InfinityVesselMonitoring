﻿//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Gauges.ArcGaugeLeft
{
    public sealed partial class ArcGaugeLeftView : BaseGaugeView
    {
        public ArcGaugeLeftView()
        {
            this.InitializeComponent();
        }

        public InfinityGroup.VesselMonitoring.Controls.ArcGaugeLeft ArcGaugeLeft { get { return ArcGaugeLeftControl; } }

        public override BaseGaugeViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
