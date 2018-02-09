//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

using System;

namespace InfinityGroup.VesselMonitoring.Charting
{
    public class NumericalAxis : CartesianAxis, INumericalAxis
    {
        private double _lowAlarmValue;
        private double _lowWarningValue;
        private double _highAlarmValue;
        private double _highWarningValue;
        private double _maximum;
        private double _minimum;
        private double _majorStep;

        public double LowAlarmValue
        {
            get { return _lowAlarmValue; }
            set { Set<double>(() => LowAlarmValue, ref _lowAlarmValue, value); }
        }

        public double LowWarningValue
        {
            get { return _lowWarningValue; }
            set { Set<double>(() => LowWarningValue, ref _lowWarningValue, value); }
        }

        public double HighAlarmValue
        {
            get { return _highAlarmValue; }
            set { Set<double>(() => HighAlarmValue, ref _highAlarmValue, value); }
        }

        public double HighWarningValue
        {
            get { return _highWarningValue; }
            set { Set<double>(() => HighWarningValue, ref _highWarningValue, value); }
        }

        public double Maximum
        {
            get { return _maximum; }
            set { Set<double>(() => Maximum, ref _maximum, value); }
        }

        public double Minimum
        {
            get { return _minimum; }
            set { Set<double>(() => Minimum, ref _minimum, value); }
        }

        public double MajorStep
        {
            get { return _majorStep; }
            set { Set<double>(() => MajorStep, ref _majorStep, value); }
        }
    }
}
