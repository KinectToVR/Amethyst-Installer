using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static partial class InstallerStateManager {

        /// <summary>
        /// Whether the user can install Amethyst on this system at all.
        /// </summary>
        public static bool CanInstall = false;

        /// <summary>
        /// Whether SteamVR is installed and was found on this system.
        /// </summary>
        public static bool SteamVRInstalled = false;

        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsCloudPC = false;
        public static bool IsWindowsAncient = false;

        /// <summary>
        /// Whether K2EX was found on the system, so that we remove it
        /// </summary>
        public static bool K2EXDetected = true;
        public static string K2EXPath = string.Empty;

        public static Vec2 PlayspaceBounds = Vec2.Zero;

        public static List<UsbControllerData> UsbControllers = new List<UsbControllerData>();

        private static void ComputeRequirements() {

            // TODO: actually compute the requirements for installing amethyst

            // STEAMVR
            {
                // Detect if we are even able to install Amethyst
                SteamVRInstalled = Directory.Exists(OpenVRUtil.RuntimePath());

                // Determine SteamVR playspace bounds
                PlayspaceBounds = OpenVRUtil.GetPlayspaceBounds();

                Logger.Info($"Detected VR headset type {OpenVRUtil.HmdType.ToString()}, connected using {OpenVRUtil.ConnectionType}; tracking type: {OpenVRUtil.TrackingType}");
                Logger.Info($"Playspace bounds: {PlayspaceBounds}");
            }

            // K2EX UPGRADING
            {
                // Try locating K2EX
                K2EXPath = K2EXUtil.LocateK2EX();
                K2EXDetected = K2EXPath.Length > 0 && Directory.Exists(K2EXPath);
            }

            // WINDOWS VERSION GATEKEEP
            {
                // If older than Windows 10
                if ( Environment.OSVersion.Version.Major < 10 ) {
                    IsWindowsAncient = true;
                }
                // If the user is running a Windows 10 install that's older than 20H2
                if ( Environment.OSVersion.Version.Build < ( int ) WindowsUtils.WindowsMajorReleases.Win10_20H2 ) {
                    IsWindowsAncient = true;
                }
                Logger.Info($"OS too old: {IsWindowsAncient} ; OS Version: {Environment.OSVersion.Version} ; Version String: {WindowsUtils.GetDisplayVersion()}");
            }

            // USB CONTROLLERS
            {
                // TODO: USB controller shit

                // Get USB Controller info
                var deviceTree = new DeviceTree();
                // Selects USB devices exclusively
                foreach ( var device in deviceTree.DeviceNodes.Where(d => d.ClassGuid == DeviceClasses.USB) ) {
                    // USB Controllers have their Enumerator property set to PCI, while everything else has Enumerator set to USB
                    if ( device.EnumeratorName == "PCI" ) {
                        Logger.Info($"Found USB Controller: Name: {device.FriendlyName}; Location: {device.LocationInfo}; Description: {device.Description}");
                        UsbControllers.Add(new UsbControllerData(device));
                    }
                }
            }

            // CLOUD PC DETECTION
            {
                bool isPluto = CloudPCUtil.IsOnPlutoSphere();
                bool isShadow = CloudPCUtil.IsRunningOnShadow();
                IsCloudPC = isPluto || isShadow;
                if ( isPluto ) {
                    Logger.Info($"Detected Plutosphere! Amethyst does not work in a networked environment!");
                }
                if ( isShadow ) {
                    Logger.Info($"Detected Shadow! Amethyst does not work in a networked environment!");
                }
            }

            CanInstall = !IsCloudPC && SteamVRInstalled & !IsWindowsAncient;

            // SHIT TO DETECT IF WE'RE USING A LAPTOP
            {
                // TODO: YEET!
                // This is temp, just to see if this is reliable for now
                Logger.Warn($"PowerButtonPresent: {PowerProvider.SystemPowerCapabilites.PowerButtonPresent}");
                Logger.Warn($"SleepButtonPresent: {PowerProvider.SystemPowerCapabilites.SleepButtonPresent}");
                Logger.Warn($"LidPresent: {PowerProvider.SystemPowerCapabilites.LidPresent}");
                Logger.Warn($"SystemS1: {PowerProvider.SystemPowerCapabilites.SystemS1}");
                Logger.Warn($"SystemS2: {PowerProvider.SystemPowerCapabilites.SystemS2}");
                Logger.Warn($"SystemS3: {PowerProvider.SystemPowerCapabilites.SystemS3}");
                Logger.Warn($"SystemS4: {PowerProvider.SystemPowerCapabilites.SystemS4}");
                Logger.Warn($"SystemS5: {PowerProvider.SystemPowerCapabilites.SystemS5}");
                Logger.Warn($"HiberFilePresent: {PowerProvider.SystemPowerCapabilites.HiberFilePresent}");
                Logger.Warn($"FullWake: {PowerProvider.SystemPowerCapabilites.FullWake}");
                Logger.Warn($"VideoDimPresent: {PowerProvider.SystemPowerCapabilites.VideoDimPresent}");
                Logger.Warn($"ApmPresent: {PowerProvider.SystemPowerCapabilites.ApmPresent}");
                Logger.Warn($"UpsPresent: {PowerProvider.SystemPowerCapabilites.UpsPresent}");
                Logger.Warn($"ThermalControl: {PowerProvider.SystemPowerCapabilites.ThermalControl}");
                Logger.Warn($"ProcessorThrottle: {PowerProvider.SystemPowerCapabilites.ProcessorThrottle}");
                Logger.Warn($"ProcessorMinThrottle: {PowerProvider.SystemPowerCapabilites.ProcessorMinThrottle}");
                Logger.Warn($"ProcessorMaxThrottle: {PowerProvider.SystemPowerCapabilites.ProcessorMaxThrottle}");
                Logger.Warn($"FastSystemS4: {PowerProvider.SystemPowerCapabilites.FastSystemS4}");
                Logger.Warn($"Hiberboot: {PowerProvider.SystemPowerCapabilites.Hiberboot}");
                Logger.Warn($"WakeAlarmPresent: {PowerProvider.SystemPowerCapabilites.WakeAlarmPresent}");
                Logger.Warn($"WakeAlarmPresent: {PowerProvider.SystemPowerCapabilites.WakeAlarmPresent}");
                Logger.Warn($"AoAc: {PowerProvider.SystemPowerCapabilites.AoAc}");
                Logger.Warn($"DiskSpinDown: {PowerProvider.SystemPowerCapabilites.DiskSpinDown}");
                Logger.Warn($"HiberFileType: {PowerProvider.SystemPowerCapabilites.HiberFileType}");
                Logger.Warn($"AoAcConnectivitySupported: {PowerProvider.SystemPowerCapabilites.AoAcConnectivitySupported}");
                Logger.Warn($"SystemBatteriesPresent: {PowerProvider.SystemPowerCapabilites.SystemBatteriesPresent}");
                Logger.Warn($"BatteriesAreShortTerm: {PowerProvider.SystemPowerCapabilites.BatteriesAreShortTerm}");
            }
        }

    }

    public struct UsbControllerData {
        /// <summary>
        /// Actual string name as seen in device manager
        /// </summary>
        public string Name;
        /// <summary>
        /// A cleaned up version of the string to display in UI
        /// </summary>
        public string FriendlyString;
        public bool KinectV1_Compatible;
        public bool KinectV2_Compatible;

        public UsbControllerData(DeviceNode device) : this() {
            this.Name = device.FriendlyName;
            if (device.FriendlyName.Length == 0) {
                this.Name = device.Description;
            }

            // TODO: Regex :D
            this.FriendlyString = this.Name;


            // this.IsGoodController = false;
            KinectV1_Compatible = false;
            KinectV2_Compatible = false;
        }
    }
}
