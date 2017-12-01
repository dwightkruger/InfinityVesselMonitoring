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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace VesselMonitoringSuite.Sensors
{
    public class SensorCollection : ObservableCollection<ISensorItem>, INotifyPropertyChanged
    {
        public SensorCollection()
        {
            this.Lock = new object();
        }
        /// <summary>
        /// Load all of the sensors from the SQL Db table
        /// </summary>
        public void Load()
        {
            try
            {
                foreach (ItemRow row in BuildDBTables.SensorTable.Rows)
                {
                    ISensorItem sensor = null;
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
                        case SensorType.Unknown: break;
                        case SensorType.VideoCamera: break;
                        case SensorType.Volts: break;
                        case SensorType.Volume: break;
                        case SensorType.VolumeResettable: break;
                        case SensorType.VolumeTotal: break;
                        case SensorType.VolumeTotalResettable: break;
                        case SensorType.Wind: break;
                    }

                    if (null != sensor)
                    {
                        this.Add(sensor);
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
        public List<ISensorItem> GetSensorsForDeviceId(int deviceId)
        {
            List<ISensorItem> results = null;

            lock (Lock)
            {
                IEnumerable<ISensorItem> query = this.Where((sensor) => sensor.DeviceId == deviceId);
                if (query.Count<ISensorItem>() > 0)
                    results = query.ToList<ISensorItem>();
                else
                    results = new List<ISensorItem>();
            }

            return results;
        }

        private object Lock { get; set; }

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
