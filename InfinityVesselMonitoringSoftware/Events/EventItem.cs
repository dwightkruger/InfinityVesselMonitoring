//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;
using System.Linq.Expressions;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using System.Reflection;
using InfinityGroup.VesselMonitoring.Globals;
using System.Threading.Tasks;

namespace InfinityVesselMonitoringSoftware.Events
{
    public class EventItem : ObservableObject, IEventItem
    {
        private object _lock = new object();
        private PropertyBag _propertyBag;
        private int _eventId = 0;

        public EventItem()
        {
        }

        public EventItem(ItemRow row)
        {
            this.Row = row;
        }

        public bool AlarmAcknowledged { get; set; }

        async public Task BeginCommit()
        {
            lock (_lock)
            {
                Row.SetField<string>("PropertyBag", PropertyBag.JsonSerialize());

                if (_eventId <= 0)
                {
                    BuildDBTables.EventsTable.AddRow(Row);
                }
            }

            // Persist the row into the database
            await BuildDBTables.EventsTable.BeginCommitRow(this.Row,
                () =>
                {
                    _eventId = Row.Field<int>("EventId");
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                });
        }

        public EventCode EventCode
        {
            get { return GetRowPropertyValue<EventCode>(() => EventCode); }
            set { SetRowPropertyValue<EventCode>(() => EventCode, value); }
        }

        public DateTime EventDateTimeUTC
        {
            get { return GetRowPropertyValue<DateTime>(() => EventDateTimeUTC); }
            set { SetRowPropertyValue<DateTime>(() => EventDateTimeUTC, value); }
        }

        public long EventId
        {
            get { return GetRowPropertyValue<long>(() => EventId); }
        }

        public int EventPriority
        {
            get { return GetRowPropertyValue<int>(() => EventPriority); }
            set { SetRowPropertyValue<int>(() => EventPriority, value); }
        }

        public bool IsAlarmOn
        {
            get
            {
                return EventCode == EventCode.HighAlarmOn ||
                       EventCode == EventCode.LowAlarmOn ||
                       EventCode == EventCode.SwitchOffTooLongOn ||
                       EventCode == EventCode.SwitchOnTooLongOn;
            }
        }

        public bool IsWarningOn
        {
            get
            {
                return EventCode == EventCode.HighWarningOn ||
                       EventCode == EventCode.LowWarningOn; 
            }
        }

        public double Latitude
        {
            get { return GetRowPropertyValue<double>(() => Latitude); }
            set { SetRowPropertyValue<double>(() => Latitude, value); }
        }

        public double Longitude
        {
            get { return GetRowPropertyValue<double>(() => Longitude); }
            set { SetRowPropertyValue<double>(() => Longitude, value); }
        }

        public void Rollback()
        {
            // Rollback all of the changed values.
            if ((this.Row.RowState == ItemRowState.Modified) || this.PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                this.LoadPropertyBag();
                this.RaisePropertyChangeAll();
            }
        }

        public int SensorId
        {
            get { return GetRowPropertyValue<int>(() => SensorId); }
            set { SetRowPropertyValue<int>(() => SensorId, value); }
        }

        public double Value
        {
            get { return GetRowPropertyValue<double>(() => Value); }
            set { SetRowPropertyValue<double>(() => Value, value); }
        }

        #region private
        private ItemRow Row { get; set; }

        private T GetRowPropertyValue<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (null != this.Row) return this.Row.Field<T>(propertyName);
            return default(T);
        }

        protected void LoadPropertyBag()
        {
            lock (_lock)
            {
                this.PropertyBag = new PropertyBag();

                string blob = this.Row.Field<string>("PropertyBag");
                if ((blob != null) && (blob.Length > 0))
                {
                    this.PropertyBag.JsonDeserialize(blob);
                }
            }
        }

        private PropertyBag PropertyBag
        {
            get
            {
                if (_propertyBag == null)
                {
                    _propertyBag = new PropertyBag();
                    Row.SetField<string>("PropertyBag", _propertyBag.JsonSerialize());
                }

                return _propertyBag;
            }
            set
            {
                if (_propertyBag != value)
                {
                    _propertyBag = value;
                    Row.SetField<string>("PropertyBag", _propertyBag.JsonSerialize());
                    RaisePropertyChanged(() => PropertyBag);
                }
            }
        }

        private void RaisePropertyChangeAll()
        {
            // Raise an INotifyProperty changed on each row in the SQL table
            foreach (ItemColumn column in BuildDBTables.EventsTable.Columns)
            {
                RaisePropertyChanged(column.ColumnName);
            }

            // Raise an INotifyPropertyChanged for each column in the propertyBlob
            foreach (string propName in PropertyBag.PropertyNames())
            {
                // If this property exists in this object, raise the property changed event
                var myType = this.GetType();

                if (!string.IsNullOrEmpty(propName)
                    && myType.GetProperty(propName) != null)
                {
                    RaisePropertyChanged(propName);
                }
            }
        }

        private bool SetRowPropertyValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (value.Equals(this.Row.Field<T>(propertyName)))
            {
                return false;
            }
            else
            {
                this.Row.SetField<T>(propertyName, (T)value);
                RaisePropertyChanged(() => propertyExpression);
            }

            return true;
        }

        #endregion

    }
}
