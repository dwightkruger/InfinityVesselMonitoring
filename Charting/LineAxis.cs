//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2018 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

namespace InfinityGroup.VesselMonitoring.Charting
{
    public class LineAxis : Axis, ILineAxis
    {
        private RangePair _actualRange;
        private int _desiredTicCount;
        private RangeExtended _rangeExtendedDirection;

        public RangePair ActualRange
        {
            get { return _actualRange; }
            set { Set<RangePair>(() => ActualRange, ref _actualRange, value); }
        }
        public int DesiredTicCount
        {
            get { return _desiredTicCount; }
            set { Set<int>(() => DesiredTicCount, ref _desiredTicCount, value); }
        }
        public RangeExtended RangeExtendedDirection
        {
            get { return _rangeExtendedDirection; }
            set { Set<RangeExtended>(() => RangeExtendedDirection, ref _rangeExtendedDirection, value); }
        }
    }
}
