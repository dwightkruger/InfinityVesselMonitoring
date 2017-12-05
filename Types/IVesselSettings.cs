//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////   

using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace InfinityGroup.VesselMonitoring.Types
{
    public interface IVesselSettings
    {
        string FromEmailAddress { get; set; }
        Image GetImage(string imageName);
        List<string> GetImageNames();
        void SetImage(Image image, string imageName);
        string ToEmailAddress { get; set; }
        string VesselImageName { get; set; }
        string VesselName { get; set; }

    }
}
