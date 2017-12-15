//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;

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

        private List<IGaugeItem> _gaugeItemList = null;
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
                            Task.Run(async () => 
                            {
                                await this.BeginCommit();
                            }).Wait();
                        },
                        () =>
                        {
                            return this.IsDirty;
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
                            this.Rollback();
                        },
                        () =>
                        {
                            return this.IsDirty;
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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

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
                            if (!this.IsGaugeType(GaugeTypeEnum.TextControl) &&
                                !this.IsGaugeType(GaugeTypeEnum.TextGauge)) return false;

                            return true;
                        }
                       );
                }

                return _centerAlignTextCommand;
            }
        }

        /// <summary>
        /// THe list of gauges that can be edited.
        /// </summary>
        public List<IGaugeItem> GaugeItemList
        {
            get { return _gaugeItemList; }
            set
            {
                Set<List<IGaugeItem>>(() => GaugeItemList, ref _gaugeItemList, value );
                RaisePropertyChangedAll();
            }
        }

        /// <summary>
        /// The current selected gauge
        /// </summary>
        public IGaugeItem SelectedGaugeItem { get; set; }

        /// <summary>
        /// Are one or more of the gauge items in the list of the type specified?
        /// </summary>
        /// <param name="gaugeType"></param>
        /// <returns></returns>
        private bool IsGaugeType(GaugeTypeEnum gaugeType)
        {
            if (null == this.GaugeItemList) return false;
            if (this.GaugeItemList.Count == 0) return false;

            IEnumerable<IGaugeItem> query = _gaugeItemList.Where((item) => item.GaugeType == gaugeType);
            return (query.Count<IGaugeItem>() > 0);
        }

        /// <summary>
        /// Do any of the gauge items have unsaved changes?
        /// </summary>
        private bool IsDirty
        {
            get
            {
                if (null == this.GaugeItemList) return false;
                if (this.GaugeItemList.Count == 0) return false;

                IEnumerable<IGaugeItem> query = _gaugeItemList.Where((item) => item.IsDirty);
                return (query.Count<IGaugeItem>() > 0);
            }
        }

        /// <summary>
        /// Save/commit any dirty gauge items to the sql database
        /// </summary>
        /// <returns></returns>
        async private Task BeginCommit()
        {
            if (null == this.GaugeItemList) return;
            if (this.GaugeItemList.Count == 0) return;

            foreach (IGaugeItem gaugeItem in _gaugeItemList)
            {
                await gaugeItem.BeginCommit();
            }
        }

        private void Rollback()
        {
            if (null == this.GaugeItemList) return;
            if (this.GaugeItemList.Count == 0) return;

            foreach (IGaugeItem gaugeItem in _gaugeItemList)
            {
                gaugeItem.Rollback();
            }

            RaisePropertyChangedAll();
        }

        private void RaisePropertyChangedAll()
        {
            _saveCommand?.RaiseCanExecuteChanged();
            _revertCommand?.RaiseCanExecuteChanged();
            _exitCommand?.RaiseCanExecuteChanged();
            _fontSizeIncreaseCommand?.RaiseCanExecuteChanged();
            _fontSizeDecreaseCommand?.RaiseCanExecuteChanged();
            _boldFontCommand?.RaiseCanExecuteChanged();
            _italicsFontCommand?.RaiseCanExecuteChanged();
            _underlineFontCommand?.RaiseCanExecuteChanged();
            _leftAlignTextCommand?.RaiseCanExecuteChanged();
            _rightAlignTextCommand?.RaiseCanExecuteChanged();
            _centerAlignTextCommand?.RaiseCanExecuteChanged();
        }
    }
}
