//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group LLC, All rights reserved.                    //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.Threading.Tasks;
using InfinityGroup.VesselMonitoring.Utilities;
using Microsoft.Graphics.Canvas.Text;
using Windows.UI;

namespace InfinityGroup.VesselMonitoring.Interfaces
{
    public enum GaugeTypeEnum
    {
        Unknown = 0,
        LeftArcGauge = 1,
        RightArcGauge = 2,
        LeftTankGauge = 3,
        RightTankGauge = 4,
        TextControl = 5,
        TextGauge = 6
    }

    public interface IGaugeItem
    {
        DateTime ChangeDate { get; set; }
        int Divisions { get; set; }
        Int64 GaugeId { get; }
        double GaugeHeight { get; set; }
        double GaugeLeft { get; set; }
        Windows.UI.Xaml.Visibility GaugeOutlineVisibility { get; set; }
        double GaugeTop { get; set; }
        Color GaugeColor { get; set; }
        GaugeTypeEnum GaugeType { get; set; }
        double GaugeWidth { get; set; }
        int InnerCircleDelta { get; set; }
        bool IsDirty { get; }
        double MajorTicLength { get; set; }
        int MinorTicsPerMajorTic   { get; set; }
        int MediumTicsPerMajorTic  { get; set; }
        double MediumTicLength { get; set; }
        int MiddleCircleDelta { get; set; }
        double MinorTicLength { get; set; }
        Int64 PageId { get; }
        int Resolution { get; set; }
        Int64 SensorId { get; set; }
        string Text { get; set; }
        double TextFontSize { get; set; }
        double TextAngle { get; set; }
        Color TextFontColor { get; set; }
        CanvasHorizontalAlignment TextHorizontalAlignment { get; set; }
        CanvasVerticalAlignment TextVerticalAlignment { get; set; }
        double ValueFontSize { get; set; }
        Units Units { get; set; }
        double UnitsFontSize { get; set; }
        Task BeginCommit();
        Task BeginDelete();
        void Rollback();
    }
}
