//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.SQLiteDB;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace InfinityVesselMonitoringSoftware.Events
{
    public class EventsCollection : ObservableCollection<IEventItem>
    {
        private readonly AsyncReaderWriterLock _lock = new AsyncReaderWriterLock();
        private ObservableCollection<IEventItem> _alarmOnList = new ObservableCollection<IEventItem>();

        public EventsCollection()
        {
        }

        /// <summary>
        /// Do not call this function. Call await BeginAdd instead.
        /// </summary>
        /// <param name="eventItem"></param>
        new public void Add(IEventItem eventItem)
        {
            throw new NotImplementedException("Use BeginAdd");
        }

        /// <summary>
        /// Add an event, and send email if appropriate
        /// </summary>
        /// <param name="myEventItem"></param>
        /// <param name="bRaiseEvent"></param>
        async public Task BeginAddEvent(IEventItem myEventItem, bool bRaiseEvent)
        {
            await myEventItem.BeginCommit();
            Debug.Assert(myEventItem.EventId > 0);

            // If the alarm is turned on, then add it to the list. If the alarm was turned off,
            // the delete it from the list.

            if (myEventItem.IsAlarmOn)
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    _alarmOnList.Add(myEventItem);
                }

                await AlarmAudio.PlayAlarm(false, myEventItem.EventId);
            }

            using (var releaser = await _lock.WriterLockAsync())
            {
                base.Add(myEventItem);
            }

            if (App.VesselSettings.SendAlarmEmail)
            {
                this.SendEventEmail(myEventItem);
            }
        }

        /// <summary>
        /// Calling this routine acknowledges an individual alarm
        /// </summary>
        /// <param name="myEventID"></param>
        /// <returns></returns>
        async public Task BeginAlarmAcknowledge(int myEventId)
        {
            // Find the event we are acknowledging.
            IEnumerable<IEventItem> query = null;
            using (var releaser = await _lock.ReaderLockAsync())
            {
                query = from item in this
                        where item.EventId == myEventId
                        select item;
            }

            // If we have found the trigger event, then build the acknowledge event
            if (query.Count<IEventItem>() > 0)
            {
                IEventItem triggerEvent = query.First<IEventItem>();

                IEventItem acknowledgedEvent = new EventItem(triggerEvent.SensorId);
                acknowledgedEvent.EventPriority = 0;
                acknowledgedEvent.EventCode = EventCode.AlarmAcknowledged;
                acknowledgedEvent.Value = 0;

                await this.BeginAddEvent(acknowledgedEvent, true);

                // if this event has triggered an alarm, cancel that alarm.
                AlarmAudio.CancelAlarm(triggerEvent.EventId);
            }
        }

        /// <summary>
        /// Find an event given a int eventID. Note that this returns a COPY of the event found, 
        /// since the event table could be flushed to disk at any time.
        /// </summary>
        /// <param name="myEventId"></param>
        /// <returns></returns>
        async public Task<IEventItem> BeginFindByEventId(int myEventId)
        {
            IEventItem result = null;
            IEnumerable<IEventItem> query = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                query = from item in this
                        where item.EventId == myEventId
                        select item;

                if (query.Count<IEventItem>() != 0)
                {
                    result = query.First<IEventItem>();
                }
            }

            return result;
        }


        /// <summary>
        /// Delete an event object from this collection based on the event item
        /// </summary>
        /// <param name="myAlarmItem"></param>
        /// <remarks></remarks>
        async public Task BeginRemoveEvent(IEventItem myEventItem)
        {
            IEnumerable<IEventItem> query = null;

            using (var releaser = await _lock.WriterLockAsync())
            {
                // Delete the item from the AlarmON list
                query = from item in _alarmOnList
                        where item.EventId == myEventItem.EventId
                        select item;

                if (query.Count() != 0)
                {
                    IEventItem myDeletedAlarmItem = query.First();
                    _alarmOnList.Remove(myDeletedAlarmItem);
                }
            }

            // If the item is still in this collection, delete it.
            query = from item in this
                    where item.EventId == myEventItem.EventId
                    select item;

            if (query.Count() != 0)
            {
                using (var releaser = await _lock.WriterLockAsync())
                {
                    base.Remove(query.First<IEventItem>());
                }
            }
        }

        /// <summary>
        /// Delete an event from this collection based on the eventID
        /// </summary>
        /// <param name="myAlarmID"></param>
        /// <remarks></remarks>
        async public Task BeginRemoveEvent(int myEventId)
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                // Delete the event if it is still in this collection
                IEnumerable<IEventItem> query = null;
                query = from item in this.AsEnumerable()
                        where item.EventId == myEventId
                        select item;
                if (query.Count<IEventItem>() > 0)
                {
                    base.Remove(query.First<IEventItem>());
                }

                // Delete the item from the AlarmON list
                query = from item in _alarmOnList
                        where item.EventId == myEventId
                        select item;

                if (query.Count() != 0)
                {
                    _alarmOnList.Remove(query.First<IEventItem>());
                }
            }
        }

        /// <summary>
        /// Delete all of the events for a given sensor from this collection
        /// </summary>
        /// <param name="mySensorId"></param>
        async public Task BeginDeleteEventsForSensor(int mySensorId)
        {
            List<IEventItem> myEventList = null;

            using (var releaser = await _lock.WriterLockAsync())
            {
                // Get the list of events corresponding to this sensor
                IEnumerable<IEventItem> query = null;
                query = from item in this.AsEnumerable()
                        where item.SensorId == mySensorId
                        select item;

                myEventList = query.ToList<IEventItem>();
            }

            if (myEventList != null)
            {
                foreach (IEventItem item in myEventList)
                {
                    await BeginRemoveEvent(item);
                }
            }
        }


        /// <summary>
        /// Get upto myMaxAlarms from the list of all historical alarms
        /// </summary>
        /// <param name="myMaxAlarms"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        async public Task<List<IEventItem>> BeginGetAllEvents(int myMaxAlarms)
        {
            List<IEventItem> myEventItemList = null;
            myEventItemList = new List<IEventItem>(100);

            // Get the list of alarm rows corresponding to this sensor
            IEnumerable<IEventItem> query = null;

            using (var releaser = await _lock.ReaderLockAsync())
            {
                query = from item in this.AsEnumerable()
                        orderby item.EventDateTimeUTC descending
                        select item;

                myEventItemList = query.Take(myMaxAlarms).ToList<IEventItem>();
            }

            return myEventItemList;
        }


        /// <summary>
        /// Get a list of all of the ON alarms.  
        /// </summary>
        /// <returns></returns>
        async public Task<ObservableCollection<IEventItem>> BeginGetAllOnAlarmsCollection()
        {
            using (var releaser = await _lock.ReaderLockAsync())
            {
                //_AlarmOnList.Sort(_Comparer);
                return _alarmOnList;
            }
        }

        /// <summary>
        /// Get a list of all of the ON alarms.  Make a copy since this list could 
        /// change after we pass it to the caller.
        /// </summary>
        /// <returns></returns>
        async public Task<List<IEventItem>> BeginGetAllOnAlarmsList()
        {
            using (var releaser = await _lock.ReaderLockAsync())
            {
                //_AlarmOnList.Sort(_Comparer);
                return _alarmOnList.ToList<IEventItem>();
            }
        }

        /// <summary>
        /// Empty the collection of events and the backing SQL store
        /// </summary>
        async public Task BeginEmpty()
        {
            using (var releaser = await _lock.WriterLockAsync())
            {
                await App.BuildDBTables.EventsTable.BeginEmpty();
                App.BuildDBTables.EventsTable.Load();
                _alarmOnList.Clear();
                base.Clear();
            }
        }

        /// <summary>
        /// Shutdown event collection and flush all records to the database
        /// </summary>
        /// <returns></returns>
        async public Task BeginShutdown()
        {
            if ((null != App.BuildDBTables) && (null != App.BuildDBTables.EventsTable))
            {
                await App.BuildDBTables.EventsTable.BeginCommitAllAndClear(() =>
                {
                },
                (ex) =>
                {
                    Telemetry.TrackException(ex);
                });
            }
        }

        /// <summary>
        /// Loads events from persisted storage
        /// </summary>
        private void Load()
        {
            lock (_lock)
            {
                this.Clear();
                _alarmOnList.Clear();

                foreach (ItemRow row in App.BuildDBTables.EventsTable.Rows)
                {
                    IEventItem newEvent = new EventItem(row);
                    base.Add(newEvent);
                }
            }
        }

        private void SendEventEmail(IEventItem myEvent)
        {
            ISensorItem mySensor = App.SensorCollection.FindBySensorId(myEvent.SensorId);

            if (mySensor != null)
            {
                string subject = string.Empty;
                if (App.VesselSettings.VesselName.Length > 0)
                {
                    subject = "** Alarm Message from " + App.VesselSettings.VesselName + " ** ";
                }
                else
                {
                    subject = "** Alarm Message ** ";
                }

                SendEmail.Send(App.VesselSettings.ToEmailAddress,
                               App.VesselSettings.VesselName,
                               subject,
                               "A new alarm condition has been detected." +
                                    Environment.NewLine +
                                    Environment.NewLine +
                                    mySensor.Description +
                                    Environment.NewLine +
                                    //mySensor.AlarmMessage(myEvent.Value, myEvent.EventCode),
                                    "",
                            string.Empty);
            }
        }
    }
}
