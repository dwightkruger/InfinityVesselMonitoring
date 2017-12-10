//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Streams;

namespace InfinityGroup.VesselMonitoring.Utilities
{
    public static class AlarmAudio
    {
        private static object _lock;
        private static System.Threading.Timer _timerRecurringAlarm;
        private static StorageFile _alarmFile;
        private static MediaPlayer _mediaPlayer;
        private static List<PlayAlarmItem> _playList; 

        static AlarmAudio()
        {
            _lock = new object();
            _timerRecurringAlarm = new System.Threading.Timer(timerRecurringAlarm_Tick, null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            _alarmFile = null;
            _mediaPlayer = new MediaPlayer();
            _playList = new List<PlayAlarmItem>();
        }

        /// <summary>
        /// Starts playing an alarm sound after 3 seconds. If isContinuous is true, it will play the alarm sound every 
        /// three seconds until it is dismissed.
        /// </summary>
        /// <param name="isContinuousAlarm"></param>
        /// <param name="alarmId"></param>
        /// <returns></returns>
        static public async Task PlayAlarm(bool isContinuousAlarm, long alarmId)
        {
            if (null == _alarmFile)
            {
                _alarmFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///InfinityGroup.VesselMonitoring.Utilities/Properties/Alert.wav"));
            }

            PlayAnnouncement(_alarmFile, isContinuousAlarm, alarmId);
        }
         
        static public async Task PlaySpeech(string speech, bool isContinuousAlarm, int alarmId)
        {
            var synth = new SpeechSynthesizer();

            // Generate the audio stream from plain text.
            IRandomAccessStream stream = await synth.SynthesizeTextToStreamAsync(speech);

            IInputStream reader = stream.GetInputStreamAt(0);
            var bytes = new byte[stream.Size];

            //_mediaPlayer.Source = MediaSource.CreateFromStream
            //_mediaPlayer.Play();

            // Send the stream to the media object.
            //_mediaPlayer.SetSource(stream, stream.ContentType);
            //_mediaPlayer.Play();
        }

        static public void PlayAnnouncement(IStorageFile myAnnouncement, bool isContinuousAlarm, long announcementId)
        {
            PlayAlarmItem item = new PlayAlarmItem(myAnnouncement, isContinuousAlarm, announcementId);

            lock (_lock)
            {
                _playList.Add(item);
            }

            // Start a background timer job to signal the alarm. 
            // Start this timer job 3 seconds later, so that the system as a few seconds to start up.
            _timerRecurringAlarm.Change(3000, 3000);
        }

        /// <summary>
        /// Cancel the alarm by the id provided. 
        /// </summary>
        /// <param name="alarmId"></param>
        static public void CancelAlarm(long alarmId)
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
                // Try to get the first item under a lock.
                int index = -1;
                PlayAlarmItem item = null;
                lock (_lock)
                {
                    if (_playList.Count > 0)
                    {
                        index = _playList.Count - 1;
                        item = (PlayAlarmItem)_playList[index];
                        _playList.Remove(item);
                    }
                }

                if (null != item)
                {
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
    }

    /// <summary>
    /// Contains all of the information to play a given alarm.
    /// </summary>
    public class PlayAlarmItem
    {
        public PlayAlarmItem(IStorageFile myAlarmFile, bool myIsContinuous, long myAlarmId)
        {
            this.AlarmFile = myAlarmFile;
            this.IsContinuousAlarm = myIsContinuous;
            this.AlarmId = myAlarmId;
        }
        public IStorageFile AlarmFile { get; set; }
        public bool IsContinuousAlarm { get; set; }
        public long AlarmId { get; set; }
    }
}
