//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.Editors.GaugePageEditor
{
    public class EditRibbonViewModel : ObservableObject
    {
        private RelayCommand _saveCommand;
        private RelayCommand _revertCommand;
        private RelayCommand _exitCommand;
        private RelayCommand _fontSizeIncreaseCommand;
        private RelayCommand _fontSizeDecreaseCommand;
        private RelayCommand _boldFontCommand;
        private RelayCommand _italicsFontCommand;
        private RelayCommand _underlineFontCommand;
        private RelayCommand _leftAlignTextCommand;
        private RelayCommand _rightAlignTextCommand;
        private RelayCommand _centerAlignTextCommand;

        private IGaugeItem _gaugeItem = null;
        private bool _isEditMode = false;

        public bool IsEditMode
        {
            get {  return _isEditMode; }
            set { Set<bool>(() => IsEditMode, ref _isEditMode, value); }
        }

        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(
                        () =>
                        {
                            Task.Run(async () => { await App.GaugeItemCollection.BeginCommitAll(); }).Wait();
                        },
                        () =>
                        {
                            //if (null == this.GaugeItem) return false;
                            //return this.GaugeItem.IsDirty;
                            return true;
                        }
                       );
                }

                return _saveCommand;
            }
        }

        public ICommand RevertCommand
        {
            get
            {
                if (_revertCommand == null)
                {
                    _revertCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            return this.GaugeItem.IsDirty;
                        }
                       );
                }

                return _revertCommand;
            }
        }

        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            return true;
                        }
                       );
                }

                return _exitCommand;
            }
        }

        public ICommand FontSizeIncreaseCommand
        {
            get
            {
                if (_fontSizeIncreaseCommand == null)
                {
                    _fontSizeIncreaseCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _fontSizeIncreaseCommand;
            }
        }

        public ICommand FontSizeDecreaseCommand
        {
            get
            {
                if (_fontSizeDecreaseCommand == null)
                {
                    _fontSizeDecreaseCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _fontSizeDecreaseCommand;
            }
        }

        public ICommand BoldFontCommand
        {
            get
            {
                if (_boldFontCommand == null)
                {
                    _boldFontCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _boldFontCommand;
            }
        }

        public ICommand ItalicsFontCommand
        {
            get
            {
                if (_italicsFontCommand == null)
                {
                    _italicsFontCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _italicsFontCommand;
            }
        }

        public ICommand UnderlineFontCommand
        {
            get
            {
                if (_underlineFontCommand == null)
                {
                    _underlineFontCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _underlineFontCommand;
            }
        }

        public ICommand LeftAlignTextCommand
        {
            get
            {
                if (_leftAlignTextCommand == null)
                {
                    _leftAlignTextCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _leftAlignTextCommand;
            }
        }

        public ICommand RightAlignTextCommand
        {
            get
            {
                if (_rightAlignTextCommand == null)
                {
                    _rightAlignTextCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;
                            return true;
                        }
                       );
                }

                return _rightAlignTextCommand;
            }
        }

        public ICommand CenterAlignTextCommand
        {
            get
            {
                if (_centerAlignTextCommand == null)
                {
                    _centerAlignTextCommand = new RelayCommand(
                        () =>
                        {
                        },
                        () =>
                        {
                            if (null == this.GaugeItem) return false;
                            if (this.GaugeItem.GaugeType != GaugeTypeEnum.TextControl ||
                                this.GaugeItem.GaugeType != GaugeTypeEnum.TextGauge) return false;

                            return true;
                        }
                       );
                }

                return _centerAlignTextCommand;
            }
        }

        public IGaugeItem GaugeItem
        {
            get { return _gaugeItem; }
            set
            {
                Set<IGaugeItem>(() => GaugeItem, ref _gaugeItem, value );

                _saveCommand.RaiseCanExecuteChanged();
                _revertCommand.RaiseCanExecuteChanged();
                _exitCommand.RaiseCanExecuteChanged();
                _fontSizeIncreaseCommand.RaiseCanExecuteChanged();
                _fontSizeDecreaseCommand.RaiseCanExecuteChanged();
                _boldFontCommand.RaiseCanExecuteChanged();
                _italicsFontCommand.RaiseCanExecuteChanged();
                _underlineFontCommand.RaiseCanExecuteChanged();
                _leftAlignTextCommand.RaiseCanExecuteChanged();
                _rightAlignTextCommand.RaiseCanExecuteChanged();
                _centerAlignTextCommand.RaiseCanExecuteChanged();
            }
        }
    }
}
