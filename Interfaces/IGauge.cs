//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using Windows.UI;
using Windows.UI.Xaml.Media;


namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IGauge 
    {
        IGaugeItem GaugeItem { get; set; }
        Color GaugeLabelsColor { get; set; }
        Color HighAlarmColor { get; set; }
        double HighAlarmValue { get; set; }
        Color HighWarningColor { get; set; }
        double HighWarningValue { get; set; }
        bool IsSensorEnabled { get; set; }
        bool IsHighAlarmEnabled { get; set; }
        bool IsHighWarningEnabled { get; set; }
        bool IsLowAlarmEnabled { get; set; }
        bool IsLowWarningEnabled { get; set; }
        //bool IsNightMode { get; set; }
        bool IsNominalValueEnabled { get; set; }
        bool IsOnline { get; set; }
        Color LowAlarmColor { get; set; }
        double LowAlarmValue { get; set; }
        Color LowWarningColor { get; set; }
        double LowWarningValue { get; set; }
        double MaxValue { get; set; }
        double MinValue { get; set; }
        double NominalValue { get; set; }
        int Resolution { get; set; }
        string Title { get; set; }
        double TitleFontSize { get; set; }
        Color TitleColor { get; set; }
        double Value { get; set; }
        Color ValueColor { get; set; }
        double ValueFontSize { get; set; }
        string Units { get; set; }
        double UnitsFontSize { get; set; }
        double Left { get; set; }
        double Top { get; set; }
    }
}
