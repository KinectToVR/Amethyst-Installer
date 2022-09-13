using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static partial class KinectUtil {

        #region Drivers

        /// <summary>
        /// Returns whether an Xbox 360 Kinect is plugged in to the current machine
        /// </summary>
        public static bool IsKinectV1Present() {

            TryGetDeviceTree();

            // Get Devices
            foreach ( var device in s_deviceTree.DeviceNodes) {

                // Device is a Kinect 360 Device
                if (device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"         || // Kinect for Windows Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_00"   || // Kinect for Windows Audio Array
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_01"   || // Kinect for Windows Security Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02AE&REV_010;"            // Kinect for Windows Camera
                    ) {

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether an Xbox 360 Kinect is plugged in to the current machine and receiving power
        /// </summary>
        public static bool IsKinectV1Powered() {

            TryGetDeviceTree();

            int devices = 0;

            // Get Devices
            foreach ( var device in s_deviceTree.DeviceNodes) {

                // Device is a Kinect 360 Device
                if (device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"         || // Kinect for Windows Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_00"   || // Kinect for Windows Audio Array
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_01"   || // Kinect for Windows Security Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02AE&REV_010;"            // Kinect for Windows Camera
                    ) {

                    devices++;
                }
            }

            return devices > 3;
        }

        /// <summary>
        /// Returns whether an Xbox One Kinect is plugged in to the current machine
        /// </summary>
        public static bool IsKinectV2Present() {

            TryGetDeviceTree();

            // Get Devices
            foreach ( var device in s_deviceTree.DeviceNodes ) {

                // Device is a Xbox One Kinect Device
                if (device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02D8&REV_0100&MI_00"   || // WDF KinectSensor Interface 0
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02D8&MI_00"            || // WDF KinectSensor Interface 0
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02D8&REV_0100&MI_02"   || // Xbox One Kinect Audio Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02D8&MI_02"               // Xbox One Kinect Audio Device
                    ) {

                    return true;
                }
            }

            return false;
        }

        #endregion
    }
}
