//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Globals;
using SQLitePCL;
using System;
using System.IO;
using Windows.UI;

namespace InfinityGroup.VesselMonitoring.SQLiteDB
{
    public class Utilities
    {
        public static string DateTimeSQLite(DateTime myDatetime)
        {
            const string dateTimeFormat = "{0:0000}-{1:00}-{2:00} {3:00}:{4:00}:{5:00}.{6:000}";
            string result = string.Format(dateTimeFormat, myDatetime.Year, myDatetime.Month, myDatetime.Day, myDatetime.Hour, myDatetime.Minute, myDatetime.Second, myDatetime.Millisecond);
            return result;
        }

        public static int BooleanSQLite(bool myBool)
        {
            return (myBool) ? 1 : 0;
        }

        public static double GetSize(ISQLiteConnection connection)
        {
            return 0;
        }

        public static DateTime Normalize(DateTime myDateTime)
        {
            Int64 tics = myDateTime.Ticks;
            tics /= 10000000;
            tics *= 10000000;
            tics = myDateTime.Ticks - tics;

            DateTime result = myDateTime.AddTicks(-tics);
            return result;
        }

        public static Color ColorFromI64(Int64 i64Value)
        {
            Color myBrush = Colors.Red;
            myBrush = Color.FromArgb(Convert.ToByte((i64Value & 0XFF000000) >> 24), 
                                     Convert.ToByte((i64Value & 0XFF0000) >> 16), 
                                     Convert.ToByte((i64Value & 0XFF00) >> 8), 
                                     Convert.ToByte(i64Value & 0XFF));

            return myBrush;
        }

        public static Int64 ColorToI64(Color brush)
        {
            Int64 value = 0;

            value = (Convert.ToInt64(brush.A) << 24) +
                    (Convert.ToInt64(brush.R) << 16) +
                    (Convert.ToInt64(brush.G) << 8) +
                    (Convert.ToInt64(brush.B));

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="myPath"></param>
        /// <param name="sidType"></param>
        public static void CreateDirectory(string myPath)
        {
            myPath = myPath.Trim();

            // The directory passed MUST exist, or must be created.  
            try
            {
                if (!System.IO.Directory.Exists(myPath))
                {
                    DirectoryInfo dInfo = new DirectoryInfo(myPath);
                    dInfo.Create();
                }
            }
            catch (Exception ex)
            {
                Telemetry.TrackException(ex);
            }
        }

    }
}
