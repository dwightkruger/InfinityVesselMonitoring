//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using InfinityGroup.VesselMonitoring.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace InfinityVesselMonitoringSoftware.Editors.GaugePageEditor
{
    public class EditRibbonViewModel : ObservableObject
    {
        private RelayCommand _saveCommand;
        private RelayCommand _revertCommand;
        private RelayCommand _exitCommand;
        private RelayCommand _copyCommand;
        private RelayCommand _pasteCommand;
        private RelayCommand _deleteCommand;
        private RelayCommand _fontSizeIncreaseCommand;
        private RelayCommand _fontSizeDecreaseCommand;
        private RelayCommand _boldFontCommand;
        private RelayCommand _italicsFontCommand;
        private RelayCommand _underlineFontCommand;
        private RelayCommand _leftAlignTextCommand;
        private RelayCommand _rightAlignTextCommand;
        private RelayCommand _centerAlignTextCommand;
        private RelayCommand _undoCommand;
        private RelayCommand _redoCommand;

        private List<IGaugeItem> _gaugeItemList = null;
        private ObservableCollection<IGaugeItem> _gaugeItemSelectedList = null;
        private List<IGaugeItem> _gaugeItemCopyList = null;
        private List<DeletionItem<IGaugeItem>> _gaugeDeletionList = new List<DeletionItem<IGaugeItem>>();

        public EditRibbonViewModel()
        {
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
                            this.Rollback();
                        },
                        () =>
                        {
                            return true;
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

        public ICommand CopyCommand 
        {
            get
            {
                if (_copyCommand == null)
                {
                    _copyCommand = new RelayCommand(
                        () =>
                        {
                            // Make a copy of the list of items selected.
                            if (null != _gaugeItemList)
                            {
                                _gaugeItemCopyList = new List<IGaugeItem>();
                                foreach (IGaugeItem item in this.SelectedGaugeItemList)
                                {
                                    IGaugeItem newItem = item.Copy();
                                    _gaugeItemCopyList.Add(newItem);
                                }
                            }

                            _pasteCommand.RaiseCanExecuteChanged();
                        },
                        () =>
                        {
                            if (null == this.SelectedGaugeItemList) return false;
                            if (0 == this.SelectedGaugeItemList.Count) return false;
                            return true;
                        }
                       );
                }

                return _copyCommand;
            }
        }

        public ICommand PasteCommand
        {
            get
            {
                if (_pasteCommand == null)
                {
                    _pasteCommand = new RelayCommand(
                        () =>
                        {
                            // Move the items a few pixels over.
                            foreach (IGaugeItem item in _gaugeItemCopyList)
                            {
                                item.GaugeLeft += 80;
                                item.GaugeTop += 80;

                                this.GaugeItemList.Add(item.Copy());
                            }

                            // Send the list of gaugeItems to the page to rebuild itself.
                            Messenger.Default.Send<List<IGaugeItem>>(this.GaugeItemList, "BuildGaugeItemList");
                        },
                        () =>
                        {
                            if (null == _gaugeItemCopyList) return false;
                            if (0 == _gaugeItemCopyList.Count) return false;
                            return true;
                        }
                       );
                }

                return _pasteCommand;
            }
        }

        public ICommand DeleteCommand
        {
            get
            {
                if (_deleteCommand == null)
                {
                    _deleteCommand = new RelayCommand(
                        () =>
                        {
                            foreach (IGaugeItem item in _gaugeItemSelectedList)
                            {
                                this.GaugeItemList.Remove(item);
                                _gaugeDeletionList.Add(new DeletionItem<IGaugeItem>(item));
                            }

                            _gaugeItemSelectedList.Clear();

                            // Send the list of gaugeItems to the page to rebuild itself.
                            Messenger.Default.Send<List<IGaugeItem>>(this.GaugeItemList, "BuildGaugeItemList");
                        },
                        () =>
                        {
                            if (null == _gaugeItemSelectedList) return false;
                            if (0 == _gaugeItemSelectedList.Count) return false;
                            return true;
                        }
                       );
                }

                return _deleteCommand;
            }
        }

        public ICommand UndoCommand
        {
            get
            {
                if (_undoCommand == null)
                {
                    _undoCommand = new RelayCommand(
                        () =>
                        {
                            this.UndoLastAction();
                        },
                        () =>
                        {
                            if (null == this.GaugeItemList) return false;
                            if (0 == this.GaugeItemList.Count) return false;
                            return true;
                        }
                       );
                }

                return _undoCommand;
            }
        }

        public ICommand RedoCommand
        {
            get
            {
                if (_redoCommand == null)
                {
                    _redoCommand = new RelayCommand(
                        () =>
                        {
                            IEnumerable<IGaugeItem> query =
                                from item in this.GaugeItemList
                                orderby item.PropertyChangedTime descending
                                select item;

                            if (query.Count<IGaugeItem>() == 0) return;

                            // There may be more then one item modified at this time. We want to undo all of them
                            // with the same time.
                            DateTime lastModifiedTime = query.First<IGaugeItem>().PropertyChangedTime;
                            foreach (IGaugeItem item in query)
                            {
                                if (item.PropertyChangedTime == lastModifiedTime)
                                {
                                    item.RedoCommand.Execute(null);
                                    item.Update();
                                }
                            }
                        },
                        () =>
                        {
                            if (null == this.GaugeItemList) return false;
                            if (0 == this.GaugeItemList.Count) return false;
                            return true;
                        }
                       );
                }

                return _redoCommand;
            }
        }

        /// <summary>
        /// The list of gauges that can be edited.
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
        public ObservableCollection<IGaugeItem> SelectedGaugeItemList
        {
            get { return _gaugeItemSelectedList; }
            set
            {
                Set<ObservableCollection<IGaugeItem>>(() => SelectedGaugeItemList, ref _gaugeItemSelectedList, value);
                RaisePropertyChangedAll();
                _gaugeItemSelectedList.CollectionChanged += GaugeItemSelectedList_CollectionChanged;
            }
        }

        private void GaugeItemSelectedList_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChangedAll();
        }

        private void UndoLastAction()
        {
            // What is the most recent deletion?
            DateTime lastDeletionTime = DateTime.MinValue;
            IEnumerable<DeletionItem<IGaugeItem>> query1 = null;
            if ((null != _gaugeDeletionList) && (0 != _gaugeDeletionList.Count))
            {
                query1 =
                    from item in this._gaugeDeletionList
                    orderby item.DeletionTime descending
                    select item;

                lastDeletionTime = query1.First<DeletionItem<IGaugeItem>>().DeletionTime;
            }

            // What is the most recent property change?
            DateTime lastPropertyChangedTime = DateTime.MinValue;
            IEnumerable<IGaugeItem> query2 = null;
            if ((null != this.GaugeItemList) && (0 != this.GaugeItemList.Count))
            {
                query2 =
                    from item in this.GaugeItemList
                    orderby item.PropertyChangedTime descending
                    select item;

                lastPropertyChangedTime = query2.First<IGaugeItem>().PropertyChangedTime;
            }

            // Undo the most recent action choosing between a deletion and a property change.
            if (lastPropertyChangedTime > lastDeletionTime)
            {
                // There may be more then one item modified at this time. We want to undo all of them
                // with the same time.
                foreach (IGaugeItem item in query2)
                {
                    if (item.PropertyChangedTime >= lastPropertyChangedTime)
                    {
                        item.UndoCommand.Execute(null);
                        item.Update();
                    }
                }
            }
            else if ((lastDeletionTime > lastPropertyChangedTime) &&
                     (lastDeletionTime != DateTime.MinValue))
            {
                // There may be more than one deletion at this time. We want to undo all of them
                // with the same time.

                int i = 0;
                while (i < _gaugeDeletionList.Count)
                {
                    if (_gaugeDeletionList[i].DeletionTime >= lastDeletionTime)
                    {
                        this.GaugeItemList.Add(_gaugeDeletionList[i].Item);
                        _gaugeDeletionList.RemoveAt(i);
                    }
                    else
                    {
                        i++;
                    }
                }

                // Send the list of gaugeItems to the page to rebuild itself.
                Messenger.Default.Send<List<IGaugeItem>>(this.GaugeItemList, "BuildGaugeItemList");
            }
        }

        /// <summary>
        /// Are one or more of the gauge items in the list of the type specified?
        /// </summary>
        /// <param name="gaugeType"></param>
        /// <returns></returns>
        private bool IsGaugeType(GaugeTypeEnum gaugeType)
        {
            if (null == this.SelectedGaugeItemList) return false;
            if (this.SelectedGaugeItemList.Count == 0) return false;

            IEnumerable<IGaugeItem> query = this.SelectedGaugeItemList.Where((item) => item.GaugeType == gaugeType);
            return (query.Count<IGaugeItem>() > 0);
        }


        /// <summary>
        /// Save/commit any dirty gauge items to the sql database
        /// </summary>
        /// <returns></returns>
        async private Task BeginCommit()
        {
            // Execute all of the gauge item deletion requests.
            foreach (DeletionItem<IGaugeItem> item in _gaugeDeletionList)
            {
                await item.Item.BeginDelete();
            }

            if (null == this.GaugeItemList) return;
            if (this.GaugeItemList.Count == 0) return;

            foreach (IGaugeItem gaugeItem in this.GaugeItemList)
            {
                await gaugeItem.BeginCommit();
            }
        }

        private void Rollback()
        {
            // Roll back all of the gauge item deletion reqests
            foreach (DeletionItem<IGaugeItem> item in _gaugeDeletionList)
            {
                this.GaugeItemList.Add(item.Item);
            }

            _gaugeDeletionList.Clear();

            if (null == this.GaugeItemList) return;
            if (this.GaugeItemList.Count == 0) return;

            foreach (IGaugeItem gaugeItem in this.GaugeItemList)
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
            _undoCommand?.RaiseCanExecuteChanged();
            _redoCommand?.RaiseCanExecuteChanged();
            _copyCommand?.RaiseCanExecuteChanged();
            _pasteCommand?.RaiseCanExecuteChanged();
            _deleteCommand?.RaiseCanExecuteChanged();
        }
    }

    public class DeletionItem<T>
    {
        public DeletionItem(T item)
        {
            this.Item = item;
            this.DeletionTime = DateTime.Now.ToUniversalTime();
        }

        public T Item { get; private set; }
        public DateTime DeletionTime { get; private set; }
    }
}
