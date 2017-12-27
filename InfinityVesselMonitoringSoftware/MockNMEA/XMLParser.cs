//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using InfinityGroup.VesselMonitoring.Interfaces;
using InfinityGroup.VesselMonitoring.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using VesselMonitoringSuite.Devices;
using VesselMonitoringSuite.Sensors;
using Windows.Storage;

namespace InfinityVesselMonitoringSoftware.MockNMEA
{
    public class XMLParser
    {
        private List<IDeviceItem> _deviceItemList;
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

        private string XmlUrl { get; set;}
    }
}
