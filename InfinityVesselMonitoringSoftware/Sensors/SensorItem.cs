//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

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
        private bool _demoMode = false;
        private Timer _valueTimer;
        private Random randu = new Random();
        private ItemRow _sensorValueRow;
        private DateTime _lastDBWriteTime = DateTime.MinValue;
        private SensorValueBucket _sensorValueBucket = new SensorValueBucket();

        /// <summary>
        /// Call this constructor if your building a sensor from scratch.
        /// </summary>
        public SensorItem(long deviceId)
        {
            // Persist the sensor, and write the first record as an offline observation
            this.Row = BuildDBTables.SensorTable.CreateRow();
            BuildDBTables.SensorTable.AddRow(this.Row);
            this.DeviceId = deviceId;

            Task.Run(async () =>
            {
                await this.BeginCommit();
            }).Wait();

            this.AddOfflineObservation(true);
        }

        /// <summary>
        /// Call this constructor if you are restoring a sensor from the database.
        /// </summary>
        /// <param name="row"></param>
        public SensorItem(ItemRow row)
        {
            this.Row = row;

            // If the last observation for this sensor was not offline, then we did not shutdown cleanly.
            // Add an offline observation
            Task.Run(async () =>
            {
                await BuildDBTables.SensorDataTable.BeginGetLastDataPoint(row.Field<long>("SensorId"),
                    (lastUpdate, lastValue, lastOnline, bucket) =>
                    {
                        AddOfflineObservation(lastUpdate.AddSeconds(2), true);
                    });
            }).Wait();

            // Add an off-line observation at the start. Every sensor starts offline
            this.AddOfflineObservation(true);
        }

        /// <summary>
        /// Write an offline observation at the current dateTime
        /// </summary>
        /// <param name="forceFlush"></param>
        /// <returns></returns>
        public void AddOfflineObservation(bool forceFlush)
        {
            DateTime nowUTC = DateTime.Now.ToUniversalTime();
            this.AddOfflineObservation(nowUTC, forceFlush);
        }

        /// <summary>
        /// Write an offline observation at the dateTime provided
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <param name="forceFlush"></param>
        /// <returns></returns>
        public void AddOfflineObservation(DateTime timeUTC, bool forceFlush)
        {
            this.AddSensorValue(timeUTC, 0, false, forceFlush);
        }

        /// <summary>
        /// Update the sensor value with the value provided. 
        /// </summary>
        /// <param name="sensorValue"></param>
        /// <param name="isOnline"></param>
        /// <param name="forceFlush"></param>
        /// <returns></returns>
        public void AddSensorValue(double sensorValue, bool isOnline, bool forceFlush)
        {
            DateTime nowUTC = DateTime.Now.ToUniversalTime();
            AddSensorValue(nowUTC, sensorValue, isOnline, forceFlush);
        }

        /// <summary>
        /// Update the sensor value at the UTC time specified with the value provided.
        /// </summary>
        /// <param name="timeUTC"></param>
        /// <param name="sensorValue"></param>
        /// <param name="isOnline"></param>
        /// <param name="forceFlush"></param>
        /// <returns></returns>
        public void AddSensorValue(DateTime timeUTC, double sensorValue, bool isOnline, bool forceFlush)
        {
            Debug.Assert(this.SensorId > 0);

            // Round off the sensor value to the appropriate number of significant digits and limit it between
            // minValue and MaxValue.
            double value = sensorValue;
            value = Math.Max(this.MinValue, value);
            value = Math.Min(this.MaxValue, value);
            value = Math.Round(value * Math.Pow(10, this.Resolution));
            value = value / Math.Pow(10, this.Resolution);

            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                this.SensorValue = value;
                this.IsOnline = isOnline;
            });

            // If the online/offline state changed to offline/online, write out a new record
            if (this.IsOnline != isOnline)
            {
                _sensorValueRow = BuildDBTables.SensorDataTable.CreateRow();
                _sensorValueRow.SetField<long>("SensorId", this.SensorId);
                _sensorValueRow.SetField<double>("Value", value);
                _sensorValueRow.SetField<DateTime>("TimeUTC", timeUTC);
                _sensorValueRow.SetField<bool>("IsOnline", isOnline);
                _sensorValueRow.SetField<byte>("Bucket", _sensorValueBucket.CalculateBucket(timeUTC, isOnline));
                BuildDBTables.SensorDataTable.AddRow(_sensorValueRow);
                Debug.WriteLine("Sensor " + this.SensorId.ToString() + " value = " + value.ToString() + " isOnline = " + isOnline.ToString());

                _sensorValueRow = null;
                _lastDBWriteTime = timeUTC;
            }
            else if (forceFlush)
            {
                // Build a new observation and flush it out
                _sensorValueRow = BuildDBTables.SensorDataTable.CreateRow();
                _sensorValueRow.SetField<DateTime>("TimeUTC", timeUTC);
                _sensorValueRow.SetField<long>("SensorId", this.SensorId);
                _sensorValueRow.SetField<double>("Value", value);
                _sensorValueRow.SetField<bool>("IsOnline", isOnline);
                _sensorValueRow.SetField<byte>("Bucket", _sensorValueBucket.CalculateBucket(timeUTC, isOnline));
                BuildDBTables.SensorDataTable.AddRow(_sensorValueRow);
                Debug.WriteLine("Sensor " + this.SensorId.ToString() + " value = " + value.ToString() + " isOnline = " + isOnline.ToString());

                _sensorValueRow = null;
                _lastDBWriteTime = timeUTC;
            }
            else if ((_sensorValue == value) && (this.IsOnline == isOnline))
            {
                // If we do not have a row to contain this observation, create one.
                if (null == _sensorValueRow)
                {
                    _sensorValueRow = BuildDBTables.SensorDataTable.CreateRow();
                    _sensorValueRow.SetField<long>("SensorId", this.SensorId);
                    _sensorValueRow.SetField<double>("Value", value);
                    _sensorValueRow.SetField<DateTime>("TimeUTC", timeUTC);
                    _sensorValueRow.SetField<bool>("IsOnline", isOnline);
                    _sensorValueRow.SetField<byte>("Bucket", SensorValueBucket.LastBucket);
                    BuildDBTables.SensorDataTable.AddRow(_sensorValueRow);
                }

                // Has enough time elapsed since we last wrote a value to the datahase that we want to do it again.
                bool forceByTime = ((timeUTC - _lastDBWriteTime) >= this.Throttle);

                if (forceByTime)
                {
                    // If the sensorValue and isOnline not changed, them simply update the DateTime and the bucket
                    _sensorValueRow.SetField<DateTime>("TimeUTC", timeUTC);
                    _sensorValueRow.SetField<byte>("Bucket", _sensorValueBucket.CalculateBucket(timeUTC, isOnline));

                    Debug.WriteLine("Sensor " + this.SensorId.ToString() + " value = " + value.ToString() + " isOnline = " + isOnline.ToString());
                    _lastDBWriteTime = timeUTC;
                }
            }
        }

        async public Task BeginCommit()
        {
            if (this.IsDirty)
            {
                if (this.PropertyBag.IsDirty)
                {
                    lock (_lock)
                    {
                        this.Row.SetField<string>("PropertyBag", PropertyBag.JsonSerialize());
                    }
                }

                // Persist the row into the database
                await BuildDBTables.SensorTable.BeginCommitRow(this.Row,
                    () =>
                    {
                        Debug.Assert(this.Row.Field<int>("SensorId") > 0);
                    },
                    (ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
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

        public bool DemoMode
        {
            get { return _demoMode; }
            set
            {
                if (value)
                {
                    if (null == _valueTimer)
                    {
                        _valueTimer = new Timer(DemoValueTimerTic, null, 5000, 2000);
                    }
                    else
                    {
                        _valueTimer.Change(5000, 2000);
                    }
                }
                else
                {
                    if (null != _valueTimer)
                    {
                        _valueTimer.Change(Timeout.Infinite, Timeout.Infinite);
                    }
                }

                _demoMode = value;
            }
        }

        public string Description
        {
            get { return GetRowPropertyValue<string>(() => Description); }
            set { SetRowPropertyValue<string>(() => Description, value); }
        }

        public long DeviceId
        {
            get { return GetRowPropertyValue<long>(() => DeviceId); }
            set { SetRowPropertyValue<long>(() => DeviceId, value); }
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

        public long SensorId
        {
            get { return GetRowPropertyValue<long>(() => SensorId); }
            set { SetRowPropertyValue<long>(() => SensorId, value); }
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

                if (!IsOnline) return 0.0;

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
                    this.Row.SetField<string>("PropertyBag", _propertyBag.JsonSerialize());
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

        private void DemoValueTimerTic(object stateInfo)
        {
            double value = this.SensorValue;

            if (this.SensorValue == 0)
            {
                value = randu.Next((int)this.MinValue, (int)this.MaxValue);
            }
            else
            {
                double delta = randu.Next(0, (int)Math.Ceiling((this.MaxValue - this.MinValue) / 30.0));
                double sign = randu.Next();

                if (sign <= 0.5)
                {
                    value += delta;
                }
                else
                {
                    value -= delta;
                }

                value = Math.Min(value, this.MaxValue);
                value = Math.Max(value, this.MinValue);
            }

            this.AddSensorValue(value, true, false);
        }

        #endregion
    }

    /// <summary>
    /// This class places each datapoint into 1 or more buckets. Each bucket contains upto 100 data points for a 
    /// given interval of time. For example, if a value is in the 1 hour bucket, it will be returned for queries
    /// that request data for a given hour. This is an optimization to limit the amount of data being returned
    /// for plotting and analysis
    /// </summary>
    public class SensorValueBucket
    {
        public const int TotalBuckets = 21;                                         // 21 total buckets
        public const byte LastBucket = TotalBuckets - 1;
        private const int c_ObservationsPerBucket = 1000;                           // 10-0 observations per bucket
        private static TimeSpan[] _bucketTimeSpanList = new TimeSpan[TotalBuckets]; // The timespan for each of the buckets
        private DateTime[] _lastObservationTimePerBucket = new DateTime[TotalBuckets];
        private bool _isOnline = false;                                             // Is the sensor online?

        /// <summary>
        /// Calculate the timespan for each of the buckets
        /// </summary>
        static SensorValueBucket()
        {
            _bucketTimeSpanList[00] = new TimeSpan( 0,  0, 10, 0);      // 10 minutes
            _bucketTimeSpanList[01] = new TimeSpan( 0,  0, 30, 0);      // 30 minutes
            _bucketTimeSpanList[02] = new TimeSpan( 0,  1, 00, 0);      //  1 hour 
            _bucketTimeSpanList[03] = new TimeSpan( 0,  2, 00, 0);      //  2 hours 
            _bucketTimeSpanList[04] = new TimeSpan( 0,  3, 00, 0);      //  3 hours 
            _bucketTimeSpanList[05] = new TimeSpan( 0,  4, 00, 0);      //  4 hours 
            _bucketTimeSpanList[06] = new TimeSpan( 0,  6, 00, 0);      //  6 hours 
            _bucketTimeSpanList[07] = new TimeSpan( 0,  8, 00, 0);      //  8 hours 
            _bucketTimeSpanList[08] = new TimeSpan( 0, 12, 00, 0);      // 12 hours 
            _bucketTimeSpanList[09] = new TimeSpan( 1,  0, 00, 0);      //  1 day
            _bucketTimeSpanList[10] = new TimeSpan( 2,  0, 00, 0);      //  2 days
            _bucketTimeSpanList[11] = new TimeSpan( 3,  0, 00, 0);      //  3 days
            _bucketTimeSpanList[12] = new TimeSpan( 5,  0, 00, 0);      //  5 days
            _bucketTimeSpanList[13] = new TimeSpan( 7,  0, 00, 0);      //  7 days
            _bucketTimeSpanList[14] = new TimeSpan(14,  0, 00, 0);      // 14 days
            _bucketTimeSpanList[15] = new TimeSpan(28,  0, 00, 0);      // 28 days
            _bucketTimeSpanList[16] = new TimeSpan(60,  0, 00, 0);      // 60 days
            _bucketTimeSpanList[17] = new TimeSpan(90,  0, 00, 0);      // 90 days
            _bucketTimeSpanList[18] = new TimeSpan(120, 0, 00, 0);      // 120 days
            _bucketTimeSpanList[19] = new TimeSpan(180, 0, 00, 0);      // 180 days
            _bucketTimeSpanList[20] = new TimeSpan(360, 0, 00, 0);      // 360 days
        }

        /// <summary>
        /// Initialize all of the buckets so that on the first call we can fill all of them. The first
        /// datapoint needs to be written into all of the buckets
        /// </summary>
        public SensorValueBucket()
        {
            for (int i=0; i<TotalBuckets; i++)
            {
                _lastObservationTimePerBucket[i] = NormalizeDateTime(DateTime.MinValue);
            }
        }

        public byte CalculateBucket(DateTime timeUTC, bool isOnline)
        {
            // If the onLine status is changing, then write the value to ALL of the buckets
            if (isOnline != _isOnline)
            {
                _isOnline = isOnline;
                for (int i = 0; i < TotalBuckets; i++) _lastObservationTimePerBucket[i] = timeUTC;
                return LastBucket;
            }

            // Calculate which buckets will contain this entry. We will put the entry to all of the buckets
            // upto the bucket with the largest timespan covering the interval between the last entry for
            // the bucket and the timeUTC.
            byte lastBucket = 0;
            do
            {
                _lastObservationTimePerBucket[lastBucket] = timeUTC;
                lastBucket++;
            }
            while ((lastBucket < TotalBuckets) &&
                   (timeUTC - _lastObservationTimePerBucket[lastBucket]) > _bucketTimeSpanList[lastBucket]);

            return lastBucket;
        }

        /// <summary>
        /// Round the DateTime value to the nearest 100 milliseconds
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        static DateTime NormalizeDateTime(DateTime value)
        {
            Int64 tics = value.Ticks;
            tics /= 1000000;
            tics *= 1000000;
            tics = value.Ticks - tics;
            return value.AddTicks(-tics);
        }

        /// <summary>
        /// GIven a timespan, which of the buckets should we be usig to get the data?
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        static byte CalculateBucketFromTimeSpan(TimeSpan timeSpan)
        {
            byte lastBucket = 0;

            while ((lastBucket < TotalBuckets) &&
                   (_bucketTimeSpanList[lastBucket] < timeSpan))
            {
                lastBucket++;
            }

            // Don't run off the end of the buckets. The timespan may be longer than the timespan covered by
            // the last bucket. In this case, just return the last bucket.
            lastBucket = Math.Min(lastBucket, LastBucket);

            return lastBucket;
        }
    }
}
