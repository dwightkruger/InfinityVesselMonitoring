//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

using GalaSoft.MvvmLight;
using Windows.UI.Xaml;

namespace InfinityGroup.VesselMonitoring.Charting
{
    public class Axis : ObservableObject, IAxis
    {
        private string _title;
        private string _labelFormat;
        private int _labelInterval;
        private Style _labelStyle;
        private Style _lineStyle;
        private Style _majorTicStyle;
        private bool _showLabels;

        public string Title
        {
            get { return _title; }
            set { Set<string>(() => Title, ref _title, value); }
        }
        public string LabelFormat
        {
            get { return _labelFormat; }
            set { Set<string>(() => LabelFormat, ref _labelFormat, value); }
        }

        public int LabelInterval
        {
            get { return _labelInterval; }
            set { Set<int>(() => LabelInterval, ref _labelInterval, value); }
        }
        public Style LabelStyle
        {
            get { return _labelStyle; }
            set { Set<Style>(() => LabelStyle, ref _labelStyle, value); }
        }
        public Style LineStyle
        {
            get { return _lineStyle; }
            set { Set<Style>(() => LineStyle, ref _lineStyle, value); }
        }
        public Style MajorTicStyle
        {
            get { return _majorTicStyle; }
            set { Set <Style>(() => MajorTicStyle, ref _majorTicStyle, value); }
        }
        public bool ShowLabels
        {
            get { return _showLabels; }
            set { Set<bool>(() => ShowLabels, ref _showLabels, value); }
        }
    }
}
