//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

namespace InfinityGroup.VesselMonitoring.Charting
{
    public enum RangeExtended
    {
        None = 0,
        Both = 1,
        Positive = 2,
        Negative = 3,
    }
    public class RangePair
    {
        public RangePair(double min, double max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }
        double Maximum { get; set; }
        double Minimum { get; set; }
    }

    public interface ILineAxis
    {
        RangePair ActualRange { get; set; }
        int DesiredTicCount { get; set; }
        RangeExtended RangeExtendedDirection { get; set; }
    }
}
