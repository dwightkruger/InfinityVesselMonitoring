//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using InfinityVesselMonitoringSoftware;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using VesselMonitoring;

namespace VesselMonitoringSuite.Devices
{
    public class DeviceItem : ObservableObject, IDeviceItem
    {
        private PropertyBag _propertyBag;
        private bool _isOnline;

        /// <summary>
        /// Call this constructor if you are building a new device item from scratch
        /// </summary>
        public DeviceItem()
        {
        }

        /// <summary>
        /// Call this constructor is your are restoring a device item from them DB
        /// </summary>
        /// <param name="row"></param>
        public DeviceItem(ItemRow row)
        {
            this.Row = row;
        }

        public DateTime ChangeDate
        {
            get
            {
                DateTime? value = Row.Field<DateTime?>("ChangeDate");
                value = (value == null) ? DateTime.Now.ToUniversalTime() : value;
                return ((DateTime)value);
            }
        }

        public string Description
        {
            get { return GetRowPropertyValue<string>(() => Description); }
            set { SetRowPropertyValue<string>(() => Description, value); }
        }

        public byte DeviceAddress
        {
            get { return GetRowPropertyValue<byte>(() => DeviceAddress); }
            set { SetRowPropertyValue<byte>(() => DeviceAddress, value); }
        }

        public int DeviceId
        {
            get { return GetRowPropertyValue<int>(() => DeviceId); }
            set { SetRowPropertyValue<int>(() => DeviceId, value); }
        }

        public DeviceType DeviceType
        {
            get { return GetRowPropertyValue<DeviceType>(() => DeviceType); }
            set { SetRowPropertyValue<DeviceType>(() => DeviceType, value); }
        }

        public string FirmwareVersion
        {
            get { return GetRowPropertyValue<string>(() => FirmwareVersion); }
            set { SetRowPropertyValue<string>(() => FirmwareVersion, value); }
        }

        public string HardwareVersion
        {
            get { return GetRowPropertyValue<string>(() => HardwareVersion); }
            set { SetRowPropertyValue<string>(() => HardwareVersion, value); }
        }

        public bool IsDirty
        {
            get
            {
                if (this.Row == null) return false;                             // We do not have any data
                if (this.Row.RowState != ItemRowState.Unchanged) return true;   // No changes have been made
                if (_propertyBag == null) return false;                         // We do not have a property bag

                return PropertyBag.IsDirty;
            }
        }

        public bool IsOnline
        {
            get { return _isOnline; }
            set { Set<bool>(() => IsOnline, ref _isOnline, value); }
        }

        public bool IsSwitchDevice
        {
            get
            {
                if (this.TransmitPGNList.Count == 0) return false;
                if (this.TransmitPGNList.Contains(127501)) return true; // Binary switch bank status

                return false;
            }
        }

        public bool IsVirtual
        {
            get { return GetPropertyBagValue<bool>(() => IsVirtual); }
            set { SetPropertyBagValue<bool>(() => IsVirtual, value); }
        }

        public string IPAddress
        {
            get { return GetPropertyBagValue<string>(() => IPAddress); }
            set { SetPropertyBagValue<string>(() => IPAddress, value); }
        }

        public DateTime LastUpdate
        {
            get
            {
                if (null == this.Row) return DateTime.Now.ToUniversalTime();
                return this.Row.Field<DateTime>("LastUpdate");
            }
            set { SetRowPropertyValue<DateTime>(() => LastUpdate, value); }
        }

        public string Location
        {
            get { return GetRowPropertyValue<string>(() => Location); }
            set { SetRowPropertyValue<string>(() => Location, value); }
        }

        public string Model
        {
            get { return GetRowPropertyValue<string>(() => Model); }
            set { SetRowPropertyValue<string>(() => Model, value); }
        }

        public string Name
        {
            get { return GetRowPropertyValue<string>(() => Name); }
            set { SetRowPropertyValue<string>(() => Name, value); }
        }

        public string Manufacturer
        {
            get { return GetRowPropertyValue<string>(() => Manufacturer); }
            set { SetRowPropertyValue<string>(() => Manufacturer, value); }
        }

        public List<uint> ReceivePGNList
        {
            get
            {
                List<UInt32> value = null;
                if (null != this.Row)
                {
                    value = (List<UInt32>)JsonConvert.DeserializeObject<List<UInt32>>(this.Row.Field<string>("ReceivePGNList"));
                }

                return value;
            }
            set
            {
                string json = JsonConvert.SerializeObject(value,
                                Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });

                PropertyBag.Set<string>("ReceivePGNList", json);
                RaisePropertyChanged(() => ReceivePGNList);
            }
        }

        public string SerialNumber
        {
            get { return GetRowPropertyValue<string>(() => SerialNumber); }
            set { SetRowPropertyValue<string>(() => SerialNumber, value); }
        }

        public string SoftwareVersion
        {
            get { return GetRowPropertyValue<string>(() => SoftwareVersion); }
            set { SetRowPropertyValue<string>(() => SoftwareVersion, value); }
        }

        public List<uint> TransmitPGNList
        {
            get
            {
                List<UInt32> value = null;
                if (null != this.Row)
                {
                    value = (List<UInt32>)JsonConvert.DeserializeObject<List<UInt32>>(this.Row.Field<string>("TransmitPGNList"));
                }

                return value;
            }
            set
            {
                string json = JsonConvert.SerializeObject(value,
                                Formatting.Indented,
                                new JsonSerializerSettings()
                                {
                                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                                });

                PropertyBag.Set<string>("TransmitPGNList", json);
                RaisePropertyChanged(() => TransmitPGNList);
                RaisePropertyChanged(() => IsSwitchDevice);
            }
        }

        public void Rollback()
        {
            if ((this.Row.RowState == ItemRowState.Modified) || PropertyBag.IsDirty)
            {
                this.Row.RejectChanges();
                LoadPropertyBag();
                NotifyOfPropertyChangeAll();
            }
        }

        public List<ISensorItem> Sensors
        {
            get
            {
                return App.SensorCollection.GetSensorsForDeviceId(this.DeviceId);
            }
        }

        #region privates

        private T GetRowPropertyValue<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression);

            if (null != this.Row) return this.Row.Field<T>(propertyName);
            return default(T);
        }

        private T GetPropertyBagValue<T>(Expression<Func<T>> propertyExpression)
        {
            T value = default(T);

            var propertyName = GetPropertyName(propertyExpression);

            if ((null != this.PropertyBag) &&
                (!this.PropertyBag.Get<T>(propertyName, out value)))
            {
                value = default(T);
            }

            return value;
        }

        private void LoadPropertyBag()
        {
            byte[] blob = Row.Field<byte[]>("PropertyBag");

            if ((blob != null) && (blob.Length > 0))
            {
                _propertyBag = new PropertyBag();
                _propertyBag.JsonDeserialize(Convert.ToString(blob));
            }
            else
            {
                this.PropertyBag = new PropertyBag();
            }
        }

        private void NotifyOfPropertyChangeAll()
        {
            foreach (ItemColumn column in BuildDBTables.DeviceTable.Columns)
            {
                RaisePropertyChanged(column.ColumnName);
            }

            foreach (string propName in PropertyBag.PropertyNames())
            {
                RaisePropertyChanged(propName);
            }
        }

        private PropertyBag PropertyBag
        {
            get
            {
                if (_propertyBag == null)
                {
                    _propertyBag = new PropertyBag();
                }
                return _propertyBag;
            }

            set
            {
                if (_propertyBag != value)
                {
                    _propertyBag = value;
                    RaisePropertyChanged(() => PropertyBag);
                    Row.SetField<byte[]>("PropertyBag", Encoding.ASCII.GetBytes(_propertyBag.JsonSerialize()));
                }
            }
        }

        private ItemRow Row { get; set; }


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

        private bool SetPropertyBagValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            T curValue;

            var propertyName = GetPropertyName(propertyExpression);

            if (!PropertyBag.Get<T>(propertyName, out curValue) ||
                !curValue.Equals(value))
            {
                PropertyBag.Set<T>(propertyName, value);
                RaisePropertyChanged(propertyExpression);
            }

            return true;
        }

        #endregion
    }
}
