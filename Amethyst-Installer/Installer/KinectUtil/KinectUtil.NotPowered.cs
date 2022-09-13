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

        // P/Invoke fuckery to check Device Nodes

        // Checking if the current Kinect Drivers setup is fucked::
        // 1. SetupDiGetClassDevs => fuckery to get up to => Some Kinect Device::Check Status
        //              OR
        // 2. CM_Get_DevNode_Registry_Property_Ex type shit for each device => if class == GUID {0} and description includes "Kinect" probably E_NUI_NOTPOWERED
        //        OR count number of devices under Kinect Device class GUID

        // Removing drivers::
        //     (effectively "Uninstall Device" without deleting the software
        //     CM_Uninstall_DevNode
        //
        // Removing corrupt drivers::
        //     (effectively "Uninstall Device" AND deleting the software
        //     DiUninstallDevice followed by SetupUninstallOEMInf
        //
        // need to talk with Ella about this bit

        #region Not Powered Fix

        public static bool FixNotPowered() {

            // An automagic fix for E_NUI_NOTPOWERED
            // This fix involves practically uninstalling all Unknown Kinect Drivers, using P/Invoke, then forcing a scan for hardware changes to trigger the drivers to re-scan

            // The method involved with fixing this is subject to change at this point

            bool success = true;

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.Unknown) ) {

                // Device is a Kinect 360 Device
                if (device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"       || // Kinect for Windows Device
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
                if (device.DeviceProperties[( int ) DevRegProperty.HardwareId] == "USB\\VID_045E&PID_02B0&REV_0107"       || // Kinect for Windows Device
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
