//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityVesselMonitoringSoftware;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace VesselMonitoringSuite.Sensors
{
    public class SensorCollection : ObservableCollection<ISensorItem>, INotifyPropertyChanged
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        private Hashtable _hashBySensorId = new Hashtable();
        private Hashtable _hashBySerialNumber = new Hashtable();
        private Timer _sensorObservationFlushTimer;
        private int c_20_seconds = 20 * 1000;
        private int c_2_minutes = 2 * 60 * 1000;


        public SensorCollection()
        {
            _sensorObservationFlushTimer = new Timer(SensorObservationFlushTimerTic, null, Timeout.Infinite, Timeout.Infinite);
            this.EnableSensorObservationFlushTimer();
        }

        /// <summary>
        /// Do not call this function. Call await BeginAdd instead.
        /// </summary>
        /// <param name="sensorItem"></param>
        new public void Add(ISensorItem sensorItem)
        {
            throw new NotImplementedException("Use BeginAdd instead");
        }

        async public Task<ISensorItem> BeginAdd(ISensorItem sensorItem)
        {
            ISensorItem item = null;

            using (var releaser = await _lock.WriterLockAsync())
            {
                item = (ISensorItem)_hashBySerialNumber[sensorItem.SerialNumber];

                // If the item was not found, then we have a new sensor. Add it.
                if (null == item)
                {
                    await sensorItem.BeginCommit();

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
            Task.Run(async () =>
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    _hashBySensorId.Clear();
                    _hashBySerialNumber.Clear();
                    base.Clear();
                }
            });
        }

        async public Task<ISensorItem> BeginFindBySerialNumber(string serialNumber)
        {
            ISensorItem result = null;

            using (var releaser = await _lock.WriterLockAsync())
            {
                result = (ISensorItem)_hashBySerialNumber[serialNumber];
            }

            return result;

        }

        /// <summary>
        /// Loads the sensors from the SQL database table
        /// </summary>
        public void Load()
        {
            try
            {
                foreach (ItemRow row in App.BuildDBTables.SensorTable.Rows)
                {
                    ISensorItem sensorItem = null;
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
                            sensorItem = new SensorItem(row);
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
                            sensorItem = new SensorItem(row);
                        }
                        break;
                    }

                    if (null != sensorItem)
                    {
                        base.Add(sensorItem);
                        _hashBySerialNumber.Add(sensorItem.SerialNumber, sensorItem);
                        _hashBySensorId.Add(sensorItem.SensorId, sensorItem);
                    }
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
        public List<ISensorItem> GetSensorsForDeviceId(Int64 deviceId)
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

        public ISensorItem FindBySensorId(Int64 sensorId)
        {
            ISensorItem result = null;

            lock (_lock)
            {
                result = (ISensorItem)_hashBySensorId[sensorId];
            }

            return result;
        }

        /// <summary>
        /// Empty the collection of sensors and the backing SQL store
        /// </summary>
        async public Task BeginEmpty()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                await App.BuildDBTables.SensorTable.BeginEmpty();
                App.BuildDBTables.SensorTable.Load();
                _hashBySerialNumber.Clear();
                _hashBySensorId.Clear();
                base.Clear();
            }
        }

        /// <summary>
        /// Shutdown sensor data collection and flush all records to the database
        /// </summary>
        /// <returns></returns>
        async public Task BeginShutdown()
        {
            if ((null != App.BuildDBTables) && (null != App.BuildDBTables.SensorDataTable))
            {
                // Shutdown the timer job flushing sensor data
                this.DisableSensorObservationFlushTimer();

                // Write an observation for each of the sensors indicating that it is going offline
                foreach (ISensorItem sensorItem in this)
                {
                    sensorItem.AddOfflineObservation(true);
                }

                await App.BuildDBTables.SensorDataTable.BeginCommitAllAndClear(() =>
                {
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                });

                this.Clear();
            }
        }

        public void EnableSensorObservationFlushTimer()
        {
            _sensorObservationFlushTimer.Change(c_20_seconds, c_2_minutes);
        }

        public void DisableSensorObservationFlushTimer()
        {
            _sensorObservationFlushTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Flush all of the sensor observations to the disk.
        /// </summary>
        /// <param name="stateInfo"></param>
        async private void SensorObservationFlushTimerTic(object stateInfo)
        {
            if (null != App.BuildDBTables.SensorDataTable)
            {
                await App.BuildDBTables.SensorDataTable.BeginCommitAllAndClear(() =>
                {
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                });
            }
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
