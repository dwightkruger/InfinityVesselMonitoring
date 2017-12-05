/////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace InfinityGroup.VesselMonitoring.Utilities
{
    public class AlarmAudio
    {
        private object _lock = new object();
        private System.Threading.Timer _timerRecurringAlarm;
        private bool _isOnceAlarmSounding;
        private bool _isContinuousAlarmSounding;
        private StorageFile _alarmFile = null;
        private MediaPlayer _mediaPlayer;
        public AlarmAudio()
        {
            _mediaPlayer = new MediaPlayer();
        }

        async Task Play()
        {
            _alarmFile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///InfinityGroup.VesselMonitoring.Utilities/Properties/Alert.wav"));
            _mediaPlayer.Source = MediaSource.CreateFromStorageFile(_alarmFile);
            _mediaPlayer.Play();

            // Start a background timer job to signal the alarm. Start this timer job 3 seconds later, so that the system as a few seconds to start up.
            _timerRecurringAlarm = new System.Threading.Timer(timerRecurringAlarm_Tick, null, 3000, 3000);
        }

        public bool IsOnceAlarmSounding
        {
            get
            {
                return _isOnceAlarmSounding;
            }
            set
            {
                lock (_lock)
                {
                    _isOnceAlarmSounding = value;
                }
            }
        }

        public bool IsContinuousAlarmSounding
        {
            get
            {
                return _isContinuousAlarmSounding;
            }
            set
            {
                lock (_lock)
                {
                    _isContinuousAlarmSounding = value;
                }
            }
        }

        private void timerRecurringAlarm_Tick(object stateInfo)
        {
            lock (_lock)
            {
                try
                {
                    if (this.IsOnceAlarmSounding || this.IsContinuousAlarmSounding)
                    {
                        this.IsOnceAlarmSounding = false;
                        Globals.Globals.MediaPlayer.Play();
                    }
                    else
                    {
                    }
                }
                catch (Exception ex)
                {
                    Telemetry.TrackException(ex);
                }
            }
        }
    }
}
