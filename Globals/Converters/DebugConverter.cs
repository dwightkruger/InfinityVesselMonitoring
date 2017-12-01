//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using System;
using System.Diagnostics;
using Windows.UI.Xaml.Data;

namespace InfinityGroup.VesselMonitoring.Globals.Converters
{
    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                Debugger.Break();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (!ViewModelBase.IsInDesignModeStatic)
            {
                Debugger.Break();
            }

            return value;
        }
    }
}
