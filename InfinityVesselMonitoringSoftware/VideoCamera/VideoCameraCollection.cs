/////////////////////////////////////////////////////////////////
//                                                                                                  //
// Copyright (c) 2017 Dwight Kruger and Infinity Group, All rights reserved.                        //
//                                                                                                  //
//////////////////////////////////////////////////////////////////////////////////////////////////////     

using GalaSoft.MvvmLight.Threading;
using InfinityGroup.VesselMonitoring.Interfaces;
using Rssdp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using VesselMonitoringSuite.Devices;
using VesselMonitoringSuite.Sensors;

namespace InfinityVesselMonitoringSoftware.VideoCamera
{
    public class VideoCameraCollection : ObservableCollection<IVideoCamera>
    {
        private SsdpDeviceLocator _deviceLocator;

        public VideoCameraCollection()
        {
        }

        /// <summary>
        /// See the UPnP documentation at http://upnp.org/specs/arch/UPnP-arch-DeviceArchitecture-v1.1.pdf
        /// UPnP library documetation is available at https://github.com/Yortw/RSSDP
        /// </summary>
        /// <returns></returns>
        async public Task Start()
        {
            _deviceLocator = new SsdpDeviceLocator();
            //_deviceLocator.NotificationFilter = "upnp:rootdevice";
            //_deviceLocator.NotificationFilter = "urn:schemas-upnp-org:device:Basic:1";
            _deviceLocator.NotificationFilter = "ssdp:all";
            _deviceLocator.DeviceAvailable += DeviceLocator_DeviceAvailable;
            _deviceLocator.StartListeningForNotifications();
            IEnumerable<Discovered​Ssdp​Device> devices = await _deviceLocator.SearchAsync(TimeSpan.FromSeconds(30));
        }

        public void Stop()
        {
            if (null != _deviceLocator)
            {
                _deviceLocator.StopListeningForNotifications();
                _deviceLocator.Dispose();
                _deviceLocator = null;
            }
        }

        async void DeviceLocator_DeviceAvailable(object sender, DeviceAvailableEventArgs e)
        {
            //Device data returned only contains basic device details and location of full device description.
            Debug.WriteLine("UPnP Found: " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString());

            //Can retrieve the full device description easily though.
            SsdpDevice fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            Debug.WriteLine("\t" + fullDevice.FriendlyName);
            Debug.WriteLine("");

            // If we don't have a serial number for this device, we need to construct one.
            string serialNumber = 
                fullDevice.Uuid + "|" + 
                fullDevice.ModelDescription + "|" + 
                fullDevice.ModelName + "|" + 
                fullDevice.ModelNumber + "|" + 
                fullDevice.ModelUrl + "|" + 
                fullDevice.PresentationUrl;
            if (null != fullDevice.SerialNumber)
            {
                serialNumber = fullDevice.SerialNumber;
            }

            ISensorItem videoCameraSensorItem = null;
            IDeviceItem videoCameraDeviceItem = await App.DeviceCollection.BeginFindBySerialNumber(serialNumber);

            if (null == videoCameraDeviceItem)
            {
                // No device was found to contain this camera. Build the device and the sensor. 
                videoCameraDeviceItem = await this.CreateVideoDevice(fullDevice, serialNumber);
                Debug.Assert(null != videoCameraDeviceItem);
                videoCameraSensorItem = await this.CreateVideoSensor(fullDevice, serialNumber, videoCameraDeviceItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else if ((null != fullDevice.PresentationUrl) && 
                     (videoCameraDeviceItem.IPAddress != fullDevice.PresentationUrl.ToString()))
            {
                // The IP address of the camera changed. We need to refresh the device.
                await App.DeviceCollection.BeginDelete(videoCameraDeviceItem);

                videoCameraDeviceItem = await this.CreateVideoDevice(fullDevice, serialNumber);
                Debug.Assert(null != videoCameraDeviceItem);
                videoCameraSensorItem = await this.CreateVideoSensor(fullDevice, serialNumber, videoCameraDeviceItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else if (videoCameraDeviceItem.Sensors.Count == 0)
            {
                // We have a device to hold the camera, but no camera sensor. We need to build the sensor.
                videoCameraSensorItem = await this.CreateVideoSensor(fullDevice, serialNumber, videoCameraDeviceItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else
            {
                videoCameraDeviceItem.IsOnline = true;
            }
        }

        async private Task<IDeviceItem> CreateVideoDevice(
            SsdpDevice fullDevice, 
            string serialNumber)
        {
            IDeviceItem device = null;

            // We need to do this in the UI thread since it may cause some UI elements to be updated.
            await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () => 
            {
                device = new DeviceItem()
                {
                    Description = (fullDevice.FriendlyName ?? string.Empty),
                    DeviceType = DeviceType.IPCamera,
                    FirmwareVersion = fullDevice.DeviceVersion.ToString(),
                    IPAddress = (null != fullDevice.PresentationUrl ? fullDevice.PresentationUrl.ToString() : string.Empty),
                    IsOnline = true,
                    Location = "Video camera location", // BUGBUG - localize this.
                    Manufacturer = fullDevice.Manufacturer,
                    Model = (fullDevice.ModelNumber ?? string.Empty),
                    Name = (fullDevice.ModelName ?? string.Empty),
                    IsVirtual = false,
                    SerialNumber = serialNumber,
                };

                device = await App.DeviceCollection.BeginAdd(device);
            });

            Debug.Assert(null != device);

            return device;
        }

        async private Task<ISensorItem> CreateVideoSensor(
            SsdpDevice fullDevice, 
            string serialNumber,
            IDeviceItem videoCameraDeviceItem)
        {
            ISensorItem sensor = null;

            // We need to do this in the UI thread since it may cause some UI elements to be updated.
            await DispatcherHelper.UIDispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                sensor = new SensorItem(videoCameraDeviceItem.DeviceId)
                {
                    Description  = (fullDevice.FriendlyName ?? string.Empty),
                    IsEnabled    = true,
                    SensorType   = SensorType.VideoCamera,
                    SensorUsage  = SensorUsage.Other,
                    SerialNumber = serialNumber,
                };

                sensor = await App.SensorCollection.BeginAdd(sensor);
            });

            Debug.Assert(null != sensor);

            return sensor;
        }
    }
}
