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
using System.IO;
using System.Linq.Expressions;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
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
        public string FromEmailPassword
        {
            get { return GetPropertyRowValue<string>(() => FromEmailPassword); }
            set { SetPropertyRowValue<string>(() => FromEmailPassword, value); }
        }

        /// <summary>
        /// Get the BitmapImage from the database given the iamgeName provided.
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public BitmapImage GetBitmapImage(string imageName)
        {
            BitmapImage bitmapImage = new BitmapImage();
            byte[] buffer = GetPropertyRowValue<byte[]>(c_imagePrefix + imageName);

            // Convert the byte array into a stream, then load it into the BitmapImage.
            IRandomAccessStream stream = new InMemoryRandomAccessStream();
            stream.Seek(0);

            Task.Run(async () =>
            {
                await stream.WriteAsync(buffer.AsBuffer());
            }).Wait();

            stream.Seek(0);
            bitmapImage.SetSource(stream);
            return bitmapImage;
        }

        /// <summary>
        /// Get a list of bitmap images saved in the database.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Save the BitmapImage image provided with the image name provided.
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <param name="imageName"></param>
        public void SetBitmapImage(BitmapImage bitmapImage, string imageName)
        {
            // Get the bitmap in the image and then serialize the bitmap into a byte array.
            byte[] buffer = null;

            Uri uri = bitmapImage.UriSource;

            Task.Run(async () =>
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                var inputStream = await file.OpenSequentialReadAsync();
                var readStream = inputStream.AsStreamForRead();
                buffer = new byte[readStream.Length];
                await readStream.ReadAsync(buffer, 0, buffer.Length);
            }).Wait();

            // Save the value in the DB.
            SetPropertyRowValue<byte[]>(c_imagePrefix + imageName, buffer);
        }

        public int SMTPEncryptionMethod
        {
            get { return GetPropertyRowValue<int>(() => SMTPEncryptionMethod); }
            set { SetPropertyRowValue<int>(() => SMTPEncryptionMethod, value); }
        }

        public int SMTPPort
        {
            get { return GetPropertyRowValue<int>(() => SMTPPort); }
            set { SetPropertyRowValue<int>(() => SMTPPort, value); }
        }

        public string SMTPServerName
        {
            get { return GetPropertyRowValue<string>(() => SMTPServerName); }
            set { SetPropertyRowValue<string>(() => SMTPServerName, value); }
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





        #region privates

        private T GetPropertyRowValue<T>(string propertyName)
        {
            T value = default(T);

            // Iterate through each of the rows looking for the property name
            foreach (ItemRow row in BuildDBTables.VesselSettingsTable.Rows)
            {
                if (row.Field<string>("Property") == propertyName)
                {
                    var @switch = new Dictionary<Type, Action> {
                                    { typeof(Int64),    () => { value = row.Field<T>(Constants.c_SystemInt64);     } },
                                    { typeof(int),      () => { value = row.Field<T>(Constants.c_SystemInt64);     } },
                                    { typeof(float),    () => { value = row.Field<T>(Constants.c_SystemDouble);    } },
                                    { typeof(double),   () => { value = row.Field<T>(Constants.c_SystemDouble);    } },
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
            var propertyName = GetPropertyName(propertyExpression);

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
                        return true;
                    }
                }

                // If we get this far, the row does not exist. We'll create it and set the value.
                ItemRow newRow = BuildDBTables.VesselSettingsTable.CreateRow();
                newRow.SetField<string>("Property", propertyName);
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
                                    { typeof(int),      () => { row.SetField<T>(Constants.c_SystemInt64, value);     } },
                                    { typeof(Int64),    () => { row.SetField<T>(Constants.c_SystemInt64, value);     } },
                                    { typeof(float),    () => { row.SetField<T>(Constants.c_SystemDouble, value);    } },
                                    { typeof(double),   () => { row.SetField<T>(Constants.c_SystemDouble, value);    } },
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
