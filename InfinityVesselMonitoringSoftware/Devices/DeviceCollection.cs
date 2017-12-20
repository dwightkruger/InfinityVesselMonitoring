//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////  

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using InfinityVesselMonitoringSoftware;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.ApplicationModel;

namespace VesselMonitoringSuite.Devices
{
    public class DeviceCollection : ObservableCollection<IDeviceItem>, INotifyPropertyChanged
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        private Hashtable _hashBySerialNumber = new Hashtable();

        public DeviceCollection()
        {
        }

        /// <summary>
        /// Do not call this function. Call await BeginAdd instead.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IGaugePageItem item)
        {
            throw new NotImplementedException("Use BeginAdd");
        }

        async public Task<IDeviceItem> BeginAdd(IDeviceItem deviceItem) 
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                IDeviceItem item = (IDeviceItem)_hashBySerialNumber[deviceItem.SerialNumber];

                // If the item was not found, then we have a new device. Add it.
                if (null == item)
                {
                    await deviceItem.BeginCommit(); 
                    _hashBySerialNumber.Add(deviceItem.SerialNumber, deviceItem);
                    base.Add(deviceItem);
                }
                else
                {
                    deviceItem = item;
                }
            }

            return deviceItem;
        }

        public void BeginClear()
        {
            Task.Run(async () =>
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    _hashBySerialNumber.Clear();
                    base.Clear();
                }
            });
        }

        async public Task<IDeviceItem> BeginFindBySerialNumber(string serialNumber)
        {
            IDeviceItem deviceItem = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                deviceItem = (IDeviceItem)_hashBySerialNumber[serialNumber];
            }

            return deviceItem;
        }

        /// <summary>
        /// Load all of the devices from the device table
        /// </summary>
        async public Task BeginLoad()
        {
            try
            {
                bool isVirtualDeviceFound = false;          // HJave we found the required virtual device
                IDeviceItem device = null;
                using (var releaser = await _lock.ReaderLockAsync())
                {
                    foreach (ItemRow row in App.BuildDBTables.DeviceTable.Rows)
                    {
                        switch (row.Field<DeviceType>("DeviceType"))
                        {
                            case DeviceType.IPCamera:
                                break;

                            case DeviceType.NMEA2000:
                                break;

                            case DeviceType.Virtual:
                                device = new DeviceItem(row);
                                device.IsVirtual = true;
                                device.DeviceType = DeviceType.Virtual;
                                this.Add(device);
                                isVirtualDeviceFound = true;
                                break;

                            default:
                                device = new DeviceItem(row);
                                this.Add(device);
                                break;
                        }
                    }

                    // We need at least one virtual device to contain virtual sensors
                    if (!isVirtualDeviceFound)
                    {
                        device = new DeviceItem();
                        device.IsVirtual = true;
                        device.DeviceType = DeviceType.Virtual;
                        device.SerialNumber = typeof(App).ToString();

                        Package package = Package.Current;
                        PackageId packageId = package.Id;
                        PackageVersion packageVersion = packageId.Version;
                        device.FirmwareVersion = string.Format("{0}.{1}.{2}",
                            packageVersion.Major,
                            packageVersion.Minor,
                            packageVersion.Revision);

                        this.Add(device);
                    }
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        public object Lock { get; set; }

        #region INotifyPropertyChanged

        public const string IS_DIRTY = "IsDirty";
        public const string IGNORE_DIRTY = "IgnoreDirty";
        private bool _isDirty = false;
        private bool _ignoreDirty;          // do we ignore dirty changes? 

        private void BaseModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_isDirty && !e.PropertyName.Equals(IS_DIRTY))
            {
                IsDirty = true;
            }
        }

        public bool IgnoreDirty
        {
            get { return _ignoreDirty; }
            set
            {
                if (!value.Equals(_ignoreDirty))
                {
                    _UpdateIgnore(value);
                    _ignoreDirty = value;
                }
            }
        }


        public bool IsDirty
        {
            get { return _isDirty; }
            set
            {
                if (!_isDirty.Equals(value))
                {
                    _isDirty = value;
                    RaisePropertyChanged(() => IsDirty);
                }
            }
        }

        private void _UpdateIgnore(bool ignore)
        {
            if (ignore)
            {
                PropertyChanged -= BaseModel_PropertyChanged;
            }
            else
            {
                PropertyChanged += BaseModel_PropertyChanged;
            }
        }

        public new event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            if (PropertyChanged == null)
            {
                return;
            }

            var lambda = (LambdaExpression)property;

            MemberExpression memberExpression;

            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = (UnaryExpression)lambda.Body;
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }
            else
            {
                memberExpression = (MemberExpression)lambda.Body;
            }

            RaisePropertyChanged(memberExpression.Member.Name);
        }

        #endregion

    }
}
