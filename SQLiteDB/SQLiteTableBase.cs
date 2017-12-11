//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Types;
using SQLitePCL;
using System;
using System.Collections.ObjectModel;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public abstract class SQLiteTableBase<TItemType, TKeyType>
    {
        protected abstract string GetCreateTableSql();

        protected abstract string GetSelectAllSql();
        protected abstract void FillSelectAllStatement(ISQLiteStatement statement);


        protected abstract TItemType CreateItem(ISQLiteStatement statement);


        protected abstract string GetSelectItemSql();
        protected abstract void FillSelectItemStatement(ISQLiteStatement statement, TKeyType key);


        protected abstract string GetDeleteItemSql();
        protected abstract void FillDeleteItemStatement(ISQLiteStatement statement, TKeyType key);


        protected abstract string GetInsertItemSql();
        protected abstract void FillInsertItemStatement(ISQLiteStatement statement, TItemType item);


        protected abstract string GetUpdateItemSql();
        protected abstract void FillUpdateItemStatement(ISQLiteStatement statement, TKeyType key, TItemType item);

        protected abstract ISQLiteConnection sqlConnection { get; }


        public ObservableCollection<TItemType> GetAllItems()
        {
            var items = new ObservableCollection<TItemType>();
            using (var statement = sqlConnection.Prepare(GetSelectAllSql()))
            {
                FillSelectAllStatement(statement);
                while (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    items.Add(item);
                }
            }

            return items;
        }


        public TItemType GetItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetSelectItemSql()))
            {
                FillSelectItemStatement(statement, key);

                if (statement.Step() == SQLiteResult.ROW)
                {
                    var item = CreateItem(statement);
                    return item;
                }
            }

            throw new ArgumentOutOfRangeException("GetItem() Key not found");
        }

        /// <summary>
        /// Insert an item into the table and return the primary key value created for this new item.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int64 InsertItem(TItemType item)
        {
            Int64 result = -1;

            lock (Lock)
            {
                using (var statement = sqlConnection.Prepare(GetInsertItemSql()))
                {
                    this.FillInsertItemStatement(statement, item);
                    SQLiteResult output = statement.Step();

                    if (output != SQLiteResult.DONE)
                    {
                        string foo = sqlConnection.ErrorMessage();
                        throw new Exception(output.ToString());
                    }

                    result = (Int64)sqlConnection.LastInsertRowId();
                }
            }

            return result;
        }


        public void UpdateItem(TKeyType key, TItemType item)
        {
            using (var statement = sqlConnection.Prepare(GetUpdateItemSql()))
            {
                FillUpdateItemStatement(statement, key, item);
                statement.Step();
            }
        }


        public void DeleteItem(TKeyType key)
        {
            using (var statement = sqlConnection.Prepare(GetDeleteItemSql()))
            {
                FillDeleteItemStatement(statement, key);
                statement.Step();
            }
        }

        public object Lock { get; protected set; }

        public string PrimaryKeyName { get; protected set; }

    }

}
