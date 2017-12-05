//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using GalaSoft.MvvmLight;
using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityVesselMonitoringSoftware
{
    public class VesselSettings : ObservableObject, IVesselSettings
    {
        public VesselSettings()
        {
        }
        public string FromEmailAddress
        {
            get { return GetPropertyRowValue<string>(() => FromEmailAddress); }
            set { SetPropertyRowValue<string>(() => FromEmailAddress, value); }
        }

        public string ToEmailAddress
        {
            get { return GetPropertyRowValue<string>(() => ToEmailAddress); }
            set { SetPropertyRowValue<string>(() => ToEmailAddress, value); }
        }

        public string VesselName
        {
            get { return GetPropertyRowValue<string>(() => VesselName); }
            set { SetPropertyRowValue<string>(() => VesselName, value); }
        }

        public byte[] VesselImage
        {
            get { return GetPropertyRowValue<byte[]>(() => VesselImage); }
            set { SetPropertyRowValue<byte[]>(() => VesselImage, value); }
        }

        public Image Image
        {
            get
            {
                byte[] rawImage = GetPropertyRowValue<byte[]>(() => VesselImage);
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                //stream.ReadAsync(rawImage, rawImage.Length, InputStreamOptions.None);

                    return new Image();
             }
            set
            {
                BitmapSource myBitmap = (BitmapSource)value.Source;
                BitmapImage myImage = (BitmapImage)value.Source;
                IRandomAccessStream stream = new InMemoryRandomAccessStream();
                //stream.ReadAsync()

                byte[] rawImage = null;
                SetPropertyRowValue<byte[]>(() => VesselImage, rawImage);
                RaisePropertyChanged(() => VesselImage);
            }
        }

        #region privates

        /// <summary>
        /// Get the value of the property specified by the propertyExpression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        private T GetPropertyRowValue<T>(Expression<Func<T>> propertyExpression)
        {
            T value = default(T);

            var propertyName = GetPropertyName(propertyExpression).ToLowerInvariant();

            // Iterate through each of the rows looking for the property name
            foreach (ItemRow row in BuildDBTables.VesselSettingsTable.Rows)
            {
                if (row.Field<string>("Property").ToLowerInvariant() == propertyName)
                {
                    var @switch = new Dictionary<Type, Action> {
                                    { typeof(Int64),    () => { value = row.Field<T>(Constants.c_SystemInt64);     } },
                                    { typeof(float),    () => { value = row.Field<T>(Constants.c_SystemDouble);    } },
                                    { typeof(string),   () => { value = row.Field<T>(Constants.c_SystemString);    } },
                                    { typeof(byte[]),   () => { value = row.Field<T>(Constants.c_SystemByteArray); } },
                                    { typeof(DateTime), () => { value = row.Field<T>(Constants.c_SystemDateTime);  } },
                                    { typeof(bool),     () => { value = row.Field<T>(Constants.c_SystemBoolean);   } },
                    };

                    @switch[typeof(T)]();

                    break;
                }
            }

            return value;
        }

        /// <summary>
        /// Sends tye INotifyPropertyChanhges event for all of the properies in this class.
        /// </summary>
        private void RaisePropertyChangeAll()
        {
            // Raise an INotifyPropertyChanged for each column in the propertyBlob
            foreach (ItemRow row in BuildDBTables.VesselSettingsTable.Rows)
            {
                RaisePropertyChanged(row.Field<string>("Property"));
            }
        }

        private bool SetPropertyRowValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            T curValue = GetPropertyRowValue(propertyExpression);

            if (value.Equals(curValue))
            {
                return false;
            }
            else
            {
                var propertyName = GetPropertyName(propertyExpression);

                // Iterate through each of the rows looking for the property name
                foreach (ItemRow row in BuildDBTables.VesselSettingsTable.Rows)
                {
                    if (row.Field<string>("Property").ToLowerInvariant() == propertyName)
                    {
                        this.SetPropertyValue<T>(row, value);
                        break;
                    }
                }

                // If we get this far, the row does not exist. We'll create it and set the value.
                ItemRow newRow = BuildDBTables.VesselSettingsTable.CreateRow();
                this.SetPropertyValue<T>(newRow, value);

                BuildDBTables.VesselSettingsTable.AddRow(newRow);
                Task.Run(async () => 
                {
                    await BuildDBTables.VesselSettingsTable.BeginCommitRow(newRow, ()=>
                    {
                    }, 
                    (ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
                }).Wait();
            }

            return true;
        }

        private void SetPropertyValue<T>(ItemRow row, T value)
        {
            var @switch = new Dictionary<Type, Action> {
                                    { typeof(Int64),    () => { row.SetField<T>(Constants.c_SystemInt64, value);     } },
                                    { typeof(float),    () => { row.SetField<T>(Constants.c_SystemDouble, value);    } },
                                    { typeof(string),   () => { row.SetField<T>(Constants.c_SystemString, value);    } },
                                    { typeof(byte[]),   () => { row.SetField<T>(Constants.c_SystemByteArray, value); } },
                                    { typeof(DateTime), () => { row.SetField<T>(Constants.c_SystemDateTime, value);  } },
                                    { typeof(bool),     () => { row.SetField<T>(Constants.c_SystemBoolean, value);   } },
                        };

            @switch[typeof(T)]();
        }

        #endregion
    }
}
