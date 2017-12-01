//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Runtime.Serialization;


namespace InfinityGroup.VesselMonitoring.Types
{
    [DataContract]
    public class ItemColumn
    {
        private object _defaultValue;

        public ItemColumn()
        {
        }

        public ItemColumn(string myName, Type myType)
        {
            this.ColumnName = myName;
            this.DataType = myType;
        }

        [DataMember(Name = "ColumnName")]
        public string ColumnName { get; set; }

        [DataMember(Name = "DataType")]
        public Type DataType { get; set; }

        [DataMember(Name = "DefaultValue")]
        public object DefaultValue
        {
            get { return _defaultValue; }
            set { _defaultValue = value; }
        }
    }
}
