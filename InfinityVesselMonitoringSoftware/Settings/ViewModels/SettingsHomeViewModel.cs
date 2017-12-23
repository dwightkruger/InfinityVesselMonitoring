//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.Settings.ViewModels
{
    public class SettingsHomeViewModel : ObservableObject
    {
        private RelayCommand _vesselSettingsCommand;
        private RelayCommand _sensorsCommand;
        private RelayCommand _pagesCommand;
        private RelayCommand _databaseCommand;

        public SettingsHomeViewModel()
        {
        }

        public VesselSettingsItem VesselSettings
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

    }
}
