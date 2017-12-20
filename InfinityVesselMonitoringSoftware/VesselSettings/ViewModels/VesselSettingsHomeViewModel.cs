//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.VesselSettings.ViewModels
{
    public class VesselSettingsHomeViewModel : ObservableObject
    {
        private RelayCommand _vesselSettingsCommand;
        private RelayCommand _sensorsCommand;
        private RelayCommand _pagesCommand;
        private RelayCommand _databaseCommand;

        public VesselSettingsHomeViewModel()
        {
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
