//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.IO;
using System.Threading.Tasks;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Globals;
using SQLitePCL;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteVesselDB : IVesselDB
    {
        public SQLiteVesselDB()
        {
            this.Lock = new object();
        }

        async public Task<string> BeginBackup(string myDirectory, Action sucessCallback, Action<Exception> failureCallback)
        {
            string results = string.Empty;

            await Task.Run(() =>
            {
                try
                {
                    FileInfo sourceFileInfo = new FileInfo(this.DatabaseFileName);  // Get the source file directory
                    string sourceDir = sourceFileInfo.DirectoryName;

                    DirectoryInfo sourceDirinfo = new DirectoryInfo(sourceDir);     // Get all of the files in the source directory
                    FileInfo[] sourceFileInfoList = sourceDirinfo.GetFiles();

                    DirectoryInfo destDirInfo = new DirectoryInfo(Path.Combine(sourceDir, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-d-T-hh-mm-ss")));
                    destDirInfo.Create();

                    foreach (FileInfo dbFileInfo in sourceFileInfoList)
                    {
                        string destFileName = Path.Combine(destDirInfo.FullName, dbFileInfo.Name);

                        dbFileInfo.CopyTo(destFileName, true);
                    }

                    results = destDirInfo.FullName;
                    sucessCallback();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex); 
                    failureCallback(ex);
                }
            });

            return results;
        }

        public void Create()
        {
                try
                {
                    this.ItemSet = new ItemSet("VesselDB");

                    this.Connection = new SQLiteConnection(this.DatabaseFileName);

                    // Enable Write-Ahead Logging (see https://www.sqlite.org/wal.html)
                    using (var statement = ((ISQLiteConnection)this.Connection).Prepare("PRAGMA journal_mode = WAL;"))
                    {
                        statement.Step();
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    throw;
                }
        }

        public async Task BeginDropDatabase(Action sucessCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                try
                {
                    FileInfo fileInfo = new FileInfo(this.DatabaseFileName);

                    ISQLiteConnection con = this.Connection as ISQLiteConnection;
                    if (null != con)
                    {
                        con.Dispose();
                        GC.Collect();
                    }

                    fileInfo.Delete();

                    this.Connection = null;

                    sucessCallback();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }
            });
        }

        async public Task BeginDropTable(string myTableName, Action sucessCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(myTableName))
                    {
                        throw new ArgumentException("TableName", "Tablename was not specified");
                    }

                    this.ItemSet.Tables.Remove(myTableName);

                    string str = "DROP TABLE " + myTableName;
                    using (var statement = ((ISQLiteConnection)(this.Connection)).Prepare(str))
                    {
                        statement.Step();
                    }

                    sucessCallback();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }
            });
        }

        async public Task BeginRestore(string myBackupDirectory, bool relocateOnRestore, Action sucessCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                try
                {
                    // Validate input parameters
                    if (string.IsNullOrEmpty(this.DatabaseFileName) ||
                        string.IsNullOrWhiteSpace(this.DatabaseFileName))
                    {
                        throw new NullReferenceException("Target database file is empty.");
                    }

                    // Cloase down our DB connectoin so that the files can be overwritten
                    ISQLiteConnection con = this.Connection as ISQLiteConnection;
                    if (null != con)
                    {
                        con.Dispose();
                        GC.Collect();
                    }

                    // Copy all of the files across
                    DirectoryInfo sourceDirinfo = new DirectoryInfo(myBackupDirectory); // Get all of the files in the source directory
                    FileInfo[] sourceFileInfoList = sourceDirinfo.GetFiles();

                    FileInfo destFileInfo = new FileInfo(this.DatabaseFileName);        // Get the destination directory

                    foreach (FileInfo dbFileInfo in sourceFileInfoList)
                    {
                        string destFileName = Path.Combine(destFileInfo.DirectoryName, dbFileInfo.Name);

                        dbFileInfo.CopyTo(destFileName, true);
                    }

                    sucessCallback();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }
            });
        }

        public void Dispose()
        {
            lock (Lock)
            {
                if (null != Connection)
                {
                    ((SQLiteConnection)Connection).Dispose();
                    Connection = null;
                }
            }
        }

        public object Connection { get; private set; }

        public string ConnectionString { get; set; }

        public string DatabaseFileName { get; set; }

        public ItemSet ItemSet { get; set; }


        public object Lock { get; private set; }
        public double Size()
        {
            return Utilities.GetSize(this.Connection as ISQLiteConnection);
        }
    }

}
