//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

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
    public class EventCollection : ObservableCollection<IEventItem>
    {
        private object _lock = new object();
        private ObservableCollection<IEventItem> _alarmOnList = new ObservableCollection<IEventItem>();

        public EventCollection()
        {
            // Load some of the existing events for this vessel
            this.Load();
        }

        /// <summary>
        /// Add an event, and send email if appropriate
        /// </summary>
        /// <param name="myEventItem"></param>
        /// <param name="bRaiseEvent"></param>
        async public Task AddEvent(IEventItem myEventItem, bool bRaiseEvent)
        {
            bool sendEventEmail = false;
            await myEventItem.BeginCommit();
            Debug.Assert(myEventItem.EventId > 0);

            // If the alarm is turned on, then add it to the list. If the alarm was turned off,
            // the delete it from the list.

            if (myEventItem.IsAlarmOn)
            {
                lock (_lock)
                {
                    _alarmOnList.Add(myEventItem);
                }
                await AlarmAudio.PlayAlarm(false, myEventItem.EventId);
            }

            lock (_lock)
            {
                base.Add(myEventItem);
            }


            if (sendEventEmail)
            {
                this.SendEventEmail(myEventItem);
            }
        }

        /// <summary>
        /// Calling this routine acknowledges an individual alarm
        /// </summary>
        /// <param name="myEventID"></param>
        /// <returns></returns>
        async public Task AlarmAcknowledge(int myEventId)
        {
            // Find the event we are acknowledging.
            IEnumerable<IEventItem> query = null;
            lock (_lock)
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

                await this.AddEvent(acknowledgedEvent, true);

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
        public IEventItem FindByEventId(int myEventId)
        {
            IEventItem result = null;
            IEnumerable<IEventItem> query = null;

            lock (_lock)
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
        public void RemoveEvent(IEventItem myEventItem)
        {
            IEnumerable<IEventItem> query = null;

            lock (_lock)
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
                base.Remove(query.First<IEventItem>());
            }
        }

        /// <summary>
        /// Delete an event from this collection based on the eventID
        /// </summary>
        /// <param name="myAlarmID"></param>
        /// <remarks></remarks>
        public void RemoveEvent(int myEventId)
        {
            lock (_lock)
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

                lock (_lock)
                {
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
        }

        /// <summary>
        /// Delete all of the events for a given sensor from this collection
        /// </summary>
        /// <param name="mySensorId"></param>
        public void DeleteEventsForSensor(int mySensorId)
        {
            List<IEventItem> myEventList = null;

            lock (_lock)
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
                    RemoveEvent(item);
                }
            }
        }


        /// <summary>
        /// Get upto myMaxAlarms from the list of all historical alarms
        /// </summary>
        /// <param name="myMaxAlarms"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public List<IEventItem> GetAllEvents(int myMaxAlarms)
        {
            List<IEventItem> myEventItemList = null;
            myEventItemList = new List<IEventItem>(100);

            // Get the list of alarm rows corresponding to this sensor
            IEnumerable<IEventItem> query = null;

            lock (_lock)
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
        public ObservableCollection<IEventItem> GetAllOnAlarmsCollection()
        {
            lock (_lock)
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
        public List<IEventItem> GetAllOnAlarmsList()
        {
            lock (_lock)
            {
                //_AlarmOnList.Sort(_Comparer);
                return _alarmOnList.ToList<IEventItem>();
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

                foreach (ItemRow row in BuildDBTables.EventsTable.Rows)
                {
                    IEventItem newEvent = new EventItem(row);
                    base.Add(newEvent);
                }
            }
        }

        private void SendEventEmail(IEventItem myEvent)
        {
            //if (_locator.VesselAdminParameters.SendAlarmEmail)
            //{
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
