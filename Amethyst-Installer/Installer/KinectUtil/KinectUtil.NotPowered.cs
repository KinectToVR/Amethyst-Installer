using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json.Linq;
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

        private static DeviceTree s_deviceTree;

        #region Not Powered Fix

        public static bool FixNotPowered() {

            // An automagic fix for E_NUI_NOTPOWERED
            // This fix involves practically uninstalling all Kinect Drivers, using P/Invoke, then forcing a scan for hardware changes to trigger the drivers to re-scan

            // The method involved with fixing this is subject to change at this point

            // TODO: For each unrecognised Xbox 360 device in XX, try uninstall then scan for hardware changes

            bool success = true;

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.Unknown) ) {

                // Device is a Kinect 360 Device
                if (
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"       || // Kinect for Windows Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_00" || // Kinect for Windows Audio Array
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_01" || // Kinect for Windows Security Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02AE&REV_010;"          // Kinect for Windows Camera
                    ) {

                    Logger.Info($"Found faulty Kinect device!  {{ Name: {device.Description} }}");
                    Logger.Info($"Attemping to fix device {device.Description}...");

                    success = success && device.UninstallDevice();
                }
            }

            // Scan for hardware changes
            return success && s_deviceTree.RescanDevices();
        }

        public static bool MustFixNotPowered() {

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.Unknown) ) {

                // Device is a Kinect 360 Device
                if (
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"       || // Kinect for Windows Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_00" || // Kinect for Windows Audio Array
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02BB&REV_0100&MI_01" || // Kinect for Windows Security Device
                    device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02AE&REV_010;"          // Kinect for Windows Camera
                    ) {
                    
                    Logger.Info($"Found broken Kinect device: Name: {device.FriendlyName}; Location: {device.LocationInfo}; Description: {device.Description}");
                    return true;
                }
            }

            return false;
        }

        private static void TryGetDeviceTree() {
            if ( s_deviceTree == null)
                s_deviceTree = new DeviceTree();
        }

        #endregion
    }
}
