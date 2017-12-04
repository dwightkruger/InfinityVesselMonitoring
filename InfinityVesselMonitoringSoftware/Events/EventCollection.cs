//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Types;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InfinityVesselMonitoringSoftware.Events
{
    public class EventCollection : ObservableCollection<IEventItem>
    {
        private bool _isLoaded = false;

        private object _listLock = new object();
        private ObservableCollection<IEventItem> _AlarmOnList = new ObservableCollection<IEventItem>();

        private System.Threading.Timer _timerSentEmail;
        private int _sixtyMinutes = 60 * 60 * 1000;
        private bool _SentEmailDelay = false;


        public EventCollection()
        {
            this.AlarmAudio = new AlarmAudio();
            Lock = new Object();

            // Load some of the existing events for this vessel
            Load();

            _timerSentEmail = new System.Threading.Timer(_timerSentEmail_Tick, null, 0, System.Threading.Timeout.Infinite);
        }

        public AlarmAudio AlarmAudio { get; set; }

        /// <summary>
        /// Add an event, and send email if appropriate
        /// </summary>
        /// <param name="myEventItem"></param>
        /// <param name="bRaiseEvent"></param>
        public void AddEvent(IEventItem myEventItem, bool bRaiseEvent)
        {
            bool sendEventEmail = false;
            myEventItem.Commit();

            lock (_listLock)
            {
                // If the alarm is ON/OFF, then add/delete it to the ON list.
                IEnumerable<IEventItem> query = null;
                query = from item in _AlarmOnList
                        where item.SensorId == myEventItem.SensorId
                        select item;

                if (myEventItem.IsAlarmOn && (query.Count() == 0))          // An alasm has started, sound a repeating audible alarm
                {
                    _AlarmOnList.Add(myEventItem);
                    this.AlarmAudio.IsContinuousAlarmSounding = true;
                    sendEventEmail = true;
                }
                else if (myEventItem.IsWarningOn && (query.Count() == 0))    // A warning has started, sound an audible alarm once
                {
                    this.AlarmAudio.IsOnceAlarmSounding = true;
                }
                else if (!myEventItem.IsAlarmOn && (query.Count() > 0))     // An alasm is over/cancelled. 
                {
                    IEventItem myOffAlarmItem = query.First();
                    _AlarmOnList.Remove(myOffAlarmItem);
                }

                if (_AlarmOnList.Count == 0)                                // If the list of alarms is empty, stop the audible signal
                {
                    this.AlarmAudio.IsContinuousAlarmSounding = false;
                }

            }

            lock (Lock)
            {
                base.Add(myEventItem);
            }

            // Raise the event outside of the lock.
            if (bRaiseEvent)
            {
                //RaiseAnEvent(Krill_Library.ValueChangeTypeEnum.Added, myEventItem, null);
            }

            if (sendEventEmail)
            {
                if (!_SentEmailDelay)
                {
                    this.SendEventEmail(myEventItem);
                    _SentEmailDelay = true;
                    _timerSentEmail.Change(_sixtyMinutes, System.Threading.Timeout.Infinite);
                }
            }
        }


        public void _timerSentEmail_Tick(object stateInfo)
        {
            _SentEmailDelay = false;
        }


        /// <summary>
        /// Calling this routine acknowledges an individual alarm
        /// </summary>
        /// <param name="myEventID"></param>
        /// <returns></returns>
        public void AlarmAcknowledge(int myEventID)
        {
            //IEventItem acknowledgedAlarmItem = new EventItem(_locator);
            //acknowledgedAlarmItem.SensorID = 0;
            //acknowledgedAlarmItem.EventPriority = 0;
            //acknowledgedAlarmItem.EventCode = AlarmCode.AlarmAcknowledged;
            //acknowledgedAlarmItem.Value = 0;

            //lock (Lock)
            //{
            //    IEnumerable<IEventItem> query =
            //        from item in this
            //        where item.EventId == myEventID
            //        select item;

            //    if (query.Count<IEventItem>() != 0)
            //    {
            //        IEventItem sourceEvent = query.First<IEventItem>();
            //        acknowledgedAlarmItem.SensorId = sourceEvent.SensorId;
            //    }
            //}

            //this.AddEvent(acknowledgedAlarmItem, true);
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

            lock (Lock)
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
        /// Calling this routine acknowledges all alarms and raises the AcKAlarm event
        /// </summary>
        /// <remarks></remarks>
        public void AlarmAcknowledge()
        {
            //bool bSendAck = false;              // Assume we are not acknowledging an alarm

            //lock (_listLock)
            //{
            //    AlertOnCount myAlerts = GetAlertCount();

            //    if (myAlerts.AllAlerts > 0)
            //    {
            //        // Mark each alarm item in the On list as being acknowledged. That way the 
            //        // Audio Gauge will know if there are any new alarms that need to be 
            //        // sounded.
            //        foreach (IEventItem myAlarmItem in _AlarmOnList)
            //        {
            //            if (!myAlarmItem.AlarmAcknowledged)
            //            {
            //                bSendAck = true;
            //                myAlarmItem.AlarmAcknowledged = true;
            //            }
            //        }
            //    }
            //}

            //// Signal the event outside of the lock
            //if (bSendAck)
            //{
            //    this.AlarmAudio.IsContinuousAlarmSounding = false;

            //    IEventItem ackAlarm = new EventItem(_locator);
            //    ackAlarm.EventDateTimeUTC = DateTime.UtcNow;
            //    ackAlarm.EventCode = AlarmCode.AlarmAcknowledged;
            //    this.AddEvent(ackAlarm, true);
            //}
        }


        /// <summary>
        /// Delete an event object from this collection based on the event item
        /// </summary>
        /// <param name="myAlarmItem"></param>
        /// <remarks></remarks>
        public void RemoveEvent(IEventItem myEventItem)
        {
            IEnumerable<IEventItem> query = null;

            lock (Lock)
            {
                lock (_listLock)
                {
                    // Delete the item from the AlarmON list
                    query = from item in _AlarmOnList
                            where item.EventId == myEventItem.EventId
                            select item;

                    if (query.Count() != 0)
                    {
                        IEventItem myDeletedAlarmItem = query.First();
                        _AlarmOnList.Remove(myDeletedAlarmItem);
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

            //RaiseAnEvent(Krill_Library.ValueChangeTypeEnum.Removed, myEventItem, null);
        }

        /// <summary>
        /// Delete an event from this collection based on the eventID
        /// </summary>
        /// <param name="myAlarmID"></param>
        /// <remarks></remarks>
        public void RemoveEvent(int myEventId)
        {
            lock (Lock)
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

                lock (_listLock)
                {
                    // Delete the item from the AlarmON list
                    query = from item in _AlarmOnList
                            where item.EventId == myEventId
                            select item;

                    if (query.Count() != 0)
                    {
                        _AlarmOnList.Remove(query.First<IEventItem>());
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

            lock (Lock)
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

            lock (Lock)
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
            lock (_listLock)
            {
                //_AlarmOnList.Sort(_Comparer);
                return _AlarmOnList;
            }
        }

        /// <summary>
        /// Get a list of all of the ON alarms.  Make a copy since this list could 
        /// change after we pass it to the caller.
        /// </summary>
        /// <returns></returns>
        public List<IEventItem> GetAllOnAlarmsList()
        {
            lock (_listLock)
            {
                //_AlarmOnList.Sort(_Comparer);
                return _AlarmOnList.ToList<IEventItem>();
            }
        }


        /// <summary>
        /// Returns only those Alerts which have not been acknowledged
        /// </summary>
        /// <returns></returns>
        //public AlertOnCount GetAlertCount()
        //{
        //    AlertOnCount myAlertCount = new AlertOnCount();

        //    lock (_listLock)
        //    {
        //        foreach (IEventItem myAlarmItem in _AlarmOnList)
        //        {
        //            if (!myAlarmItem.AlarmAcknowledged)
        //            {
        //                if (myAlarmItem.EventPriority < 50)
        //                {
        //                    myAlertCount.Warning += 1;
        //                }
        //                else
        //                {
        //                    myAlertCount.Alarm += 1;
        //                }
        //            }
        //        }
        //    }

        //    return myAlertCount;
        //}


        /// <summary>
        /// Loads events from persisted storage
        /// </summary>
        private void Load()
        {
            //lock (Lock)
            //{
            //    if (!_isLoaded)
            //    {
            //        this.Clear();
            //        _AlarmOnList.Clear();

            //        foreach (ItemRow row in _locator.EventsTable.Rows)
            //        {
            //            IEventItem newEvent = new EventItem(row.Field<Guid>("EventId"), _locator);
            //            base.Add(newEvent);
            //        }

            //        _isLoaded = true;
            //    }
            //}
        }

        public object Lock { get; set; }

        //private void RaiseAnEvent(Krill_Library.ValueChangeTypeEnum myChangeType,
        //                          IEventItem myAlarmItem,
        //                          IEventItem myReplacementItem)
        //{

        //    if (Changed != null)
        //        Changed(this, new EventCollectionChangedEventArgs(myChangeType, myAlarmItem, myReplacementItem));
        //}

        private void SendEventEmail(IEventItem myEvent)
        {
            //if (_locator.VesselAdminParameters.SendAlarmEmail)
            //{
            //    ISensorItem mySensor = _locator.SensorCollection.FindBySensorID(myEvent.SensorID);

            //    if (mySensor != null)
            //    {
            //        string strTextSubject = string.Empty;
            //        if (_locator.VesselAdminParameters.VesselName.Length > 0)
            //        {
            //            strTextSubject = "** Alarm Message from " + _locator.VesselAdminParameters.VesselName + " ** ";
            //        }
            //        else
            //        {
            //            strTextSubject = "** Alarm Message ** ";
            //        }

                    //Krill_Library.Globals.Email.QueueEmail(
                    //    _locator.VesselAdminParameters.EMail.OurEmailAddr,
                    //    _locator.VesselAdminParameters.ToEmailAddress,
                    //    _locator.VesselAdminParameters.VesselName,
                    //    strTextSubject,
                    //    "A new alarm condition has been detected." +
                    //        Environment.NewLine +
                    //        Environment.NewLine +
                    //        mySensor.Description +
                    //        Environment.NewLine +
                    //        mySensor.AlarmMessage(myEvent.Value, myEvent.EventCode), "");
            //    }
            //}
        }
    }
}
