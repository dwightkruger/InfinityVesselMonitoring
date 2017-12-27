//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Utilities;
using InfinityVesselMonitoringSoftware.Gauges;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using VesselMonitoringSuite.Devices;
using VesselMonitoringSuite.Sensors;
using Windows.Storage;
using Windows.UI.Xaml;

namespace InfinityVesselMonitoringSoftware.MockNMEA
{
    public class XMLParser
    {
        private List<IDeviceItem> _deviceItemList;
        private List<IGaugePageItem> _gaugePageItemList;

        public XMLParser(string xmlUrl)
        {
            this.XmlUrl = xmlUrl;
        }

        async public Task BeginParse()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.XmlUrl));
            Stream stream = await file.OpenStreamForReadAsync();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            _deviceItemList = this.ParseDevices(xmlDocument);
            foreach (IDeviceItem deviceItem in _deviceItemList)
            {
                await App.DeviceCollection.BeginAdd(deviceItem);
            }

            List<ISensorItem> sensorItemList = this.ParseSensors(xmlDocument);
            foreach (ISensorItem sensorItem in sensorItemList)
            {
                await App.SensorCollection.BeginAdd(sensorItem);
            }

            _gaugePageItemList = this.ParseGaugePages(xmlDocument);
            foreach (IGaugePageItem gaugePageItem in _gaugePageItemList)
            {
                await App.GaugePageCollection.BeginAdd(gaugePageItem);
            }

            List<IGaugeItem> gaugeItemlist = this.ParseGauges(xmlDocument);
            foreach (IGaugeItem gaugeItem in gaugeItemlist)
            {
                await App.GaugeItemCollection.BeginAdd(gaugeItem);
            }
        }

        private List<IDeviceItem> ParseDevices(XmlDocument xmlDocument)
        {
            List<IDeviceItem> deviceItemList = new List<IDeviceItem>();

            XmlNodeList deviceListNode = xmlDocument.GetElementsByTagName("Device");
            foreach (XmlNode node in deviceListNode)
            {
                XmlElement element = (XmlElement)node;

                IDeviceItem deviceItem = new DeviceItem()
                {
                    Description = element["Description"].InnerText,
                    DeviceAddress = Convert.ToByte(element["DeviceAddress"].InnerText),
                    DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), element["DeviceType"].InnerText),
                    FirmwareVersion = element["FirmwareVersion"].InnerText,
                    HardwareVersion = element["HardwareVersion"].InnerText,
                    IsVirtual = Convert.ToBoolean(element["IsVirtual"].InnerText),
                    IPAddress = element["IPAddress"].InnerText,
                    Location = element["Location"].InnerText,
                    Model = element["Model"].InnerText,
                    Name = element["Name"].InnerText,
                    Manufacturer = element["Manufacturer"].InnerText,
                    SerialNumber = element["SerialNumber"].InnerText,
                    SoftwareVersion = element["SoftwareVersion"].InnerText,
                };

                deviceItemList.Add(deviceItem);
            }

            return deviceItemList;
        }

        private List<ISensorItem> ParseSensors(XmlDocument xmlDocument)
        {
            List<ISensorItem> sensorItemList = new List<ISensorItem>();

            XmlNodeList sensorListNode = xmlDocument.GetElementsByTagName("Sensor");
            foreach (XmlNode node in sensorListNode)
            {
                XmlElement element = (XmlElement)node;

                // Get a index to the deviceId for this sensor
                int deviceIndex = Convert.ToInt32(element["DeviceIndex"].InnerText);

                ISensorItem sensorItem = new SensorItem(_deviceItemList[deviceIndex].DeviceId)
                {
                    Description = element["Description"].InnerText,
                    HighAlarmValue = Convert.ToDouble(element["HighAlarmValue"].InnerText),
                    HighWarningValue = Convert.ToDouble(element["HighWarningValue"].InnerText),
                    IsCalibrated = Convert.ToBoolean(element["IsCalibrated"].InnerText),
                    IsEnabled = Convert.ToBoolean(element["IsEnabled"].InnerText),
                    IsHighAlarmEnabled = Convert.ToBoolean(element["IsHighAlarmEnabled"].InnerText),
                    IsHighWarningEnabled = Convert.ToBoolean(element["IsHighWarningEnabled"].InnerText),
                    IsLowAlarmEnabled = Convert.ToBoolean(element["IsLowAlarmEnabled"].InnerText),
                    IsLowWarningEnabled = Convert.ToBoolean(element["IsLowWarningEnabled"].InnerText),
                    IsVirtual = Convert.ToBoolean(element["IsVirtual"].InnerText),
                    Location = element["Location"].InnerText,
                    LowAlarmValue = Convert.ToDouble(element["LowAlarmValue"].InnerText),
                    LowWarningValue = Convert.ToDouble(element["LowWarningValue"].InnerText),
                    MaxValue = Convert.ToDouble(element["MaxValue"].InnerText),
                    MinValue = Convert.ToDouble(element["MinValue"].InnerText),
                    Name = element["Name"].InnerText,
                    NominalValue = Convert.ToDouble(element["NominalValue"].InnerText),
                    PersistDataPoints = Convert.ToBoolean(element["PersistDataPoints"].InnerText),
                    PGN = Convert.ToUInt32(element["PGN"].InnerText),
                    PortNumber = Convert.ToInt32(element["PortNumber"].InnerText),
                    Priority = Convert.ToInt32(element["Priority"].InnerText),
                    Resolution = Convert.ToInt32(element["Resolution"].InnerText),
                    SensorType = (SensorType)Enum.Parse(typeof(SensorType), element["SensorType"].InnerText),
                    SensorUnits = (Units) Enum.Parse(typeof(Units), element["SensorUnits"].InnerText),
                    SensorUnitType = (UnitType) Enum.Parse(typeof(UnitType), element["SensorUnitType"].InnerText),
                    SensorUsage = (SensorUsage) Enum.Parse(typeof(SensorUsage), element["SensorUsage"].InnerText),
                    SerialNumber = element["SerialNumber"].InnerText,
                    ShowNominalValue = Convert.ToBoolean(element["ShowNominalValue"].InnerText),
                };

                sensorItemList.Add(sensorItem);
            }

            return sensorItemList;
        }

        private List<IGaugePageItem> ParseGaugePages(XmlDocument xmlDocument)
        {
            List<IGaugePageItem> gaugePageItemList = new List<IGaugePageItem>();

            XmlNodeList gaugePageItemListNode = xmlDocument.GetElementsByTagName("GaugePage");
            foreach (XmlNode node in gaugePageItemListNode)
            {
                XmlElement element = (XmlElement)node;

                IGaugePageItem gaugePageItem = new GaugePageItem()
                {
                    IsVisible = Convert.ToBoolean(element["IsVisible"].InnerText),
                    PageName = element["PageName"].InnerText,
                    Position = Convert.ToInt32(element["Position"].InnerText),
                };

                gaugePageItemList.Add(gaugePageItem);
            }

            return gaugePageItemList;
        }

        private List<IGaugeItem> ParseGauges(XmlDocument xmlDocument)
        {
            List<IGaugeItem> gaugeItemList = new List<IGaugeItem>();

            XmlNodeList gaugeListNode = xmlDocument.GetElementsByTagName("Gauge");
            foreach (XmlNode node in gaugeListNode)
            {
                XmlElement element = (XmlElement)node;

                // Get a index to the gaugePageId for this gauge
                int gaugePageIndex = Convert.ToInt32(element["GaugePageIndex"].InnerText);

                IGaugeItem gaugeItem = new GaugeItem(_gaugePageItemList[gaugePageIndex].PageId)
                {
                    GaugeType               = (GaugeTypeEnum) Enum.Parse(typeof(GaugeTypeEnum), element["GaugeType"].InnerText),               
                    SensorId                = Convert.ToInt64(element["SensorId"].InnerText),
                    GaugeHeight             = Convert.ToDouble(element["GaugeHeight"].InnerText),
                    GaugeWidth              = Convert.ToDouble(element["GaugeWidth"].InnerText),
                    GaugeLeft               = Convert.ToDouble(element["GaugeLeft"].InnerText),
                    GaugeTop                = Convert.ToDouble(element["GaugeTop"].InnerText),
                    GaugeColor              = ColorHelper.ToColor(element["GaugeColor"].InnerText),
                    Divisions               = Convert.ToInt32(element["Divisions"].InnerText),
                    MinorTicsPerMajorTic    = Convert.ToInt32(element["MinorTicsPerMajorTic"].InnerText),
                    MediumTicsPerMajorTic   = Convert.ToInt32(element["MediumTicsPerMajorTic"].InnerText),
                    Resolution              = Convert.ToInt32(element["Resolution"].InnerText),
                    GaugeOutlineVisibility  = (Visibility)Enum.Parse(typeof(Visibility), element["GaugeOutlineVisibility"].InnerText),
                    MiddleCircleDelta       = Convert.ToInt32(element["MiddleCircleDelta"].InnerText),
                    InnerCircleDelta        = Convert.ToInt32(element["InnerCircleDelta"].InnerText),
                    ValueFontSize           = Convert.ToDouble(element["ValueFontSize"].InnerText),
                    UnitsFontSize           = Convert.ToDouble(element["UnitsFontSize"].InnerText),
                    MajorTicLength          = Convert.ToDouble(element["MajorTicLength"].InnerText),
                    MediumTicLength         = Convert.ToDouble(element["MediumTicLength"].InnerText),
                    MinorTicLength          = Convert.ToDouble(element["MinorTicLength"].InnerText),
                    Text                    = element["Text"].InnerText,
                    TextFontSize            = Convert.ToDouble(element["TextFontSize"].InnerText),
                    TextAngle               = Convert.ToDouble(element["TextAngle"].InnerText),  
                    TextFontColor           = ColorHelper.ToColor(element["TextFontColor"].InnerText),
                    TextHorizontalAlignment = (CanvasHorizontalAlignment) Enum.Parse(typeof(CanvasHorizontalAlignment), element["TextHorizontalAlignment"].InnerText),
                    TextVerticalAlignment   = (CanvasVerticalAlignment) Enum.Parse(typeof(CanvasVerticalAlignment), element["TextVerticalAlignment"].InnerText),
                    Units                   = (Units) Enum.Parse(typeof(Units), element["Units"].InnerText)
                };

                gaugeItemList.Add(gaugeItem);
            }

            return gaugeItemList;
        }


        private string XmlUrl { get; set;}
    }
}
