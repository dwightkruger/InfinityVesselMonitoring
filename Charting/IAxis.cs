//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Windows.UI.Xaml;

namespace InfinityGroup.VesselMonitoring.Charting
{
    public interface IAxis
    {
        string Title { get; set; }
        string LabelFormat { get; set; }
        int LabelInterval { get; set; }
        Style LabelStyle { get; set; }
        Style LineStyle { get; set; }
        Style MajorTicStyle { get; set; }
        bool ShowLabels { get; set; }
    }
}
