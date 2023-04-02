using amethyst_installer_gui.PInvoke;

namespace amethyst_installer_gui.Installer {
    public static class PSMSUtil {

        private static DeviceTree s_deviceTree;

        private static void TryGetDeviceTree() {
            if ( s_deviceTree == null )
                s_deviceTree = new DeviceTree();
        }

        public static bool IsLibusbDriverPresent() {
            TryGetDeviceTree();

            // Get Devices
            foreach ( var device in s_deviceTree.DeviceNodes ) {

                // Device is a Kinect 360 Device
                if ( ( device.GetProperty(DevRegProperty.FriendlyName)  == "USB Camera-B?.??.??.?*"     && device.ClassGuid == DeviceClasses.Media ) ||
                    ( device.GetProperty(DevRegProperty.FriendlyName)   == "USB Camera-B?.??.??.?*"     && device.ClassGuid == DeviceClasses.USBDevice ) ||
                    ( device.GetProperty(DevRegProperty.FriendlyName)   == "USB Playstation Eye Camera" && device.ClassGuid == DeviceClasses.USBDevice )
                    ) {

                    return true;
                }
            }

            return false;
        }
    }
}