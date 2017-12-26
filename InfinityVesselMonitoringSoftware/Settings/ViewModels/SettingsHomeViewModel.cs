//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.Settings.ViewModels
{
    public class SettingsHomeViewModel : ObservableObject
    {
        private RelayCommand _vesselSettingsCommand;
        private RelayCommand _sensorsCommand;
        private RelayCommand _pagesCommand;
        private RelayCommand _databaseCommand;
        private Nullable<bool> _isVesselSettingsCommandChecked;
        private Nullable<bool> _isSensorsCommandChecked;
        private Nullable<bool> _isPagesCommandChecked;
        private Nullable<bool> _isDatabaseCommandChecked;

        public SettingsHomeViewModel()
        {
        }

        public IVesselSettings VesselSettings
        {
            get { return App.VesselSettings; }
        }

        public ICommand VesselSettingsCommand
        {
            get
            {
                if (_vesselSettingsCommand == null)
                {
                    _vesselSettingsCommand = new RelayCommand(
                        () =>
                        {
                            this.IsVesselSettingsCommandChecked = true;
                            this.IsSensorsCommandChecked = false;
                            this.IsPagesCommandChecked = false;
                            this.IsDatabaseCommandChecked = false;
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _vesselSettingsCommand;
            }
        }

        public ICommand SensorsCommand
        {
            get
            {
                if (_sensorsCommand == null)
                {
                    _sensorsCommand = new RelayCommand(
                        () =>
                        {
                            this.IsVesselSettingsCommandChecked = false;
                            this.IsSensorsCommandChecked = true;
                            this.IsPagesCommandChecked = false;
                            this.IsDatabaseCommandChecked = false;
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _sensorsCommand;
            }
        }

        public ICommand PagesCommand
        {
            get
            {
                if (_pagesCommand == null)
                {
                    _pagesCommand = new RelayCommand(
                        () =>
                        {
                            this.IsVesselSettingsCommandChecked = false;
                            this.IsSensorsCommandChecked = false;
                            this.IsPagesCommandChecked = true;
                            this.IsDatabaseCommandChecked = false;
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _pagesCommand;
            }
        }

        public ICommand DatabaseCommand
        {
            get
            {
                if (_databaseCommand == null)
                {
                    _databaseCommand = new RelayCommand(
                        () =>
                        {
                            this.IsVesselSettingsCommandChecked = false;
                            this.IsSensorsCommandChecked = false;
                            this.IsPagesCommandChecked = false;
                            this.IsDatabaseCommandChecked = true;
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _databaseCommand;
            }
        }

        public Nullable<bool> IsVesselSettingsCommandChecked
        {
            get { return _isVesselSettingsCommandChecked; }
            set { Set<Nullable<bool>>(() => IsVesselSettingsCommandChecked, ref _isVesselSettingsCommandChecked, value); }
        }

        public Nullable<bool> IsSensorsCommandChecked
        {
            get { return _isSensorsCommandChecked; }
            set { Set<Nullable<bool>>(() => IsSensorsCommandChecked, ref _isSensorsCommandChecked, value); }
        }

        public Nullable<bool> IsPagesCommandChecked
        {
            get { return _isPagesCommandChecked; }
            set { Set<Nullable<bool>>(() => IsPagesCommandChecked, ref _isPagesCommandChecked, value); }
        }

        public Nullable<bool> IsDatabaseCommandChecked
        {
            get { return _isDatabaseCommandChecked; }
            set { Set<Nullable<bool>>(() => IsDatabaseCommandChecked, ref _isDatabaseCommandChecked, value); }
        }
    }
}
