//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityGroup.VesselMonitoring.Types
{
    public interface IVesselSettings
    {
        string FromEmailAddress { get; set; }
        string FromEmailPassword { get; set; }
        BitmapImage GetBitmapImage(string bitMapImageName);
        List<string> GetImageNames();
        void SetBitmapImage(BitmapImage bitmapImage, string imageName);
        int SMTPEncryptionMethod { get; set; }
        int SMTPPort { get; set; }
        string SMTPServerName { get; set; }
        string ToEmailAddress { get; set; }
        string VesselImageName { get; set; }
        string VesselName { get; set; }
    }
}
