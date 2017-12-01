//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace InfinityGroup.VesselMonitoring.Gauges
{
    /// <summary>
    /// This class describes tha attributes of single page containing many gauge. 
    /// </summary>
    public class GaugePageItem : ObservableObject, IGaugePageItem
    {
        private PropertyBag _propertyBag;
        private ItemRow _row;
        private Int64 _pageId;

        /// <summary>
        /// Call this constructor when building a new page from scratch
        /// </summary>
        public GaugePageItem()
        {
            _pageId = -1;
        }

        /// <summary>
        /// Call this constructor when restoring a page from the backing store (SQLite)
        /// </summary>
        /// <param name="gaugePageID"></param>
        public GaugePageItem(Int64 gaugePageID)
        {
            _pageId = gaugePageID;

            if (_pageId != -1)
            {
                this.Load();
            }
        }

        public DateTime ChangeDate
        {
            get
            {
                DateTime? value = Row.Field<DateTime?>("ChangeDate");
                if (value == null) value = DateTime.Now.ToUniversalTime();
                return (DateTime)value;
            }
        }

        async public Task Commit()
        {
            if (this.PropertyBag.IsDirty) Row.SetField<string>("PropertyBag", this.PropertyBag.JsonSerialize());

            // Persist the row into the database
            if (PageId == -1)
            {
                BuildDBTables.GaugePageTable.AddRow(Row);
                await BuildDBTables.GaugePageTable.BeginCommitRow(
                    Row,
                    () =>
                    {
                        _pageId = this.Row.Field<Int64>("PageId");
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
            else if (this.IsDirty)
            {
                await BuildDBTables.GaugePageTable.BeginCommitRow(
                    Row,
                    () =>
                    {
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
        }

        async public Task Delete()
        {
            await BuildDBTables.GaugePageTable.BeginRemove(_row);
        }

        public bool IsDirty
        {
            get
            {
                if (_row == null) return false;                             // We do not have any data
                if (Row.RowState != ItemRowState.Unchanged) return true;    // No changes have been made
                if (_propertyBag == null) return false;                     // We do not have a property blob

                return PropertyBag.IsDirty;
            }
        }

        public bool IsVisible
        {
            get
            {
                bool value = Row.Field<bool>("IsVisible");
                return (bool)value;
            }
            set
            {
                if (Row.Field<bool>("IsVisible") != value)
                {
                    Row.SetField<bool>("IsVisible", value);
                    RaisePropertyChanged(() => IsVisible);
                }
            }
        }

        public string PageName
        {
            get
            {
                string value = Row.Field<string>("PageName");
                return (string)value;
            }
            set
            {
                if (Row.Field<string>("PageName") != value)
                {
                    Row.SetField<string>("PageName", value);
                    RaisePropertyChanged(() => PageName);
                }
            }
        }

        public void NotifyOfPropertyChangeAll()
        {
            foreach (ItemColumn column in BuildDBTables.GaugePageTable.Columns)
            {
                RaisePropertyChanged(column.ColumnName);
            }

            foreach (string propName in PropertyBag.PropertyNames())
            {
                RaisePropertyChanged(propName);
            }
        }

        public void Load()
        {
            _row = BuildDBTables.GaugePageTable.Find(_pageId);

            Debug.Assert(_row != null);

            _pageId = this.PageId;
            LoadPropertyBag();
            NotifyOfPropertyChangeAll();
        }

        protected void LoadPropertyBag()
        {
            this.PropertyBag = new PropertyBag();

            string json = _row.Field<string>("PropertyBag");
            if (!string.IsNullOrEmpty(json))
            {
                this.PropertyBag.JsonDeserialize(json);
            }
        }

        public long PageId
        {
            get
            {
                long value = Row.Field<long>("PageId");
                return value;
            }
        }

        public PropertyBag PropertyBag
        {
            get
            {
                if (_propertyBag == null)
                {
                    _propertyBag = new PropertyBag();
                }
                return _propertyBag;
            }

            protected set
            {
                if (_propertyBag != value)
                {
                    _propertyBag = value;
                    RaisePropertyChanged(() => PropertyBag);
                    Row.SetField<string>("PropertyBag", _propertyBag.JsonSerialize());
                }
            }
        }

        public int Position
        {
            get
            {
                int value = Row.Field<int>("Position");
                return value;
            }
            set
            {
                if (Row.Field<int>("Position") != value)
                {
                    Row.SetField<int>("Position", value);
                    RaisePropertyChanged(() => Position);
                }
            }
        }

        public void Rollback()
        {
            // Rollback all of the changed values.
            if ((_row.RowState == ItemRowState.Modified) || PropertyBag.IsDirty)
            {
                _row.RejectChanges();
                this.LoadPropertyBag();
                NotifyOfPropertyChangeAll();
            }
        }

        private ItemRow Row
        {
            get
            {
                if (_row == null)
                {
                    if (_pageId == -1)
                    {
                        _row = BuildDBTables.GaugePageTable.CreateRow();
                    }
                    else
                    {
                        Load();
                    }
                }

                return _row;
            }
        }
    }
}
