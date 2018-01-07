//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Globals;
using SQLitePCL;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class SQLiteVesselTableBase<TItemType, TKeyType> : SQLiteTableBase<ItemRow, Int64>, IVesselTable
    {
        protected ItemSet _itemSet;
        protected IVesselDB _vesselDB;                          // Must be set in parent class

        public SQLiteVesselTableBase(IVesselDB myVesselDB)
        {
            this.Lock = myVesselDB.Lock;
            this.IsReadOnly = false;
            _vesselDB = myVesselDB;

            _itemSet = myVesselDB.ItemSet;
        }

        public void AddRow(ItemRow row)
        {
            if (IsReadOnly) return;

            lock (Lock)
            {
                this.ItemTable.Rows.Add(row);
            }
        }

        public ItemColumnCollection Columns
        {
            get
            {
                if (null == ItemTable) return null;
                return ItemTable.Columns;
            }
        }

        public int Count
        {
            get
            {
                if (null == Rows) return 0;
                return Rows.Count;
            }
        }

        public bool IsReadOnly { get; set; }

        public ItemTable ItemTable { get; set; }

        public ItemRowCollection Rows { get { return this.ItemTable.Rows; } }

        public virtual string TableName { get; protected set; }

        protected override ISQLiteConnection sqlConnection
        {
            get
            {
                Debug.Assert(null != _vesselDB.Connection);
                return (ISQLiteConnection)(_vesselDB.Connection);
            }
        }

        async public Task BeginCommitAll(Action successCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                lock (this.Lock)
                {
                    try
                    {
                        const string beginTransaction = "BEGIN TRANSACTION";
                        using (var statement = sqlConnection.Prepare(beginTransaction))
                        {
                            statement.Step();
                        }

                        foreach (ItemRow row in this.ItemTable.Rows)
                        {
                            switch (row.RowState)
                            {
                                case ItemRowState.Added:
                                    Int64 id = InsertItem(row);
                                    bool wasAdded = row.SetField<Int64>(PrimaryKeyName, id);

                                    row.AcceptChanges();
                                    break;

                                case ItemRowState.Deleted:
                                    DeleteItem(row.Field<Int64>(PrimaryKeyName));
                                    bool wasDeleted = this.ItemTable.Rows.Remove(row);

                                    row.AcceptChanges();
                                    break;

                                case ItemRowState.Detached:
                                    break;

                                case ItemRowState.Modified:
                                    UpdateItem(row.Field<Int64>(PrimaryKeyName), row);
                                    row.AcceptChanges();
                                    break;

                                case ItemRowState.Unchanged:
                                    break;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Telemetry.TrackException(ex);
                        failureCallback(ex);
                    }
                    finally
                    {
                        const string endTransaction = "END TRANSACTION";
                        using (var statement = sqlConnection.Prepare(endTransaction))
                        {
                            statement.Step();
                        }
                    }
                }

                successCallback();
            });
        }

        async public Task BeginCommitAllAndClear(Action successCallback, Action<Exception> failureCallback)
        {
            await BeginCommitAll(
                () =>
                {
                    this.ItemTable.Clear();

                    successCallback();
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                });
        }

        async virtual public Task BeginCommitRow(ItemRow row, Action successCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                lock (this.Lock)
                {
                    try
                    {
                        switch (row.RowState)
                        {
                            case ItemRowState.Added:
                                Int64 id = InsertItem(row);
                                bool wasAdded = row.SetField<Int64>(PrimaryKeyName, id);

                                row.AcceptChanges();
                                break;

                            case ItemRowState.Deleted:
                                DeleteItem(row.Field<Int64>(PrimaryKeyName));
                                bool wasDeleted = this.ItemTable.Rows.Remove(row);
                                Debug.Assert(wasDeleted);

                                row.AcceptChanges();
                                break;

                            case ItemRowState.Detached:
                                break;

                            case ItemRowState.Modified:
                                UpdateItem(row.Field<Int64>(PrimaryKeyName), row);

                                row.AcceptChanges();
                                break;

                            case ItemRowState.Unchanged:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Telemetry.TrackException(ex);
                        failureCallback(ex);
                    }

                }

                successCallback();
            });
        }

        async virtual public Task BeginCreateTable(Action successCallback, Action<Exception> failureCallback)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (IsReadOnly) return;

                    using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(GetCreateTableSql()))
                    {
                        statement.Step();
                    }

                    const string enableForeighKeys = "PRAGMA foreign_keys = ON;";
                    using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(enableForeighKeys))
                    {
                        statement.Step();
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                    failureCallback(ex);
                }

                successCallback();
            });
        }

        virtual public void Load()
        {
            try
            {
                lock (this.Lock)
                {
                    // If the itemTable has been created via some other code path, then
                    // attach to it here.
                    if (_itemSet.Tables.Contains((string)this.TableName) && (this.ItemTable == null))
                    {
                        this.ItemTable = _itemSet.Tables[(string)this.TableName];
                    }

                    // If this is the first time we are loading this itemTable, then simply
                    // add it to the itemSet.
                    if (null == this.ItemTable)
                    {
                        // Build the in-memory dataset and load it from the SQLite database. 
                        // This call will also create the schema for the dataTable
                        this.ItemTable = new ItemTable((string)this.TableName);
                        this.CreateTableSchema(this.ItemTable);

                        using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(GetSelectAllSql()))
                        {
                            while (statement.Step() == SQLiteResult.ROW)
                            {
                                this.LoadTableRow(statement);
                            }
                        }

                        this.ItemTable.AcceptChanges();
                        _itemSet.Tables.Add(this.ItemTable);
                    }
                    else
                    {
                        // We are trying to reload the dataset. Clear the existing one, and read the
                        // rows from SQL.  We don't need to rebuld the schema since the dataTable
                        // already has been created.
                        this.ItemTable.Clear();

                        using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(GetSelectAllSql()))
                        {
                            while (statement.Step() == SQLiteResult.ROW)
                            {
                                this.LoadTableRow(statement);
                            }

                            this.ItemTable.AcceptChanges();
                        }
                    }
                }

                // Build a primary key for this table so that records can be located based on
                // the primary key. 
                ItemColumn primaryKey = this.ItemTable.Columns[PrimaryKeyName];
                this.ItemTable.PrimaryKey = new ItemColumn[] { primaryKey };
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
                throw;
            }
        }

        protected virtual void CreateTableSchema(ItemTable itemTable)
        {
            throw new NotImplementedException();
        }

        protected virtual void LoadTableRow(ISQLiteStatement statement)
        {
            this.LoadTableRow(statement, this.ItemTable);
        }

        protected virtual void LoadTableRow(ISQLiteStatement statement, ItemTable itemTable)
        {
            throw new NotImplementedException();
        }

        public ItemTable Copy()
        {
            throw new NotImplementedException();
        }

        virtual public ItemRow CreateRow()
        {
            return CreateRow(this.ItemTable);
        }

        virtual public ItemRow CreateRow(ItemTable itemTable)
        {
            if (IsReadOnly) return null;
            ItemRow newRow;

            lock (Lock)
            {
                Debug.Assert(null != itemTable);
                newRow = itemTable.NewRow();
            }

            return newRow;
        }

        public async virtual Task BeginEmpty()
        {
            await Task.Run((Action)(() =>
            {
                try
                {
                    string str = "DELETE FROM " + this.TableName;
                    using (var statement = ((ISQLiteConnection)_vesselDB.Connection).Prepare(str))
                    {
                        SQLiteResult sqlLiteResult = statement.Step();
                    }

                    this.ItemTable.Clear();
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            }));
        }

        public ItemRow Find(Int64 id)
        {
            ItemRow result;

            lock (Lock)
            {
                result = this.ItemTable.Rows.Find(id, PrimaryKeyName);
            }

            return result;
        }

        async public Task BeginRemove(ItemRow row)
        {
            if (null != row)
            {
                row.Delete();

                await BeginCommitRow(row,
                    () =>
                    {
                    },
                    (ex) =>
                    {
                        Telemetry.TrackException(ex);
                    });
            }
        }

        async public Task BeginRemove(Int64 id)
        {
            await BeginRemove(this.Find(id));
        }

        async protected Task<int> TotalItems(string query)
        {
            int result = -1;

            await Task.Run(() =>
            {
                try
                {
                    using (var statement = sqlConnection.Prepare(query))
                    {
                        if (statement.Step() == SQLiteResult.ROW)
                        {
                            result = Convert.ToInt32(statement[0]);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            });

            return result;
        }

        protected override ItemRow CreateItem(ISQLiteStatement statement)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override void FillDeleteItemStatement(ISQLiteStatement statement, Int64 key)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override void FillInsertItemStatement(ISQLiteStatement statement, ItemRow item)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override void FillSelectAllStatement(ISQLiteStatement statement)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override void FillSelectItemStatement(ISQLiteStatement statement, Int64 key)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override void FillUpdateItemStatement(ISQLiteStatement statement, Int64 key, ItemRow item)
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetCreateTableSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetDeleteItemSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetInsertItemSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetSelectAllSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetSelectItemSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }

        protected override string GetUpdateItemSql()
        {
            throw new NotImplementedException("Implement in parent class");
        }
    }
}
