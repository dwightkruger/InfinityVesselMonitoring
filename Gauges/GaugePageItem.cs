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

        /// <summary>
        /// Call this constructor when building a new page from scratch
        /// </summary>
        public GaugePageItem()
        {
            // Persist the sensor, and write the first record as an offline observation
            this.Row = BuildDBTables.GaugePageTable.CreateRow();
            BuildDBTables.GaugePageTable.AddRow(this.Row);
            Task.Run(async () =>
            {
                await this.BeginCommit();
            }).Wait();
        }

        /// <summary>
        /// Call this constructor when restoring a page from the backing store (SQLite)
        /// </summary>
        /// <param name="gaugePageID"></param>
        public GaugePageItem(ItemRow row)
        {
            this.Row = row;
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

        async public Task BeginCommit()
        {
            if (this.IsDirty)
            {
                if (this.PropertyBag.IsDirty)
                {
                    this.Row.SetField<string>("PropertyBag", this.PropertyBag.JsonSerialize());
                }

                await BuildDBTables.GaugePageTable.BeginCommitRow(
                    this.Row,
                    () =>
                    {
                        Debug.Assert(this.Row.Field<long>("PageId") > 0);
                    },
                    (Exception ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
        }

        async public Task BeginDelete()
        {
            await BuildDBTables.GaugePageTable.BeginRemove(this.Row);
            this.Row = null;
        }

        public bool IsDirty
        {
            get
            {
                if (this.Row == null) return false;                             // We do not have any data
                if (this.Row.RowState != ItemRowState.Unchanged) return true;    // No changes have been made
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

        protected void LoadPropertyBag()
        {
            this.PropertyBag = new PropertyBag();

            string json = this.Row.Field<string>("PropertyBag");
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
            if ((this.Row.RowState == ItemRowState.Modified) || PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                this.LoadPropertyBag();
                NotifyOfPropertyChangeAll();
            }
        }
        private ItemRow Row { get; set; }
    }
}
