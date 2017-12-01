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
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("Description");
            }
            set
            {
                if (value != this.Row.Field<string>("Description"))
                {
                    this.Row.SetField<string>("Description", value);
                    RaisePropertyChanged(() => Description);
                }
            }
        }
        public byte DeviceAddress
        {
            get
            {
                if (null == this.Row) return 0;
                return this.Row.Field<byte>("DeviceAddress");
            }
            set
            {
                if (value != this.Row.Field<byte>("DeviceAddress"))
                {
                    this.Row.SetField<byte>("DeviceAddress", value);
                    RaisePropertyChanged(() => DeviceAddress);
                }
            }
        }
        public int DeviceId
        {
            get
            {
                if (null == this.Row) return -1;
                return this.Row.Field<int>("DeviceId");
            }
            set
            {

                if (value != this.Row.Field<int>("DeviceId"))
                {
                    this.Row.SetField<int>("DeviceId", value);
                    RaisePropertyChanged(() => DeviceId);
                }
            }
        }
        public DeviceType DeviceType
        {
            get
            {
                if (null == this.Row) return DeviceType.Unknown;
                return this.Row.Field<DeviceType>("DeviceType");
            }
            set
            {

                if (value != this.Row.Field<DeviceType>("DeviceType"))
                {
                    this.Row.SetField<DeviceType>("DeviceType", value);
                    RaisePropertyChanged(() => DeviceType);
                }
            }
        }
        public string FirmwareVersion
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("FirmwareVersion");
            }
            set
            {

                if (value != this.Row.Field<string>("FirmwareVersion"))
                {
                    this.Row.SetField<string>("FirmwareVersion", value);
                    RaisePropertyChanged(() => FirmwareVersion);
                }
            }
        }
        public string HardwareVersion
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("HardwareVersion");
            }
            set
            {

                if (value != this.Row.Field<string>("HardwareVersion"))
                {
                    this.Row.SetField<string>("HardwareVersion", value);
                    RaisePropertyChanged(() => HardwareVersion);
                }
            }
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
            set
            {
                if (value != _isOnline)
                {
                    _isOnline = value;
                    RaisePropertyChanged(() => IsOnline);
                }
            }
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
            get
            {
                bool value = false;

                if ((null != this.PropertyBag) && 
                    (!PropertyBag.Get<bool>("IsVirtual", out value)))
                {
                    value = false;
                }

                return false;
            }
            set
            {
                bool curValue;
                if (!PropertyBag.Get<bool>("IsVirtual", out curValue) ||
                    !curValue.Equals(value))
                {
                    PropertyBag.Set<bool>("IsVirtual", value);
                    RaisePropertyChanged(() => IsVirtual);
                }
            }
        }
        public string IPAddress
        {
            get
            {
                string value = string.Empty;

                if ((null != this.PropertyBag) &&
                    (!PropertyBag.Get<string>("IPAddress", out value)))
                {
                    value = string.Empty;
                }

                return value;
            }
            set
            {
                string curValue;
                if (!PropertyBag.Get<string>("IPAddress", out curValue) ||
                    !curValue.Equals(value))
                {
                    PropertyBag.Set<string>("IPAddress", value);
                    RaisePropertyChanged(() => IPAddress);
                }
            }
        }
        public DateTime LastUpdate
        {
            get
            {
                if (null == this.Row) return DateTime.Now.ToUniversalTime();
                return this.Row.Field<DateTime>("LastUpdate");
            }
            set
            {
                if (value != this.Row.Field<DateTime>("LastUpdate"))
                {
                    this.Row.SetField<DateTime>("LastUpdate", value);
                    RaisePropertyChanged(() => LastUpdate);
                }
            }
        }
        public string Location
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("Location");
            }
            set
            {

                if (value != this.Row.Field<string>("Location"))
                {
                    this.Row.SetField<string>("Location", value);
                    RaisePropertyChanged(() => Location);
                }
            }
        }
        public string Model
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("Model");
            }
            set
            {

                if (value != this.Row.Field<string>("Model"))
                {
                    this.Row.SetField<string>("Model", value);
                    RaisePropertyChanged(() => Model);
                }
            }
        }
        public string Name
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("Name");
            }
            set
            {

                if (value != this.Row.Field<string>("Name"))
                {
                    this.Row.SetField<string>("Name", value);
                    RaisePropertyChanged(() => Name);
                }
            }
        }
        public string Manufacturer
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("Manufacturer");
            }
            set
            {

                if (value != this.Row.Field<string>("Manufacturer"))
                {
                    this.Row.SetField<string>("Manufacturer", value);
                    RaisePropertyChanged(() => Manufacturer);
                }
            }
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
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("SerialNumber");
            }
            set
            {

                if (value != this.Row.Field<string>("SerialNumber"))
                {
                    this.Row.SetField<string>("SerialNumber", value);
                    RaisePropertyChanged(() => SerialNumber);
                }
            }
        }
        public string SoftwareVersion
        {
            get
            {
                if (null == this.Row) return string.Empty;
                return this.Row.Field<string>("SoftwareVersion");
            }
            set
            {

                if (value != this.Row.Field<string>("SoftwareVersion"))
                {
                    this.Row.SetField<string>("SoftwareVersion", value);
                    RaisePropertyChanged(() => SoftwareVersion);
                }
            }
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

        protected void LoadPropertyBag()
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
        #endregion
    }
}
