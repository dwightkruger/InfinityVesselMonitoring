/////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Globals;
using System;
using Windows.Media.Core;
using Windows.Media.Playback;
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

        public AlarmAudio()
        {
            try
            {
                Uri pathUri = new Uri("ms-appx:///Sounds/Alert.wav");

                Globals.Globals.MediaPlayer.Source = MediaSource.CreateFromUri(pathUri);
                Globals.Globals.MediaPlayer.AutoPlay = false;
                Globals.Globals.MediaPlayer.MediaPlayer.Play();
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }

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
                        Globals.Globals.MediaPlayer.MediaPlayer.Play();
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
