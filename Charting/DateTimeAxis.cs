//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

using System;

namespace InfinityGroup.VesselMonitoring.Charting
{
    public class DateTimeAxis : CartesianAxis, IDateTimeAxis
    {
        private DateTime _maximum;
        private DateTime _minimum;
        private TimeSpan _majorStep;

        public DateTime Maximum
        {
            get { return _maximum; }
            set { Set<DateTime>(() => Maximum, ref _maximum, value); }
        }

        public DateTime Minimum
        {
            get { return _minimum; }
            set { Set<DateTime>(() => Minimum, ref _minimum, value); }
        }

        public TimeSpan MajorStep
        {
            get { return _majorStep; }
            set { Set<TimeSpan>(() => MajorStep, ref _majorStep, value); }
        }
    }
}
