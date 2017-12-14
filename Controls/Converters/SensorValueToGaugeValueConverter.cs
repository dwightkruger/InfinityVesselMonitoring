//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using Windows.UI.Xaml.Data;

namespace InfinityGroup.VesselMonitoring.Controls.Converters
{
    public class SensorValueToGaugeValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double fromValue = System.Convert.ToDouble(value);
            Tuple<ISensorItem, IGaugeItem> tuple = (Tuple<ISensorItem, IGaugeItem>)parameter;

            if (tuple.Item2.GaugeType == GaugeTypeEnum.TextControl)
            {
                return fromValue;
            }

            Units fromUnits = tuple.Item1.SensorUnits;
            Units toUnits = tuple.Item2.Units;

            if (fromUnits == toUnits)
            {
                return fromValue;
            }

            UnitItem fromUnitItem = UnitsConverter.Find(fromUnits);
            UnitItem toUnitItem = UnitsConverter.Find(toUnits);

            return UnitsConverter.Convert(fromUnitItem, toUnitItem, fromValue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            double fromValue = System.Convert.ToDouble(value);
            Tuple<ISensorItem, IGaugeItem> tuple = (Tuple<ISensorItem, IGaugeItem>)parameter;

            if (tuple.Item2.GaugeType == GaugeTypeEnum.TextControl)
            {
                return fromValue;
            }

            Units fromUnits = (Units)tuple.Item1.SensorUnits;
            Units toUnits = (Units)tuple.Item2.Units;

            if (fromUnits == toUnits)
            {
                return fromValue;
            }

            UnitItem fromUnitItem = UnitsConverter.Find(fromUnits);
            UnitItem toUnitItem = UnitsConverter.Find(toUnits);

            return UnitsConverter.Convert(toUnitItem, fromUnitItem, fromValue);
        }
    }
}
