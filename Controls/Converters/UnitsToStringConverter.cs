//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using InfinityGroup.VesselMonitoring.Utilities;
using System;
using Windows.UI.Xaml.Data;


namespace InfinityGroup.VesselMonitoring.Controls.Converters
{
    public class UnitsToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Units units = (Units) value;
            return UnitItem.ToString(units);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
