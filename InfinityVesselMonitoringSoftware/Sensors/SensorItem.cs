//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using System.Linq.Expressions;
using System.Reflection;
using InfinityVesselMonitoringSoftware;
using InfinityGroup.VesselMonitoring.SQLiteDB;

namespace VesselMonitoringSuite.Sensors
{
    public class SensorItem : ObservableObject, ISensorItem
    {
        private bool _isOnline = false;
        private double _sensorValue = 0;
        private PropertyBag _propertyBag = null;
        private object _lock = new object();
        private UnitItem _sensorUnits = UnitsConverter.Find(Units.Other);
        private static UnitItem OtherUnit = UnitsConverter.Find(Units.Other);

        public SensorItem()
        {

        }
        public SensorItem(ItemRow row)
        {
            this.Row = row;
        }

        public DateTime ChangeDate => throw new NotImplementedException();

        public string Description
        {
            get { return GetRowPropertyValue<string>(() => Description); }
            set { SetRowPropertyValue<string>(() => Description, value); }
        }

        public int DeviceId
        {
            get { return GetRowPropertyValue<int>(() => DeviceId); }
            set { SetRowPropertyValue<int>(() => DeviceId, value); }
        }

        public string FriendlyName => throw new NotImplementedException();

        public double HighAlarmValue
        {
            get { return GetRowPropertyValue<double>(() => HighAlarmValue); }
            set { SetRowPropertyValue<double>(() => HighAlarmValue, value); }
        }

        public double HighWarningValue
        {
            get { return GetRowPropertyValue<double>(() => HighWarningValue); }
            set { SetRowPropertyValue<double>(() => HighWarningValue, value); }
        }

        public bool IsCalibrated
        {
            get { return GetRowPropertyValue<bool>(() => IsCalibrated); }
            set { SetRowPropertyValue<bool>(() => IsCalibrated, value); }
        }

        public bool IsDirty
        {
            get
            {
                if (this.Row.RowState != ItemRowState.Unchanged) return true;       // No changes have been made
                if (this.PropertyBag == null) return false;                         // We do not have a property bag
                return PropertyBag.IsDirty;
            }
        }

        public bool IsEnabled
        {
            get { return GetRowPropertyValue<bool>(() => IsEnabled); }
            set { SetRowPropertyValue<bool>(() => IsEnabled, value); }
        }

        public bool IsHighAlarmEnabled
        {
            get { return GetRowPropertyValue<bool>(() => IsHighAlarmEnabled); }
            set { SetRowPropertyValue<bool>(() => IsHighAlarmEnabled, value); }
        }

        public bool IsHighWarningEnabled
        {
            get { return GetRowPropertyValue<bool>(() => IsHighWarningEnabled); }
            set { SetRowPropertyValue<bool>(() => IsHighWarningEnabled, value); }
        }

        public bool IsLowAlarmEnabled
        {
            get { return GetRowPropertyValue<bool>(() => IsLowAlarmEnabled); }
            set { SetRowPropertyValue<bool>(() => IsLowAlarmEnabled, value); }
        }

        public bool IsLowWarningEnabled
        {
            get { return GetRowPropertyValue<bool>(() => IsLowWarningEnabled); }
            set { SetRowPropertyValue<bool>(() => IsLowWarningEnabled, value); }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
            set
            {
                Set<bool>(() => IsOnline, ref _isOnline, value);

                if (!_isOnline)
                {
                    this.SensorValue = 0;
                    RaisePropertyChanged(() => SensorValue);
                }
            }
        }

        public bool IsPersisted { get; set; }

        public bool IsVirtual
        {
            get { return GetRowPropertyValue<bool>(() => IsVirtual); }
            set { SetRowPropertyValue<bool>(() => IsVirtual, value); }
        }

        public string Location
        {
            get { return GetRowPropertyValue<string>(() => Location); }
            set { SetRowPropertyValue<string>(() => Location, value); }
        }

        public double LowAlarmValue
        {
            get { return GetRowPropertyValue<double>(() => LowAlarmValue); }
            set { SetRowPropertyValue<double>(() => LowAlarmValue, value); }
        }

        public double LowWarningValue
        {
            get { return GetRowPropertyValue<double>(() => LowWarningValue); }
            set { SetRowPropertyValue<double>(() => LowWarningValue, value); }
        }

        public double MaxValue
        {
            get { return GetRowPropertyValue<double>(() => MaxValue); }
            set { SetRowPropertyValue<double>(() => MaxValue, value); }
        }

        public double MinValue
        {
            get { return GetRowPropertyValue<double>(() => MinValue); }
            set { SetRowPropertyValue<double>(() => MinValue, value); }
        }

        public string Name
        {
            get { return GetRowPropertyValue<string>(() => Name); }
            set { SetRowPropertyValue<string>(() => Name, value); }
        }

        public double NominalValue
        {
            get { return GetRowPropertyValue<double>(() => NominalValue); }
            set { SetRowPropertyValue<double>(() => NominalValue, value); }
        }

        public bool PersistDataPoints
        {
            get { return GetRowPropertyValue<bool>(() => PersistDataPoints); }
            set { SetRowPropertyValue<bool>(() => PersistDataPoints, value); }
        }

        public uint PGN
        {
            get { return GetRowPropertyValue<uint>(() => PGN); }
            set { SetRowPropertyValue<uint>(() => PGN, value); }
        }

        public int PortNumber
        {
            get { return GetRowPropertyValue<int>(() => PortNumber); }
            set { SetRowPropertyValue<int>(() => PortNumber, value); }
        }

        public int Priority
        {
            get { return GetRowPropertyValue<int>(() => Priority); }
            set { SetRowPropertyValue<int>(() => Priority, value); }
        }

        public int Resolution
        {
            get { return GetRowPropertyValue<int>(() => Resolution); }
            set { SetRowPropertyValue<int>(() => Resolution, value); }
        }

        public int SensorId
        {
            get { return GetRowPropertyValue<int>(() => SensorId); }
            set { SetRowPropertyValue<int>(() => SensorId, value); }
        }

        public SensorType SensorType
        {
            get { return GetRowPropertyValue<SensorType>(() => SensorType); }
            set { SetRowPropertyValue<SensorType>(() => SensorType, value); }
        }

        public UnitItem SensorUnits
        {
            get
            {
                if (null == this.Row) return SensorItem.OtherUnit;

                if (_sensorUnits == SensorItem.OtherUnit)
                {
                    Units value = this.Row.Field<Units>("SensorUnits");
                    _sensorUnits = UnitsConverter.Find(value);
                }

                return _sensorUnits;
            }
            set
            {
                if (null == this.Row) return;

                // Verify we have a valid unit for this sensor
                this.ValidateUnits(value);

                if (Row.Field<Units>("SensorUnits") != value.Units)
                {
                    _sensorUnits = value;
                    Row.SetField<Units>("SensorUnits", value.Units);

                    RaisePropertyChanged(() => SensorUnits);
                    RaisePropertyChanged(() => IsDirty);
                }
            }
        }

        public SensorUsage SensorUsage
        {
            get { return GetRowPropertyValue<SensorUsage>(() => SensorUsage); }
            set { SetRowPropertyValue<SensorUsage>(() => SensorUsage, value); }
        }

        public double SensorValue
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic)
                {
                    return (MaxValue - MinValue) * 0.25;
                }

                if (!IsOnline) return 0;

                return _sensorValue;
            }
            set
            {
                Set<double>(() => SensorValue, ref _sensorValue, value);
            }
        }

        public string SerialNumber
        {
            get { return GetRowPropertyValue<string>(() => SerialNumber); }
            set { SetRowPropertyValue<string>(() => SerialNumber, value); }
        }
        
        public bool ShowNominalValue
        {
            get { return GetRowPropertyValue<bool>(() => ShowNominalValue); }
            set { SetRowPropertyValue<bool>(() => ShowNominalValue, value); }
        }
        public TimeSpan Throttle
        {
            get { return GetRowPropertyValue<TimeSpan>(() => Throttle); }
            set { SetRowPropertyValue<TimeSpan>(() => Throttle, value); }
        }

        public DateTime Time => throw new NotImplementedException();

        public void DisableSensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void EmptySensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void EnableSensorDataCache()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            // Rollback all of the changed values.
            if ((this.Row.RowState == ItemRowState.Modified) || this.PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                LoadPropertyBag();

                // We need to rebuild SensorUnits because it is not
                // rebuilt when we reload the DataRow
                this.SensorUnits = UnitsConverter.Find((Units)Row.Field<int>("SensorUnits"));

                RaisePropertyChangeAll();
            }
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
                    RaisePropertyChanged(() => IsDirty);
                }
            }
        }

        private void RaisePropertyChangeAll()
        {
            // Raise an INotifyProperty changed on each row in the SQL table
            foreach (ItemColumn column in BuildDBTables.SensorTable.Columns)
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

            base.RaisePropertyChanged(() => SensorValue);
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

        private void ValidateUnits(UnitItem myUnit)
        {
            // BUGBUG write this code
        }

        #endregion
    }
}
