using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using amethyst_installer_gui.PInvoke;
using Microsoft.Kinect;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static partial class KinectUtil {

        // E_NUI_NOTREADY means that the Kinect drivers couldn't load properly for whatever reason
        // Usually the fixes range from:
        // 1. Disable Memory Integrity (it is incompatible with certain drivers, which include the Kinect drivers)
        // 2. Check if the microphone is disabled, and enable it if necessary
        // 3. Uninstall all Kinect drivers from Device Manager, then uninstall the drivers via the MSI, then reinstall them
        // 4. Check for a missing driver - in some scenarios some drivers would install and others would be missing ; from our testing
        //    this usually means that the microphone driver is missing. (We are not sure regarding how to fix this yet)

        public static void FixNotReady() {
            
            // Try fixing the microphone
            KinectUtil.FixMicrophoneV1();

            // Check if Memory Integrity is enabled, and if so, early exit
            // (We don't know if this is going to be fixed until we restart the machine, let the user know that a restart is required!)
            if (IsMemoryIntegrityEnabled()) {
                Util.ShowMessageBox(Localisation.Manager.MustDisableMemoryIntegrity_Description, Localisation.Manager.MustDisableMemoryIntegrity_Title);

                // Open Windows Security on the Core Isolation page
                Process.Start("windowsdefender://coreisolation");
            }

            // I hate the Kinect drivers WHY DOES THIS HAPPEN
            AssignGenericAudioDriver();

            // Check if we installed the SDK, we need the files to dump the driver files from to reinstall them manually
            bool hasInstalledKinectSDK = false;
            for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {
                switch ( InstallerStateManager.ModulesToInstall[i].Id ) {
                    case "kinect-v1-sdk":
                        hasInstalledKinectSDK = true;
                        break;
                }
            }
            if ( hasInstalledKinectSDK ) {
                // Attempt to manually install all the Kinect Drivers
                
                // Setup a temp directory for the currently staged driver
                

                DumpKinectDriverFiles();

                // Install drivers manually

            }

            // Force enable all known Kinect devices

            // Check again

            // If it persists, pnputil!

            // Once we have silent installations, chain load the installer into a silent install with a callback to this if necessary
        }

        private static void DumpKinectDriverFiles() {

        }

        public static bool MustFixNotReady() {

            // Load Kinect10.dll (if installed and check for E_NUI_NOTREADY)
            // It should be in System32 as the user should've installed it to reach this point
            // string kinect10dllPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Kinect10.dll"));

            TryGetDeviceTree();

            int kinectDevices = 0;

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.KinectForWindows) ) {
                if ( device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02B0&REV_0107"         || // Kinect for Windows Device
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_00"    || // Kinect for Windows Audio Array
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_01"    || // Kinect for Windows Security Device
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02AE&REV_010;"             // Kinect for Windows Camera
                    ) {
                    kinectDevices++;
                }
            }

            // Check if less than 3 drivers have installed successfully
            if ( kinectDevices < 4 ) {
                return true;
            }

            if ( KinectSensor.KinectSensors.Count > 0 ) {
                KinectSensor kinect = KinectSensor.KinectSensors[0];
                return kinect.Status == KinectStatus.NotReady;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMemoryIntegrityEnabled () {

            // Check whether memory integrity is enabled or not in Windows Defender / Windows Security.
            // We use NtQuerySystemInformation to get an accurate measurement, as despite the MSDN docs recommending you
            // to use the registry, the registry is not reliable (the value might not exist, and the memory isolation
            // state is independent from the registry. The registry value only serves as an override).
            return NtDll.IsCodeIntegrityEnabled();
        }

        public static void AssignGenericAudioDriver() {

            // Somehow the Microphone driver can be missing (I have no fucking clue why this happens, but it happens)
            // The microphone uses "(Generic USB Audio)", which is provided by Microsoft.
            // We do NOT want to bundle the driver files with the installer for multiple reasons:
            // 1. That is really suspicious ඞ
            // 2. The drivers on my machine most likely will not be compatible with an end user's machine
            // 3. Since this is a *generic* audio driver, it is bound to break other devices. I do not want to deal with
            //    that
            // 4. The license (and probably something in the Windows TOS and EULA which prohibits us form doing this too)
            // 
            // Therefore what we tell users to do is to keep updating Windows until it one day decides to download the 
            // (Generic USB Audio) drive from Microsoft's servers, which only then fixes the Kinect.
            // 
            // So basically what we do instead is check if the driver is missing, if so, we tell the Windows Update API to
            // search for the driver, and if found, download it.
            // 
            // AHAHA YOU THOUGHT IT WOULD BE EASY RIGHT?
            // WRONG!
            // 
            // WE'RE DEALING WITH KINECT DRIVERS HERE
            // TURNS OUT:
            // - THE AUDIO DRIVER THE KINECT USES IS BUNDLED WITH ALL WINDOWS INSTALLS!
            // - THE INF FILE CAN BE FOUND AT %SYSTEMROOT%\INF\wdma_usb.inf
            // SO NEW COURSE OF ACTION:
            // 1. Locate the desired audio device
            // 2. Enumerate all drivers, and select the wdma_usb.inf one titled (Generic USB Audio)
            // 3. Assign it to the device
            // 4. Pray and hope Windows doesn't shit itself

            // Fetch device instance ID
            string instanceId = string.Empty;

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes ) {
                if ( device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_02" ) {
                    instanceId = device.GetInstanceId();
                    break;
                }
            }

            SetupApi.AssignExistingDriverViaInfToDeviceId(instanceId, "wdma_usb.inf", "(Generic USB Audio)", "USB Audio Device");
        }
        
        // Assigns a driver to a device programmatically
        public static bool AssignDriverToDeviceId(string deviceID, string infPath) {

            // Fetch device instance ID
            string instanceId = string.Empty;

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes ) {
                if ( device.GetProperty(DevRegProperty.HardwareId) == deviceID ) {
                    instanceId = device.GetInstanceId();
                    break;
                }
            }

            return SetupApi.AssignDriverToDeviceId(instanceId, infPath);
        }

        public static bool PreFixUnknownDevices() {


            // An automagic fix for E_NUI_NOTPOWERED
            // This fix involves practically uninstalling all Unknown Kinect Drivers, using P/Invoke, then forcing a scan for hardware changes to trigger the drivers to re-scan

            // The method involved with fixing this is subject to change at this point

            bool success = true;

            TryGetDeviceTree();

            // Get Kinect Devices
            foreach ( var device in s_deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.Unknown) ) {

                // Device is a Kinect 360 Device
                if ( device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02B0&REV_0107"         || // Kinect for Windows Device
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_00"    || // Kinect for Windows Audio Array
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_01"    || // Kinect for Windows Security Device
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02AE&REV_010;"          || // Kinect for Windows Camera
                    device.GetProperty(DevRegProperty.HardwareId) == "USB\\VID_045E&PID_02BB&REV_0100&MI_02"       // Kinect USB Audio
                    ) {

                    Logger.Info($"Found faulty Kinect device!  {{ Name: {device.Description} }}");
                    Logger.Info($"Attemping to fix device {device.Description}...");

                    success = success && device.UninstallDevice();
                }
            }

            return success;
        }

        public static bool RescanDevices() {
            TryGetDeviceTree();
            return s_deviceTree.RescanDevices();
        }
    }
}
