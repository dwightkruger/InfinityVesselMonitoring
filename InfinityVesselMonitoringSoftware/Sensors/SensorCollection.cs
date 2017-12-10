//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VesselMonitoringSuite.Sensors
{
    public class SensorCollection : ObservableCollection<ISensorItem>, INotifyPropertyChanged
    {
        private object _lock = new object();
        private Hashtable _hashBySensorId = new Hashtable();
        private Hashtable _hashBySerialNumber = new Hashtable();

        public SensorCollection()
        {
        }

        new public ISensorItem Add(ISensorItem sensorItem)
        {
            ISensorItem item = null;

            lock (_lock)
            {
                item = this.FindBySerialNumber(sensorItem.SerialNumber);

                // If the item was not found, then we have a new sensor. Add it.
                if (null == item)
                {
                    Task.Run(async () => { await sensorItem.BeginCommit(); }).Wait();

                    _hashBySerialNumber.Add(sensorItem.SerialNumber, sensorItem);
                    _hashBySensorId.Add(sensorItem.SensorId, sensorItem);
                    base.Add(sensorItem);
                }
                else
                {
                    sensorItem = item;
                }
            }

            return sensorItem;
        }

        new public void Clear()
        {
            lock (_lock)
            {
                _hashBySensorId.Clear();
                _hashBySerialNumber.Clear();
                base.Clear();
            }
        }

        public ISensorItem FindBySerialNumber(string serialNumber)
        {
            ISensorItem result = null;

            lock (_lock)
            {
                result = (ISensorItem)_hashBySerialNumber[serialNumber];
            }

            return result;

        }

        /// <summary>
        /// Loads the sensors from the SQL database table
        /// </summary>
        async public Task BeginLoad()
        {
            try
            {
                foreach (ItemRow row in BuildDBTables.SensorTable.Rows)
                {
                    ISensorItem sensor = null;

                    // Get the last observation for this sensor.
                    await BuildDBTables.SensorDataTable.BeginGetLastDataPoint(row.Field<long>("SensorId"), (lastUpdate, lastValue, lastOnline, bucket) => 
                    {
                        ItemRow sensorValueRow = BuildDBTables.SensorDataTable.CreateRow();
                        sensorValueRow.SetField<DateTime>("Time", lastUpdate);
                        sensorValueRow.SetField<double>("Value", lastValue);
                        sensorValueRow.SetField<bool>("IsOnline", lastOnline);
                        sensorValueRow.SetField<byte>("Bucket", bucket);

                        switch (row.Field<SensorType>("SensorType"))
                        {
                            case SensorType.AC: break;
                            case SensorType.Amps: break;
                            case SensorType.Angle: break;
                            case SensorType.Battery: break;
                            case SensorType.Charge: break;
                            case SensorType.CourseSpeed: break;
                            case SensorType.CurrentDirection: break;
                            case SensorType.CurrentSpeed: break;
                            case SensorType.DC_Amps: break;
                            case SensorType.Depth: break;
                            case SensorType.Distance: break;
                            case SensorType.Flow: break;
                            case SensorType.FlowTotal: break;
                            case SensorType.Frequency: break;
                            case SensorType.FuelEfficiency: break;
                            case SensorType.Heading: break;
                            case SensorType.MagneticVariation: break;
                            case SensorType.Percent: break;
                            case SensorType.Position: break;
                            case SensorType.Power: break;
                            case SensorType.PowerTotal: break;
                            case SensorType.Pressure: break;
                            case SensorType.Rotation: break;
                            case SensorType.Speed: break;
                            case SensorType.String: break;
                            case SensorType.Switch: break;
                            case SensorType.Tank: break;
                            case SensorType.TankTotal: break;
                            case SensorType.Temperature: break;
                            case SensorType.Text: break;
                            case SensorType.Time: break;
                            case SensorType.Unknown:
                            {
                                sensor = new SensorItem(row, sensorValueRow);
                            }
                            break;

                            case SensorType.VideoCamera: break;
                            case SensorType.Volts: break;
                            case SensorType.Volume: break;
                            case SensorType.VolumeResettable: break;
                            case SensorType.VolumeTotal: break;
                            case SensorType.VolumeTotalResettable: break;
                            case SensorType.Wind: break;
                            default:
                            {
                                sensor = new SensorItem(row, sensorValueRow);
                            }
                            break;
                        }

                        if (null != sensor)
                        {
                            this.Add(sensor);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

        /// <summary>
        /// Get all of the sensors attached to the deviceId provided.
        /// </summary>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        public List<ISensorItem> GetSensorsForDeviceId(long deviceId)
        {
            List<ISensorItem> results = null;

            lock (_lock)
            {
                IEnumerable<ISensorItem> query = this.Where((sensor) => sensor.DeviceId == deviceId);
                if (query.Count<ISensorItem>() > 0)
                    results = query.ToList<ISensorItem>();
                else
                    results = new List<ISensorItem>();
            }

            return results;
        }

        public ISensorItem FindBySensorId(long sensorId)
        {
            ISensorItem result = null;

            lock (_lock)
            {
                result = (ISensorItem)_hashBySensorId[sensorId];
            }

            return result;
        }


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
