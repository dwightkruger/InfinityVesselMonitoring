//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace InfinityVesselMonitoringSoftware.Editors.GaugePageEditor
{
    public sealed partial class EditRibbonView : UserControl
    {
        public EditRibbonView()
        {
            this.InitializeComponent();
        }

        public EditRibbonViewModel ViewModel
        {
            get { return this.VM; }
        }
    }
}
