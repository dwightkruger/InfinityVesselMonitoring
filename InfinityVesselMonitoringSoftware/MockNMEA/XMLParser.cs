//////////////////////////////////////////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using System;
using System.IO;
using System.Xml;
using Windows.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using InfinityGroup.VesselMonitoring.Interfaces;

namespace InfinityVesselMonitoringSoftware.MockNMEA
{
    public class XMLParser
    {
        public XMLParser(string xmlUrl)
        {
            this.XmlUrl = xmlUrl;
        }

        async public void BeginParse()
        {
            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.XmlUrl));
            Stream stream = await file.OpenStreamForReadAsync();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(stream);

            List<IDeviceItem> deviceItemList = this.ParseDevices(xmlDocument);
        }

        private List<IDeviceItem> ParseDevices(XmlDocument xmlDocument)
        {
            List<IDeviceItem> deviceItemList = new List<IDeviceItem>();

            XmlNodeList deviceListNode = xmlDocument.GetElementsByTagName("Device");
            foreach (XmlNode node in deviceListNode)
            {
                XmlElement element = (XmlElement)node;
                Int64 deviceId = Convert.ToInt64(element["DeviceId"].InnerText);
                string description = element["Description"].InnerText;
                byte deviceAddress = Convert.ToByte(element["DeviceAddress"].InnerText);
                DeviceType deviceType = (DeviceType) Enum.Parse(typeof(DeviceType), element["DeviceType"].InnerText);
                string firmwareVersion = element["FirmwareVersion"].InnerText;
                string hardwareVersion = element["HardwareVersion"].InnerText;
                bool isVirtual = Convert.ToBoolean(element["IsVirtual"].InnerText);
                string IPAddress = element["IPAddress"].InnerText;
                string location = element["Location"].InnerText;
                string model = element["Model"].InnerText;
                string name = element["Name"].InnerText;
                string manufacturer = element["Manufacturer"].InnerText;
                string serialNumber = element["SerialNumber"].InnerText;
                string softwareVersion = element["SoftwareVersion"].InnerText;
            }

            return deviceItemList;
        }
        private string XmlUrl { get; set;}
    }
}
