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
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;

namespace InfinityVesselMonitoringSoftware.MockNMEA
{
    public class XMLParser
    {
        private List<IDeviceItem> _deviceItemList;
        private List<IGaugePageItem> _gaugePageItemList;
        private List<ISensorItem> _sensorItemList;
        private List<IGaugeItem> _gaugeItemlist;

        public XMLParser(string xmlUrl)
        {
            this.XmlUrl = xmlUrl;
        }

        async public Task BeginParse(Size screenSize)
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

            _sensorItemList = this.ParseSensors(xmlDocument);
            foreach (ISensorItem sensorItem in _sensorItemList)
            {
                await App.SensorCollection.BeginAdd(sensorItem);
            }

            _gaugePageItemList = this.ParseGaugePages(xmlDocument);
            foreach (IGaugePageItem gaugePageItem in _gaugePageItemList)
            {
                await App.GaugePageCollection.BeginAdd(gaugePageItem);
            }

            _gaugeItemlist = this.ParseGauges(xmlDocument);
            foreach (IGaugeItem gaugeItem in _gaugeItemlist)
            {
                gaugeItem.GaugeHeight = screenSize.Height / 4.0;
                gaugeItem.GaugeWidth = screenSize.Width / 4.0;
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

                IDeviceItem deviceItem = new DeviceItem();

                if (null != element["Description"]) deviceItem.Description = element["Description"].InnerText;
                if (null != element["DeviceAddress"]) deviceItem.DeviceAddress = Convert.ToByte(element["DeviceAddress"].InnerText);
                if (null != element["DeviceType"]) deviceItem.DeviceType = (DeviceType)Enum.Parse(typeof(DeviceType), element["DeviceType"].InnerText);
                if (null != element["FirmwareVersion"]) deviceItem.FirmwareVersion = element["FirmwareVersion"].InnerText;
                if (null != element["HardwareVersion"]) deviceItem.HardwareVersion = element["HardwareVersion"].InnerText;
                if (null != element["IsVirtual"]) deviceItem.IsVirtual = Convert.ToBoolean(element["IsVirtual"].InnerText);
                if (null != element["IPAddress"]) deviceItem.IPAddress = element["IPAddress"].InnerText;
                if (null != element["Location"]) deviceItem.Location = element["Location"].InnerText;
                if (null != element["Model"]) deviceItem.Model = element["Model"].InnerText;
                if (null != element["Name"]) deviceItem.Name = element["Name"].InnerText;
                if (null != element["Manufacturer"]) deviceItem.Manufacturer = element["Manufacturer"].InnerText;
                if (null != element["SerialNumber"]) deviceItem.SerialNumber = element["SerialNumber"].InnerText;
                if (null != element["SoftwareVersion"]) deviceItem.SoftwareVersion = element["SoftwareVersion"].InnerText;

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

                ISensorItem sensorItem = new SensorItem(_deviceItemList[deviceIndex].DeviceId);

                if (null != element["Description"]) sensorItem.Description = element["Description"].InnerText;
                if (null != element["HighAlarmValue"]) sensorItem.HighAlarmValue = Convert.ToDouble(element["HighAlarmValue"].InnerText);
                if (null != element["HighWarningValue"]) sensorItem.HighWarningValue = Convert.ToDouble(element["HighWarningValue"].InnerText);
                if (null != element["IsCalibrated"]) sensorItem.IsCalibrated = Convert.ToBoolean(element["IsCalibrated"].InnerText);
                if (null != element["IsEnabled"]) sensorItem.IsEnabled = Convert.ToBoolean(element["IsEnabled"].InnerText);
                if (null != element["IsHighAlarmEnabled"]) sensorItem.IsHighAlarmEnabled = Convert.ToBoolean(element["IsHighAlarmEnabled"].InnerText);
                if (null != element["IsHighWarningEnabled"]) sensorItem.IsHighWarningEnabled = Convert.ToBoolean(element["IsHighWarningEnabled"].InnerText);
                if (null != element["IsLowAlarmEnabled"]) sensorItem.IsLowAlarmEnabled = Convert.ToBoolean(element["IsLowAlarmEnabled"].InnerText);
                if (null != element["IsLowWarningEnabled"]) sensorItem.IsLowWarningEnabled = Convert.ToBoolean(element["IsLowWarningEnabled"].InnerText);
                if (null != element["IsVirtual"]) sensorItem.IsVirtual = Convert.ToBoolean(element["IsVirtual"].InnerText);
                if (null != element["Location"]) sensorItem.Location = element["Location"].InnerText;
                if (null != element["LowAlarmValue"]) sensorItem.LowAlarmValue = Convert.ToDouble(element["LowAlarmValue"].InnerText);
                if (null != element["LowWarningValue"]) sensorItem.LowWarningValue = Convert.ToDouble(element["LowWarningValue"].InnerText);
                if (null != element["MaxValue"]) sensorItem.MaxValue = Convert.ToDouble(element["MaxValue"].InnerText);
                if (null != element["MinValue"]) sensorItem.MinValue = Convert.ToDouble(element["MinValue"].InnerText);
                if (null != element["Name"]) sensorItem.Name = element["Name"].InnerText;
                if (null != element["NominalValue"]) sensorItem.NominalValue = Convert.ToDouble(element["NominalValue"].InnerText);
                if (null != element["PersistDataPoints"]) sensorItem.PersistDataPoints = Convert.ToBoolean(element["PersistDataPoints"].InnerText);
                if (null != element["PGN"]) sensorItem.PGN = Convert.ToUInt32(element["PGN"].InnerText);
                if (null != element["PortNumber"]) sensorItem.PortNumber = Convert.ToInt32(element["PortNumber"].InnerText);
                if (null != element["Priority"]) sensorItem.Priority = Convert.ToInt32(element["Priority"].InnerText);
                if (null != element["Resolution"]) sensorItem.Resolution = Convert.ToInt32(element["Resolution"].InnerText);
                if (null != element["SensorType"]) sensorItem.SensorType = (SensorType)Enum.Parse(typeof(SensorType), element["SensorType"].InnerText);
                if (null != element["SensorUnits"]) sensorItem.SensorUnits = (Units)Enum.Parse(typeof(Units), element["SensorUnits"].InnerText);
                if (null != element["SensorUnitType"]) sensorItem.SensorUnitType = (UnitType)Enum.Parse(typeof(UnitType), element["SensorUnitType"].InnerText);
                if (null != element["SensorUsage"]) sensorItem.SensorUsage = (SensorUsage)Enum.Parse(typeof(SensorUsage), element["SensorUsage"].InnerText);
                if (null != element["SerialNumber"]) sensorItem.SerialNumber = element["SerialNumber"].InnerText;
                if (null != element["ShowNominalValue"]) sensorItem.ShowNominalValue = Convert.ToBoolean(element["ShowNominalValue"].InnerText);

                sensorItem.IsEnabled = true;
                sensorItem.IsDemoMode = true;
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

                IGaugePageItem gaugePageItem = new GaugePageItem();
                if (null != element["IsVisible"]) gaugePageItem.IsVisible = Convert.ToBoolean(element["IsVisible"].InnerText);
                if (null != element["PageName"])  gaugePageItem.PageName = element["PageName"].InnerText;
                if (null != element["Position"])  gaugePageItem.Position = Convert.ToInt32(element["Position"].InnerText);

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
                int sensorIndex = Convert.ToInt32(element["SensorIndex"].InnerText);

                IGaugeItem gaugeItem = new GaugeItem(_gaugePageItemList[gaugePageIndex].PageId);
                gaugeItem.SensorId = _sensorItemList[sensorIndex].SensorId;

                if (null != element["GaugeType"])               gaugeItem.GaugeType               = (GaugeTypeEnum) Enum.Parse(typeof(GaugeTypeEnum), element["GaugeType"].InnerText);
                if (null != element["GaugeHeight"])             gaugeItem.GaugeHeight             = Convert.ToDouble(element["GaugeHeight"].InnerText);
                if (null != element["GaugeWidth"])              gaugeItem.GaugeWidth              = Convert.ToDouble(element["GaugeWidth"].InnerText);
                if (null != element["GaugeLeft"])               gaugeItem.GaugeLeft               = Convert.ToDouble(element["GaugeLeft"].InnerText);
                if (null != element["GaugeTop"])                gaugeItem.GaugeTop                = Convert.ToDouble(element["GaugeTop"].InnerText);
                if (null != element["GaugeColor"])              gaugeItem.GaugeColor              = ColorHelper.ToColor(element["GaugeColor"].InnerText);
                if (null != element["Divisions"])               gaugeItem.Divisions               = Convert.ToInt32(element["Divisions"].InnerText);
                if (null != element["MinorTicsPerMajorTic"])    gaugeItem.MinorTicsPerMajorTic    = Convert.ToInt32(element["MinorTicsPerMajorTic"].InnerText);
                if (null != element["MediumTicsPerMajorTic"])   gaugeItem.MediumTicsPerMajorTic   = Convert.ToInt32(element["MediumTicsPerMajorTic"].InnerText);
                if (null != element["Resolution"])              gaugeItem.Resolution              = Convert.ToInt32(element["Resolution"].InnerText);
                if (null != element["GaugeOutlineVisibility"])  gaugeItem.GaugeOutlineVisibility  = (Visibility)Enum.Parse(typeof(Visibility), element["GaugeOutlineVisibility"].InnerText);
                if (null != element["MiddleCircleDelta"])       gaugeItem.MiddleCircleDelta       = Convert.ToInt32(element["MiddleCircleDelta"].InnerText);
                if (null != element["InnerCircleDelta"])        gaugeItem.InnerCircleDelta        = Convert.ToInt32(element["InnerCircleDelta"].InnerText);
                if (null != element["ValueFontSize"])           gaugeItem.ValueFontSize           = Convert.ToDouble(element["ValueFontSize"].InnerText);
                if (null != element["UnitsFontSize"])           gaugeItem.UnitsFontSize           = Convert.ToDouble(element["UnitsFontSize"].InnerText);
                if (null != element["MajorTicLength"])          gaugeItem.MajorTicLength          = Convert.ToDouble(element["MajorTicLength"].InnerText);
                if (null != element["MediumTicLength"])         gaugeItem.MediumTicLength         = Convert.ToDouble(element["MediumTicLength"].InnerText);
                if (null != element["MinorTicLength"])          gaugeItem.MinorTicLength          = Convert.ToDouble(element["MinorTicLength"].InnerText);
                if (null != element["Text"])                    gaugeItem.Text                    = element["Text"].InnerText;
                if (null != element["TextFontSize"])            gaugeItem.TextFontSize            = Convert.ToDouble(element["TextFontSize"].InnerText);
                if (null != element["TextAngle"])               gaugeItem.TextAngle               = Convert.ToDouble(element["TextAngle"].InnerText);
                if (null != element["TextFontColor"])           gaugeItem.TextFontColor           = ColorHelper.ToColor(element["TextFontColor"].InnerText);
                if (null != element["TextHorizontalAlignment"]) gaugeItem.TextHorizontalAlignment = (CanvasHorizontalAlignment) Enum.Parse(typeof(CanvasHorizontalAlignment), element["TextHorizontalAlignment"].InnerText);
                if (null != element["TextVerticalAlignment"])   gaugeItem.TextVerticalAlignment   = (CanvasVerticalAlignment) Enum.Parse(typeof(CanvasVerticalAlignment), element["TextVerticalAlignment"].InnerText);
                if (null != element["Units"])                   gaugeItem.Units                   = (Units) Enum.Parse(typeof(Units), element["Units"].InnerText);

                gaugeItemList.Add(gaugeItem);
            }

            return gaugeItemList;
        }


        private string XmlUrl { get; set;}
    }
}
