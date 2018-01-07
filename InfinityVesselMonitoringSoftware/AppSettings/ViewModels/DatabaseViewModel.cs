//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Threading;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.AppSettings.ViewModels
{
    public class DatabaseViewModel : ObservableObject
    {
        private RelayCommand _databaseChangeMaxSizeCommand;
        private RelayCommand _backupDatabaseCommand;
        private RelayCommand _restoreDatabaseCommand;
        private RelayCommand _moveDatabaseCommand;

        private Timer _databaseMetadataTimer;
        private double _databaseSize;
        private int _totalSensors;
        private int _totalDevices;
        private int _totalAISTargets;
        private int _newDatabaseSize;
        private string _databaseBackupDirectory;
        private string _databaseRestoreDirectory;

        private int c_20_seconds = 20 * 1000;
        private int c_2_minutes = 2 * 60 * 1000;

        public DatabaseViewModel()
        {
            _databaseMetadataTimer = new Timer(DatabaseMetadataTimerTic, null, Timeout.Infinite, Timeout.Infinite);
            this.DatabaseBackupDirectory = App.BuildDBTables.VesselDB.DatabaseFileName;

            DatabaseMetadataTimerTic(null);
        }

        public double DatabaseSize
        {
            get { return _databaseSize; }
            set { Set<double>(() => DatabaseSize, ref _databaseSize, value); }
        }

        public string DatabaseBackupDirectory
        {
            get { return _databaseBackupDirectory; }
            set { Set<string>(() => DatabaseBackupDirectory, ref _databaseBackupDirectory, value); }
        }

        public string DatabaseRestoreDirectory
        {
            get { return _databaseRestoreDirectory; }
            set { Set<string>(() => DatabaseRestoreDirectory, ref _databaseRestoreDirectory, value); }
        }

        public void DisableDatabaseMetadataTimer()
        {
            _databaseMetadataTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void EnableDatabaseMetadataTimer()
        {
            _databaseMetadataTimer.Change(c_20_seconds, c_2_minutes);
        }

        public int NewDatabaseSize
        {
            get { return _newDatabaseSize; }
            set { Set<int>(() => NewDatabaseSize, ref _newDatabaseSize, value); }
        }

        public int TotalAISTargets
        {
            get { return _totalAISTargets; }
            set { Set<int>(() => TotalAISTargets, ref _totalAISTargets, value); }
        }

        public int TotalDevices
        {
            get { return _totalDevices; }
            set { Set<int>(() => TotalDevices, ref _totalDevices, value); }
        }
        public int TotalSensors
        {
            get { return _totalSensors; }
            set { Set<int>(() => TotalSensors, ref _totalSensors, value); }
        }

        public ICommand DatabaseChangeMaxSizeCommand
        {
            get
            {
                if (_databaseChangeMaxSizeCommand == null)
                {
                    _databaseChangeMaxSizeCommand = new RelayCommand(
                        () =>
                        {
                            App.BuildDBTables.SensorDataTable.BeginTruncate(this.NewDatabaseSize);
                        },
                        () =>
                        {
                            return this.DatabaseSize > this.NewDatabaseSize;
                        }
                       );
                }

                return _databaseChangeMaxSizeCommand;
            }
        }

        public ICommand BackupDatabaseCommand
        {
            get
            {
                if (_backupDatabaseCommand == null)
                {
                    _backupDatabaseCommand = new RelayCommand(
                        () =>
                        {
                            App.BuildDBTables.VesselDB.BeginBackup(this.DatabaseBackupDirectory, () => 
                            {
                                // BUGBUG Displace some success message
                            }, 
                            (ex)=> 
                            {
                                // BUGBUG Display some error message
                                Telemetry.TrackException(ex);
                            });
                        },
                        () =>
                        {
                            return !string.IsNullOrEmpty(this.DatabaseBackupDirectory);
                        }
                       );
                }

                return _backupDatabaseCommand;
            }
        }

        public ICommand RestoreDatabaseCommand
        {
            get
            {
                if (_restoreDatabaseCommand == null)
                {
                    _restoreDatabaseCommand = new RelayCommand(
                        () =>
                        {
                            App.SensorCollection.DisableSensorObservationFlushTimer();
                            App.SensorCollection.EnableSensorObservationFlushTimer();
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _restoreDatabaseCommand;
            }
        }

        public ICommand MoveDatabaseCommand
        {
            get
            {
                if (_moveDatabaseCommand == null)
                {
                    _moveDatabaseCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _moveDatabaseCommand;
            }
        }

        async private void DatabaseMetadataTimerTic(object stateInfo)
        {
            try
            {
                if (null != App.BuildDBTables.VesselDB)    this.DatabaseSize    = App.BuildDBTables.VesselDB.Size(); 
                if (null != App.BuildDBTables.SensorTable) this.TotalSensors    = await App.BuildDBTables.SensorTable.TotalSensors();
                if (null != App.BuildDBTables.DeviceTable) this.TotalSensors    = await App.BuildDBTables.DeviceTable.TotalDevices();
                if (null != App.BuildDBTables.AISTable)    this.TotalAISTargets = await App.BuildDBTables.AISTable.TotalAISTargets();
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

    }
}
