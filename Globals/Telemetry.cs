//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2015 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////    

using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace InfinityGroup.VesselMonitoring.Globals
{
    /// <summary>
    /// See https://www.meziantou.net/2017/03/29/use-application-insights-in-a-desktop-application
    /// </summary>
    public static class Telemetry
    {
        private const string TelemetryKey = "a917134d-e034-49fb-902c-fe6806bff987";

        private static TelemetryClient _telemetry = GetAppInsightsClient();

        public static bool Enabled { get; set; } = true;

        private static TelemetryClient GetAppInsightsClient()
        {
            var config = new TelemetryConfiguration();
            config.InstrumentationKey = TelemetryKey;
            config.TelemetryChannel = new Microsoft.ApplicationInsights.Channel.InMemoryChannel();
            config.TelemetryChannel.DeveloperMode = Debugger.IsAttached;
#if DEBUG
            config.TelemetryChannel.DeveloperMode = true;
#endif
            TelemetryClient client = new TelemetryClient(config);
            //client.Context.Component.Version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            client.Context.Component.Version = "1.0.0.0";
            client.Context.Session.Id = Guid.NewGuid().ToString();
            client.Context.User.Id = (Environment.GetEnvironmentVariable("UserName") + Environment.GetEnvironmentVariable("MachineName")).GetHashCode().ToString();
            client.Context.Device.OperatingSystem = Environment.GetEnvironmentVariable("OSVersion");
            return client;
        }

        public static void SetUser(string user)
        {
            _telemetry.Context.User.Id = user;
        }

        public static void TrackEvent(string key, IDictionary<string, string> properties = null, IDictionary<string, double> metrics = null)
        {
            if (Enabled)
            {
                _telemetry.TrackEvent(key, properties, metrics);
                Debug.WriteLine("Telemetry.TrackEvent: " + key);
            }
        }

        public static void TrackException(Exception ex)
        {
            if (ex != null && Enabled)
            {
                var telex = new Microsoft.ApplicationInsights.DataContracts.ExceptionTelemetry(ex);
                _telemetry.TrackException(telex);
                Flush();
                Debug.WriteLine("Telemetry.TrackException: " + ex.Message);
            }
        }

        public static void Flush()
        {
            _telemetry.Flush();
        }
    }
}

