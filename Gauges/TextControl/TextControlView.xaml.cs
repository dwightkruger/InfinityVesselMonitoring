//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityGroup.VesselMonitoring.Gauges.TextControl
{
    public sealed partial class TextControlView : UserControl
    {
        public TextControlView()
        {
            this.InitializeComponent();
        }
        public TextControlViewModel ViewModel
        {
            get { return this.VM; }
            set { this.VM = value; }
        }
    }
}
