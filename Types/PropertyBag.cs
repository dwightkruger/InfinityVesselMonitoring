//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class PropertyBag : ObservableObject
    {
        private object _lock = new object();

        /// <summary>
        /// Public constructor
        /// </summary>
        public PropertyBag()
        {
            PropertyNameList = new ItemColumnCollection();
            PropertyValueList = new List<object>();

            RollbackPropertyNameList = new ItemColumnCollection();
            RollbackPropertyValueList = new List<object>();

            IsDirty = false;
        }

        [DataMember(Name = "PropertyNameList")]
        private ItemColumnCollection PropertyNameList { get; set; }

        [DataMember(Name = "PropertyValueList")]
        private List<object> PropertyValueList { get; set; }

        /// <summary>
        /// Deletes the property and value given the property name specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public bool Delete<T>(string propertyName)
        {
            lock (_lock)
            {
                if (PropertyNameList.Contains(propertyName) &&
                    PropertyNameList[propertyName].DataType == typeof(T))
                {
                    PropertyNameList.Remove(propertyName);
                    IsDirty = true;

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Get the current value of a property. If the property does not exist, the default value
        /// for the type will be returned and the function returns FALSE. Property names are
        /// case insensitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Get<T>(string propertyName, out T value)
        {
            lock (_lock)
            {
                value = default(T);

                if (PropertyNameList.Contains(propertyName) &&
                    PropertyNameList[propertyName].DataType == typeof(T))
                {
                    ItemColumn column = PropertyNameList[propertyName];
                    int index = PropertyNameList.IndexOf(column);

                    value = (T)PropertyValueList[index];

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Stores the new value for the names property. If the property does not exist in the blob
        /// a new column will be created. Property names are case insensitive.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propName"></param>
        /// <param name="value"></param>
        public void Set<T>(string propertyName, T value)
        {
            lock (_lock)
            {
                if (!PropertyNameList.Contains(propertyName))
                {
                    PropertyNameList.Add(propertyName, typeof(T));
                }

                ItemColumn column = PropertyNameList[propertyName];
                PropertyValueList.Insert(PropertyNameList.IndexOf(column), value);

                IsDirty = true;

                RaisePropertyChanged(propertyName);
            }
        }


        public string JsonSerialize()
        {
            // Setup the rollback data
            this.RollbackPropertyValueList = new List<object>(this.PropertyValueList);
            this.RollbackPropertyNameList = new ItemColumnCollection();
            foreach (ItemColumn column in this.PropertyNameList)
            {
                this.RollbackPropertyNameList.Add(new ItemColumn(column.ColumnName, column.DataType));
            }

            string json = JsonConvert.SerializeObject(this,
                    Formatting.Indented,
                    new JsonSerializerSettings()
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

            IsDirty = false;

            return json;
        }

        public void JsonDeserialize(string json)
        {
            lock (_lock)
            {
                try
                {
                    PropertyBag newBag = (PropertyBag)JsonConvert.DeserializeObject<PropertyBag>(json);
                    this.PropertyValueList = newBag.PropertyValueList;
                    this.PropertyNameList = newBag.PropertyNameList;

                    // Setup the rollback data
                    this.RollbackPropertyValueList = new List<object>(this.PropertyValueList);
                    this.RollbackPropertyNameList = new ItemColumnCollection();
                    foreach (ItemColumn column in this.PropertyNameList)
                    {
                        this.RollbackPropertyNameList.Add(new ItemColumn(column.ColumnName, column.DataType));
                    }

                    foreach (ItemColumn column in PropertyNameList)
                    {
                        int index = PropertyNameList.IndexOf(column);

                        // We may have arrived here as a result of serialization/deserialization.
                        // Coerce some types back into their correct type and assignment them 
                        // back to the blob so they have the right type on output.
                        if (column.DataType == typeof(Int32))
                        {
                            Int32 result = Convert.ToInt32(PropertyValueList[index]);
                            PropertyValueList[index] = result;
                        }
                        else if (column.DataType == typeof(Int64))
                        {
                            Int64 result = Convert.ToInt64(PropertyValueList[index]);
                            PropertyValueList[index] = result;
                        }
                        else if (column.DataType == typeof(double))
                        {
                            double result = Convert.ToDouble(PropertyValueList[index]);
                            PropertyValueList[index] = result;
                        }
                        else if (column.DataType == typeof(float))
                        {
                            float result = Convert.ToSingle(PropertyValueList[index]);
                            PropertyValueList[index] = result;
                        }
                        else if (column.DataType == typeof(Guid))
                        {
                            Guid result = Guid.Parse((string)PropertyValueList[index]);
                            PropertyValueList[index] = result;
                        }
                    }

                    IsDirty = false;

                    foreach (ItemColumn column in PropertyNameList)
                    {
                        RaisePropertyChanged(column.ColumnName);
                    }
                }
                catch
                {
                    PropertyNameList.Clear();
                    PropertyValueList.Clear();
                }
            }
        }

        public void Rollback()
        {
            lock (_lock)
            {
                if ((IsDirty) && (PropertyNameList.Count > 0))
                {
                    this.PropertyValueList = new List<object>(this.RollbackPropertyValueList);
                    this.PropertyNameList = new ItemColumnCollection();

                    foreach (ItemColumn column in this.RollbackPropertyNameList)
                    {
                        this.PropertyNameList.Add(new ItemColumn(column.ColumnName, column.DataType));
                    }
                }
            }
        }

        public bool IsDirty { get; set; }

        /// <summary>
        /// Get the list of properties in this blob
        /// </summary>
        /// <returns></returns>
        public List<string> PropertyNames()
        {
            List<string> result = new List<string>();

            lock (_lock)
            {
                foreach (ItemColumn column in PropertyNameList)
                {
                    result.Add(column.ColumnName);
                }
            }

            return result;
        }

        private ItemColumnCollection RollbackPropertyNameList { get; set; }

        private List<object> RollbackPropertyValueList { get; set; }
    }

}
