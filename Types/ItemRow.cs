//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;


namespace InfinityGroup.VesselMonitoring.Types
{
    public enum ItemRowState : int
    {
        Added,          // The row has been added to a ItemDataRowCollection, and AcceptChanges has not been called.
        Deleted,        // The row was deleted using the Delete method of the DataRow.
        Detached,       // The row has been created but is not part of any DataRowCollection. A DataRow is in this state immediately after it has been created and before it is added to a collection, or if it has been removed from a collection.
        Modified,       // The row has been modified and AcceptChanges has not been called.
        Unchanged       // The row has not changed since AcceptChanges was last called.
    }

    [DataContract]
    //public class ItemRow : ObservableObject
    public class ItemRow
    {
        volatile private List<object> _propertyBlob;
        volatile private ItemRowState _rowState;

        public ItemRow()
        {
            this.PropertyBlob = new List<object>();
            this.RollbackBlob = new List<object>();
            this.RowState = ItemRowState.Detached;
        }

        public void AcceptChanges()
        {
            Debug.Assert(null != this.Table);

            if (this.RowState == ItemRowState.Deleted)
            {
                this.Table.Rows.Remove(this);
            }
            else
            {
                this.RollbackBlob.Clear();

                object[] curr = new object[this.PropertyBlob.Count];
                this.PropertyBlob.CopyTo(curr, 0);
                for (int i = 0; i < curr.Count(); i++)
                {
                    this.RollbackBlob.Add(this.PropertyBlob[i]);
                }

                this.RowState = ItemRowState.Unchanged;
            }
        }

        public void Delete()
        {
            this.RowState = ItemRowState.Deleted;
        }


        public object this[int i]
        {
            get { return this.PropertyBlob[i]; }
            set { this.PropertyBlob[i] = value; }
        }

        [DataMember(Name = "PropertyBlob")]
        public List<object> PropertyBlob
        {
            get { return _propertyBlob; }
            private set { _propertyBlob = value; }
        }

        public void RejectChanges()
        {
            for (int i = 0; i < this.PropertyBlob.Count; i++)
            {
                this.PropertyBlob[i] = this.RollbackBlob[i];
            }

            this.RowState = ItemRowState.Unchanged;
        }

        [DataMember(Name = "RowState")]
        public ItemRowState RowState
        {
            get { return _rowState; }
            set { _rowState = value; }
        }

        public T Field<T>(string myPropertyName)
        {
            T value = default(T);

            try
            {
                ItemColumn col = this.Table.Columns.FirstOrDefault<ItemColumn>((column) => column.ColumnName == myPropertyName);
                if (null != col)
                {
                    int index = this.Table.Columns.IndexOf(col);
                    if (index < this.PropertyBlob.Count)
                    {
                        // We may have arrived here as a result of serialization/deserialization.
                        // Coerce some types back into their correct type and assignment them 
                        // back to the blob so they have the right type on output.
                        if ((typeof(T) != typeof(Int64)) && (PropertyBlob[index] is Int64))
                        {
                            Int32 result = Convert.ToInt32(PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }
                        else if ((typeof(T) == typeof(Int64)) && (PropertyBlob[index] is Int32))
                        {
                            Int64 result = Convert.ToInt64(PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }
                        else if ((typeof(T) == typeof(double)) && (PropertyBlob[index] is float))
                        {
                            double result = Convert.ToDouble(PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }
                        else if ((typeof(T) == typeof(float)) && (PropertyBlob[index] is double))
                        {
                            float result = Convert.ToSingle(PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }
                        else if ((typeof(T) == typeof(Guid)) && (PropertyBlob[index] is string))
                        {
                            Guid result = Guid.Parse((string)PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }
                        else if ((typeof(T) == typeof(uint)) && (PropertyBlob[index] is Int32))
                        {
                            uint result = Convert.ToUInt32(PropertyBlob[index]);
                            PropertyBlob[index] = result;
                        }

                        value = (T)this.PropertyBlob[index];
                    }
                }
            }
            catch (Exception ex)
            {

            }

            return (T)value;
        }

        public T Field<T>(string myPropertyName, T myDefaultValue)
        {
            T value = this.Field<T>(myPropertyName);

            if ((default(T) != null) &&
                (default(T).Equals(value)) &&
                (!default(T).Equals(myDefaultValue)))
            {
                value = myDefaultValue;
            }

            return value;
        }

        public bool SetField<T>(string myPropertyName, T myNewValue)
        {
            ItemColumn col = this.Table.Columns.FirstOrDefault<ItemColumn>((column) => column.ColumnName == myPropertyName);
            if (null != col)
            {
                int index = this.Table.Columns.IndexOf(col);
                object currValue = this.PropertyBlob[index];

                if ((null != myNewValue) &&
                    (null != currValue) &&
                    (currValue.Equals(myNewValue)))
                {
                    return false;
                }
                else
                {
                    this.PropertyBlob[index] = myNewValue;
                    if (this.RowState == ItemRowState.Unchanged)
                    {
                        this.RowState = ItemRowState.Modified;
                    }

                    return true;
                }
            }

            return false;
        }

        [DataMember(Name = "Table")]
        public ItemTable Table { get; set; }

        #region Internal

        internal List<object> RollbackBlob { get; set; }

        #endregion
    }

    public class ItemRowChanged
    {
        public ItemRowChanged(ItemRow myItemRow, ItemRowState myItemRowState)
        {
            this.ItemRow = myItemRow;
            this.ItemRowState = myItemRowState;
        }

        public ItemRow ItemRow { get; private set; }

        public ItemRowState ItemRowState { get; private set; }
    }
}
