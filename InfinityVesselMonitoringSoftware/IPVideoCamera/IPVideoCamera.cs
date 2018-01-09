using Windows.Devices.Enumeration;

namespace InfinityVesselMonitoringSoftware.IPVideoCamera
{
    public class IPVideoCamera
    {
        private DeviceWatcher _watcher;
        private const string c_UPnP_ID = "0e261de4-12f0-46e6-91ba-428607ccef64";

        public IPVideoCamera()
        {
            _watcher = Windows.Devices.Enumeration.DeviceInformation.CreateWatcher();
            _watcher.Updated += Watcher_Updated;
        }

        private void Watcher_Updated(Windows.Devices.Enumeration.DeviceWatcher sender, Windows.Devices.Enumeration.DeviceInformationUpdate args)
        {
            
        }

        public void Start()
        {
            _watcher.Start();
        }
    }
}
