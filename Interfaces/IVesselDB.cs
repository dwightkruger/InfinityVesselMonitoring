//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IVesselDB : IDisposable
    {
        Task<string> BeginBackup(string myDirectory, Action sucessCallback, Action<Exception> failureCallback);

        void Create();

        Task BeginDropDatabase(Action sucessCallback, Action<Exception> failureCallback);

        Task BeginDropTable(string myTableName, Action sucessCallback, Action<Exception> failureCallback);

        Task BeginRestore(string directory, bool relocateOnRestore, Action sucessCallback, Action<Exception> failureCallback);

        string ConnectionString { get; set; }

        object Connection { get; }

        string DatabaseFileName { get; set; }

        ItemSet ItemSet { get; }

        object Lock { get; }
        double Size();
    }
}
