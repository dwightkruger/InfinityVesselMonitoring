//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using Windows.UI.Xaml.Media;


namespace InfinityGroup.VesselMonitoring.Controls
{
    public interface IGauge
    {
        Brush GaugeLabelsBrush { get; set; }
        SolidColorBrush HighAlarmBrush { get; set; }
        double HighAlarmValue { get; set; }
        SolidColorBrush HighWarningBrush { get; set; }
        double HighWarningValue { get; set; }
        bool IsSensorEnabled { get; set; }
        bool IsHighAlarmEnabled { get; set; }
        bool IsHighWarningEnabled { get; set; }
        bool IsLowAlarmEnabled { get; set; }
        bool IsLowWarningEnabled { get; set; }
        //bool IsNightMode { get; set; }
        bool IsNominalValueEnabled { get; set; }
        bool IsOnline { get; set; }
        SolidColorBrush LowAlarmBrush { get; set; }
        double LowAlarmValue { get; set; }
        SolidColorBrush LowWarningBrush { get; set; }
        double LowWarningValue { get; set; }
        double MaxValue { get; set; }
        double MinValue { get; set; }
        double NominalValue { get; set; }
        int Resolution { get; set; }
        string Title { get; set; }
        double TitleFontSize { get; set; }
        Brush TitleBrush { get; set; }
        double Value { get; set; }
        Brush ValueBrush { get; set; }
        double ValueFontSize { get; set; }
        string Units { get; set; }
        double UnitsFontSize { get; set; }
        double Left { get; set; }
        double Top { get; set; }
    }
}
