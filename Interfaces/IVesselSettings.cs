//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public interface IVesselSettings
    {
        long AISRefreshInterval { get; set; }
        Task BeginCommit();
        Task BeginDeleteImage(string imageName);
        string FromEmailAddress { get; set; }
        string FromEmailPassword { get; set; }
        BitmapImage GetBitmapImage(string bitMapImageName);
        ObservableCollection<string> ImageNameList();
        bool IsMKS { get; set; }
        bool IsNightMode { get; set; }
        void SetBitmapImage(BitmapImage bitmapImage, string imageName);
        int SMTPEncryptionMethod { get; set; }
        int SMTPPort { get; set; }
        string SMTPServerName { get; set; }
        Color ThemeColor { get; set; }
        string ToEmailAddress { get; set; }
        string VesselImageName { get; set; }
        string VesselName { get; set; }
    }
}
