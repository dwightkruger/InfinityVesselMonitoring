//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IGaugePageItem
    {
        DateTime ChangeDate { get; }
        Task BeginDelete();
        bool IsDirty { get; }
        bool IsVisible { get; set; }
        string PageName { get; set; }
        Int64 PageId { get; }
        int Position { get; set; }

        Task BeginCommit();
        void Rollback();
    }
}
