//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace InfinityGroup.VesselMonitoring.Controls.Converters
{
    public sealed class BoolToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members

        /// <summary>
        /// Gets or sets the value used when the value being converted is false.
        /// The default value is Visibility.Collapsed.
        /// </summary>
        public Visibility OffVisibility { get; set; }

        /// <summary>
        /// Constructor for BoolToVisibilityConverter.
        /// </summary>
        public BoolToVisibilityConverter()
        {
            OffVisibility = Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                return Visibility.Visible;
            }

            var visibility = Visibility.Visible;

            if (value is bool)
            {
                visibility = (bool)value ? Visibility.Visible : OffVisibility;
            }
            else if (value is Nullable<bool>)
            {
                visibility = ((Nullable<bool>)value).Value ? Visibility.Visible : OffVisibility;
            }
            //else if (value is double)
            //{
            //    visibility = ((double)value > 0.0) ? Visibility.Visible : OffVisibility;
            //}

            return visibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            Visibility valueAsVisibility = (Visibility)value;

            if (valueAsVisibility == Visibility.Visible)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
