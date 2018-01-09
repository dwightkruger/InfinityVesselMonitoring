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
        /// </summary>
        /// <returns></returns>
        async public Task Start()
        {
            _deviceLocator = new SsdpDeviceLocator();
            //_deviceLocator.NotificationFilter = "upnp:rootdevice";
            _deviceLocator.NotificationFilter = "urn:schemas-upnp-org:device:Basic:1";
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
            //Console.WriteLine("Found " + e.DiscoveredDevice.Usn + " at " + e.DiscoveredDevice.DescriptionLocation.ToString());

            //Can retrieve the full device description easily though.
            SsdpDevice fullDevice = await e.DiscoveredDevice.GetDeviceInfo();
            //Console.WriteLine(fullDevice.FriendlyName);
            //Console.WriteLine();

            ISensorItem videoCameraSensorItem = null;
            IDeviceItem videoCameraDeviceItem = await App.DeviceCollection.BeginFindBySerialNumber(fullDevice.SerialNumber);
            if (null == videoCameraDeviceItem)
            {
                // No device was found to contain this camera. Build the device and the sensor. 
                this.CreateVideoDevice(fullDevice, ref videoCameraDeviceItem);
                this.CreateVideoSensor(fullDevice, videoCameraDeviceItem, ref videoCameraSensorItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else if (videoCameraDeviceItem.IPAddress != fullDevice.PresentationUrl.ToString())
            {
                // The IP address of the camera changed. We need to refresh the device.
                await App.DeviceCollection.BeginDelete(videoCameraDeviceItem);

                this.CreateVideoDevice(fullDevice, ref videoCameraDeviceItem);
                this.CreateVideoSensor(fullDevice, videoCameraDeviceItem, ref videoCameraSensorItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else if (videoCameraDeviceItem.Sensors.Count == 0)
            {
                // We have a device to hold the camera, but no camera sensor. We need to build the sensor.
                this.CreateVideoSensor(fullDevice, videoCameraDeviceItem, ref videoCameraSensorItem);
                Debug.Assert(null != videoCameraSensorItem);
            }
            else
            {
                videoCameraDeviceItem.IsOnline = true;
            }
        }

        private void CreateVideoDevice(
            SsdpDevice fullDevice, 
            ref IDeviceItem videoCameraDeviceItem)
        {
            IDeviceItem device = null;

            // We need to do this in the UI thread since it may cause some UI elements to be updated.
            DispatcherHelper.CheckBeginInvokeOnUI(() => 
            {
                Task.Run(async () =>
                {
                    device = new DeviceItem()
                    {
                        Description = fullDevice.FriendlyName,
                        DeviceType = DeviceType.IPCamera,
                        FirmwareVersion = fullDevice.DeviceVersion.ToString(),
                        IPAddress = fullDevice.PresentationUrl.ToString(),
                        IsOnline = true,
                        Location = "Video camera location", // BUGBUG - localize this.
                        Manufacturer = fullDevice.Manufacturer,
                        Model = fullDevice.ModelNumber,
                        Name = fullDevice.ModelName,
                        IsVirtual = false,
                        SerialNumber = fullDevice.SerialNumber,
                    };

                    device = await App.DeviceCollection.BeginAdd(device);
                }).Wait();
            });

            videoCameraDeviceItem = device;
        }

        private void CreateVideoSensor(
            SsdpDevice fullDevice, 
            IDeviceItem videoCameraDeviceItem, 
            ref ISensorItem videoCameraSensor)
        {
            ISensorItem sensor = null;

            // We need to do this in the UI thread since it may cause some UI elements to be updated.
            DispatcherHelper.CheckBeginInvokeOnUI(() =>
            {
                Task.Run(async () =>
                {
                    sensor = new SensorItem(videoCameraDeviceItem.DeviceId)
                    {
                        Description  = fullDevice.FriendlyName,
                        IsEnabled    = true,
                        SensorType   = SensorType.VideoCamera,
                        SensorUsage  = SensorUsage.Other,
                        SerialNumber = fullDevice.SerialNumber,
                    };

                    sensor = await App.SensorCollection.BeginAdd(sensor);
                }).Wait();
            });

            videoCameraSensor = sensor;
        }
    }
}
