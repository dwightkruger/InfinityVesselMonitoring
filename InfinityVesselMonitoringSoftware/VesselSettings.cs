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
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityVesselMonitoringSoftware
{
    public class VesselSettings : ObservableObject, IVesselSettings
    {
        private const string c_imagePrefix = "image.";

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

        public string VesselImageName
        {
            get { return GetPropertyRowValue<string>(() => VesselImageName); }
            set { SetPropertyRowValue<string>(() => VesselImageName, value); }
        }

        public string VesselName
        {
            get { return GetPropertyRowValue<string>(() => VesselName); }
            set { SetPropertyRowValue<string>(() => VesselName, value); }
        }

        public Image GetImage(string imageName)
        {
            // Convert the byte array into a stream, then load it into the Image.
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            Task.Run(async () =>
            {
                byte[] rawImage = GetPropertyRowValue<byte[]>(c_imagePrefix + imageName);
                await stream.ReadAsync(rawImage.AsBuffer(), (uint)rawImage.Length, InputStreamOptions.None);
            }).Wait();

            BitmapImage bitmapImage = new BitmapImage();
            bitmapImage.SetSource(stream);

            Image result = new Image();
            result.Source = bitmapImage;

            return result;
        }

        public void SetImage(Image image, string imageName)
        {
            // Get the bitmap in the image and then serialize the bitmap into a byte array.
            byte[] rawImage = null;

            Task.Run(async () =>
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage = (BitmapImage)image.Source;
                Uri uri = bitmapImage.UriSource;

                StorageFile sourceFile = await StorageFile.GetFileFromApplicationUriAsync(uri);
                IRandomAccessStream stream = await sourceFile.OpenAsync(FileAccessMode.Read);
                rawImage = new byte[stream.Size];
                uint length = await stream.WriteAsync(rawImage.AsBuffer());
            }).Wait();
            
            SetPropertyRowValue<byte[]>(c_imagePrefix + imageName, rawImage);
        }

        public List<string> GetImageNames()
        {
            List<string> imageNames = new List<string>();
            foreach (ItemRow row in BuildDBTables.VesselSettingsTable.Rows)
            {
                if (row.Field<string>("Property").ToLowerInvariant().StartsWith(c_imagePrefix))
                {
                    imageNames.Add(row.Field<string>("Property").Substring(c_imagePrefix.Length));
                }
            }

            return imageNames;
        }

        #region privates

        private T GetPropertyRowValue<T>(string propertyName)
        {
            T value = default(T);

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
        /// Get the value of the property specified by the propertyExpression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyExpression"></param>
        /// <returns></returns>
        private T GetPropertyRowValue<T>(Expression<Func<T>> propertyExpression)
        {
            var propertyName = GetPropertyName(propertyExpression).ToLowerInvariant();

            return GetPropertyRowValue<T>(propertyName); 
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

        private bool SetPropertyRowValue<T>(string propertyName, T value)
        {
            T curValue = GetPropertyRowValue<T>(propertyName);

            if (value.Equals(curValue))
            {
                return false;
            }
            else
            {
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
                    await BuildDBTables.VesselSettingsTable.BeginCommitRow(newRow, () =>
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
        private bool SetPropertyRowValue<T>(Expression<Func<T>> propertyExpression, T value)
        {
            var propertyName = GetPropertyName(propertyExpression);

            return SetPropertyRowValue<T>(propertyName, value);
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
