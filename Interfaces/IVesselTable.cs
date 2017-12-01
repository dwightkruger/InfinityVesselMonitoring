//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Threading.Tasks;
using InfinityGroup.VesselMonitoring.Types;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IVesselTable
    {
        void CreateTable();
        Task BeginCommitRow(ItemRow row, Action sucessCallback, Action<Exception> failureCallback);
        Task BeginCommitAll(Action sucessCallback, Action<Exception> failureCallback);
        Task BeginCommitAllAndClear(Action sucessCallback, Action<Exception> failureCallback);
        Task BeginEmpty();
        void Load();
        Task BeginRemove(Int64 id);
        Task BeginRemove(ItemRow row);

        void AddRow(ItemRow row);
        ItemColumnCollection Columns { get; }
        ItemTable Copy();
        int Count { get; }
        ItemRow CreateRow();
        ItemRow CreateRow(ItemTable itemTable);
        ItemTable ItemTable { get; }
        ItemRow Find(Int64 id);
        bool IsReadOnly { get; set; }
        string PrimaryKeyName { get; }
        ItemRowCollection Rows { get; }
        string TableName { get; }
    }
}
