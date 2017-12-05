/////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;

namespace InfinityGroup.VesselMonitoring.Utilities
{
    public static class AlarmAudio
    {
        private static object _lock = new object();
        private static System.Threading.Timer _timerRecurringAlarm = new System.Threading.Timer(timerRecurringAlarm_Tick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
        private static StorageFile _alarmFile = null;
        private static MediaPlayer _mediaPlayer = new MediaPlayer();
        private static List<PlayAlarmItem> _playList = new List<PlayAlarmItem>();

        /// <summary>
        /// Starts playing an alarm sound after 3 seconds. If isCOntinuous is true, it will play the alarm sound every 
        /// three seconds until it is dismissed.
        /// </summary>
        /// <param name="isContinuousAlarm"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        static public async Task PlayAlarm(bool isContinuousAlarm, int alarmId)
        {
            _alarmFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///InfinityGroup.VesselMonitoring.Utilities/Properties/Alert.wav"));
            PlayAlarmItem item = new PlayAlarmItem(_alarmFile, isContinuousAlarm, alarmId);
            _playList.Add(item);

            // Start a background timer job to signal the alarm. 
            // Start this timer job 3 seconds later, so that the system as a few seconds to start up.
            _timerRecurringAlarm.Change(3000, 3000);
        }

        static public void PlayAnnouncement(IStorageFile myAnnouncement, bool isContinuousAlarm, int announcementId)
        {
            PlayAlarmItem item = new PlayAlarmItem(myAnnouncement, isContinuousAlarm, announcementId);
            _playList.Add(item);

            // Start a background timer job to signal the alarm. 
            // Start this timer job 3 seconds later, so that the system as a few seconds to start up.
            _timerRecurringAlarm.Change(3000, 3000);
        }

        /// <summary>
        /// Cancel the alarm by the id provided. 
        /// </summary>
        /// <param name="alarmId"></param>
        static public void CancelAlarm(int alarmId)
        {
            lock (_lock)
            {
                PlayAlarmItem item = _playList.Find((entry) => entry.AlarmId == alarmId);
                if (null != item)
                {
                    _playList.Remove(item);
                }
            }
        }

        static private void timerRecurringAlarm_Tick(object stateInfo)
        {
            lock (_lock)
            {
                try
                {
                    Play();

                    // If there are zero items on the queue to play, then disasble the timer.
                    if (_playList.Count == 0)
                    {
                        _timerRecurringAlarm.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            }
        }

        static private void Play()
        {
            if (_playList.Count > 0)
            {
                int index = _playList.Count - 1;
                PlayAlarmItem item = (PlayAlarmItem)_playList[index];
                _playList.Remove(item);
                _mediaPlayer.Source = MediaSource.CreateFromStorageFile(item.AlarmFile);
                _mediaPlayer.Play();

                // If the item needs to be played continuously, then add it back to the list.
                if (item.IsContinuousAlarm)
                {
                    _playList.Add(item);
                }
            }
        }
    }

    public class PlayAlarmItem
    {
        public PlayAlarmItem(IStorageFile myAlarmFile, bool myIsContinuous, int myAlarmId)
        {
            this.AlarmFile = myAlarmFile;
            this.IsContinuousAlarm = myIsContinuous;
            this.AlarmId = myAlarmId;
        }
        public IStorageFile AlarmFile { get; set; }
        public bool IsContinuousAlarm { get; set; }
        public int AlarmId { get; set; }
    }
}
