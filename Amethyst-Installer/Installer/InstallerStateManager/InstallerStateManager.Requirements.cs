using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
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

        // Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsCloudPC = false;
        public static bool IsWindowsAncient = false;
        // Check if the C drive has enough storage to let the installer work
        public static bool HasEnoughStorage = false;

        /// <summary>
        /// Whether K2EX was found on the system, so that we remove it
        /// </summary>
        public static bool K2EXDetected = true;
        public static string K2EXPath = string.Empty;

        public static Vec2 PlayspaceBounds = Vec2.Zero;

        public static List<UsbControllerData> UsbControllers = new List<UsbControllerData>();

        private static void ComputeRequirements() {

            // @TODO: actually compute the requirements for installing amethyst

            CheckStorage();
            CheckSteamVR();
            CheckAmethyst();
            CheckK2EX();
            CheckOS();
            DetectUsbControllers();
            DetectCloudPc();
            DetectLaptop();

            CanInstall = !IsCloudPC && SteamVRInstalled && !IsWindowsAncient && OpenVRUtil.HmdType != VRHmdType.Phone;
        }

        private static void CheckSteamVR() {

            // Detect if we are even able to install Amethyst
            SteamVRInstalled = Directory.Exists(OpenVRUtil.RuntimePath());

            // Determine SteamVR playspace bounds
            PlayspaceBounds = OpenVRUtil.GetPlayspaceBounds();

            Logger.Info($"Detected SteamVR Install at \"{OpenVRUtil.RuntimePath()}\"");
            Logger.Info($"Detected VR headset type {OpenVRUtil.HmdType}, connected using {OpenVRUtil.ConnectionType}; tracking type: {OpenVRUtil.TrackingType}");
            Logger.Info($"Playspace bounds: {PlayspaceBounds}");
        }

        private static void CheckAmethyst() {

            // @TODO: Check for Amethyst
        }
        

        private static void CheckK2EX() {

            // @TODO: Try locating K2EX
            K2EXPath = K2EXUtil.LocateK2EX();
            K2EXDetected = K2EXPath.Length > 0 && Directory.Exists(K2EXPath);
        }

        private static void CheckOS() {
            // If older than Windows 10
            Version osVersion = WindowsUtils.GetVersion();
            if ( osVersion.Major < 10 ) {
                IsWindowsAncient = true;
            }
            // If the user is running a Windows 10 install that's older than 20H2
            if ( osVersion.Build < ( int ) WindowsUtils.WindowsMajorReleases.Win10_20H2 ) {
                IsWindowsAncient = true;
            }
            Logger.Info($"OS too old: {IsWindowsAncient} ; OS Version: {osVersion} ; Version String: {WindowsUtils.GetDisplayVersion()}");
        }

        private static void DetectUsbControllers() {
            // @TODO: USB controller shit

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

        private static void DetectCloudPc() {
            bool isShadow   = CloudPCUtil.IsRunningOnShadow();
            bool isPluto    = CloudPCUtil.IsOnPlutoSphere();
            bool isNBVR     = CloudPCUtil.IsOnNBVR();

            IsCloudPC = isPluto || isShadow || isNBVR;
            if ( isPluto ) {
                Logger.Info($"Detected Plutosphere! Amethyst does not work in a networked environment!");
            }
            if ( isShadow ) {
                Logger.Info($"Detected Shadow! Amethyst does not work in a networked environment!");
            }
            if ( isNBVR ) {
                Logger.Info($"Detected NBVR! Amethyst does not work in a networked environment!");
            }
        }

        private static void DetectLaptop() {
            // @TODO: YEET!
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

        private static void CheckStorage() {

            DriveInfo primaryDrive = null;

            var drives = DriveInfo.GetDrives();
            var systemDriveLetter = Path.GetPathRoot( Environment.GetFolderPath( Environment.SpecialFolder.Windows ));
            foreach ( var drive in drives ) {
                if (drive.IsReady && drive.RootDirectory.ToString() == systemDriveLetter ) {
                    // Assume default disk
                    primaryDrive = drive;
                    break;
                }
            }
            if ( primaryDrive == null ) {
                primaryDrive = drives[0];
            }

            if ( primaryDrive != null && primaryDrive.IsReady ) {
                // Demand 10GB of free space on the primary drive
                HasEnoughStorage = primaryDrive.AvailableFreeSpace > 10000000000; // 10GB
            } else {
                // It'll get caught elsewhere anyway
                HasEnoughStorage = true;
            }
        }
    }

    public enum UsbControllerQuality {
        Unknown,
        Good,
        OK,
        Unusable,
        Ignore,
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
        public UsbControllerQuality ControllerQuality;

        public UsbControllerData(DeviceNode device) : this() {
            this.Name = device.FriendlyName;
            if (device.FriendlyName.Length == 0) {
                this.Name = device.Description;
            }

            // @TODO: Regex :D
            this.FriendlyString = this.Name;
            this.ControllerQuality = UsbControllerQuality.Unknown;
            /*

RAW DATA:
            
Found USB Controller: Name: AMD USB 3.10 eXtensible Host Controller - 1.10 (Microsoft);         Location: PCI bus 2, device 0, function 0;      Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: Fresco Logic USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);  Location: PCI bus 5, device 0, function 0;      Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: Renesas USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);       Location: PCI bus 9, device 0, function 0;      Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: Renesas USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);       Location: PCI bus 11, device 0, function 0;     Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: AMD USB 3.10 eXtensible Host Controller - 1.10 (Microsoft);         Location: PCI bus 16, device 0, function 3;     Description: USB xHCI Compliant Host Controller

Found USB Controller: Name: Intel(R) USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);      Location: PCI bus 0, device 20, function 0;     Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: ;                                                                   Location: PCI bus 0, device 26, function 0;     Description: Intel(R) 7 Series/C216 Chipset Family USB Enhanced Host Controller - 1E2D
Found USB Controller: Name: Renesas USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);       Location: PCI bus 4, device 0, function 0;      Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: ;                                                                   Location: PCI bus 0, device 29, function 0;     Description: Intel(R) 7 Series/C216 Chipset Family USB Enhanced Host Controller - 1E26

Found USB Controller: Name: AMD USB 3.10 eXtensible Host Controller - 1.10 (Microsoft);         Location: PCI bus 2, device 0, function 0;      Description: Controlador Host Compatível com USB xHCI
Found USB Controller: Name: VIA USB 3.0 eXtensible Host Controller - 1.0 (Microsoft);           Location: PCI bus 4, device 0, function 0;      Description: Controlador Host Compatível com USB xHCI
Found USB Controller: Name: AMD USB 3.10 eXtensible Host Controller - 1.10 (Microsoft);         Location: PCI bus 45, device 0, function 3;     Description: Controlador Host Compatível com USB xHCI

Found USB Controller: Name: NVIDIA USB 3.10 eXtensible Host Controller - 1.10 (Microsoft);      Location: PCI bus 7, device 0, function 2;      Description: USB xHCI Compliant Host Controller
Found USB Controller: Name: NVIDIA USB Type-C Port Policy Controller;                           Location: PCI bus 7, device 0, function 3;      Description: NVIDIA USB Type-C Port Policy Controller

Found USB Controller: Name: Intel(R) USB 3.0 eXtensible-Hostcontroller - 1.0 (Microsoft);       Location: PCI-Bus 0, Gerät 20, Funktion 0;      Description: USB-xHCI-kompatibler Hostcontroller
Found USB Controller: Name: ASMedia USB 3.1 eXtensible-Hostcontroller - 1.10 (Microsoft);       Location: PCI-Bus 4, Gerät 0, Funktion 0;       Description: USB-xHCI-kompatibler Hostcontroller

             */

            /*
            
CLEANED-UP:

AMD USB 3.10 eXtensible Host Controller - 1.10 (Microsoft)                  #
Fresco Logic USB 3.0 eXtensible Host Controller - 1.0 (Microsoft)           #
Renesas USB 3.0 eXtensible Host Controller - 1.0 (Microsoft)                #
Intel(R) USB 3.0 eXtensible Host Controller - 1.0 (Microsoft)               #
Intel(R) 7 Series/C216 Chipset Family USB Enhanced Host Controller - 1E2D   
Intel(R) 7 Series/C216 Chipset Family USB Enhanced Host Controller - 1E26   
VIA USB 3.0 eXtensible Host Controller - 1.0 (Microsoft)                    #
NVIDIA USB 3.10 eXtensible Host Controller - 1.10 (Microsoft)               #
NVIDIA USB Type-C Port Policy Controller                                    
Intel(R) USB 3.0 eXtensible-Hostcontroller - 1.0 (Microsoft)                #
ASMedia USB 3.1 eXtensible-Hostcontroller - 1.10 (Microsoft)                #

             */

            // Generate a cleaned up string for USB devices

            // Most USB Controllers have the string "(Microsoft)" in their name, handle those first
            // If for some reason we explode (usually non-English languages), fall back to the painful method
            bool hell = false;
            try {
                if ( this.FriendlyString.Contains("(Microsoft)") ) {
                    this.FriendlyString = this.FriendlyString.Substring(0, this.FriendlyString.IndexOf("eXtensible"));
                } else {
                    hell = true;
                }
            } catch (Exception e) {
                Logger.Fatal(Util.FormatException(e));
                hell = true;
            }
            
            if (hell) {
                // Oh god oh fuck
                // Try being "smart" and cut till we find USB, and include any numerics if the next "word" is a number (i.e. version)
                int lastChar = this.FriendlyString.IndexOf("USB") + 3;
                if ( lastChar != -1 && lastChar < this.FriendlyString.Length ) {
                    int indexBuffer = this.FriendlyString.IndexOf(' ', lastChar + 1);
                    if ( indexBuffer == -1 )
                        indexBuffer = lastChar;
                    else {
                        // Try checking if the word is a number
                        string tmp = this.FriendlyString.Substring(lastChar, indexBuffer - lastChar);
                        int ind = indexBuffer;
                        indexBuffer = lastChar;
                        if ( double.TryParse(tmp.Trim(), out _) ) {
                            indexBuffer = ind;
                        } else if ( tmp.ToLowerInvariant().Contains("type") && tmp.ToLowerInvariant().Contains('c') ) {
                            // Type-C
                            indexBuffer = ind;
                        }
                    }
                    this.FriendlyString = this.FriendlyString.Substring(0, indexBuffer);
                }
            }

            this.FriendlyString = this.FriendlyString.Trim();

            // this.IsGoodController = false;
            // @TODO: Blacklist / whitelist
            // KinectV1_Compatible = false;
            // KinectV2_Compatible = false;

            // This is slow as shit, but I don't see any other way of making this faster and more performant
            ReadOnlySpan<char> usbControllerTitleAsSpan = this.Name.AsSpan();

            if ( usbControllerTitleAsSpan.Contains("intel".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                if ( usbControllerTitleAsSpan.Contains("3.1".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                    usbControllerTitleAsSpan.Contains("3.2".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                    this.ControllerQuality = UsbControllerQuality.Good;
                } else if ( usbControllerTitleAsSpan.Contains("3.0".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                    ( usbControllerTitleAsSpan.Contains("chipset".AsSpan(), StringComparison.InvariantCultureIgnoreCase)
                    && usbControllerTitleAsSpan.Contains("family".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) ) {
                    this.ControllerQuality = UsbControllerQuality.OK;
                }
            } else if ( usbControllerTitleAsSpan.Contains("amd".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                if ( usbControllerTitleAsSpan.Contains("3.1".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                    usbControllerTitleAsSpan.Contains("3.2".AsSpan(), StringComparison.InvariantCultureIgnoreCase)) {
                    this.ControllerQuality = UsbControllerQuality.Good;
                } else if ( usbControllerTitleAsSpan.Contains("3.0".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                    this.ControllerQuality = UsbControllerQuality.Unusable;
                }
            } else if ( usbControllerTitleAsSpan.Contains("asmedia".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                this.ControllerQuality = UsbControllerQuality.Good;
                if ( usbControllerTitleAsSpan.Contains("3.0".AsSpan(), StringComparison.InvariantCultureIgnoreCase)) {
                    this.ControllerQuality = UsbControllerQuality.Unusable;
                }
            } else if ( usbControllerTitleAsSpan.Contains("renesas".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                 usbControllerTitleAsSpan.Contains("nec".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                this.ControllerQuality = UsbControllerQuality.Good;
            } else if ( usbControllerTitleAsSpan.Contains("fresco".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                 usbControllerTitleAsSpan.Contains("via".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                this.ControllerQuality = UsbControllerQuality.Unusable;
            } else if ( usbControllerTitleAsSpan.Contains("nvidia".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                this.ControllerQuality = UsbControllerQuality.Ignore;
            } else if ( usbControllerTitleAsSpan.Contains("openhcd".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ||
                usbControllerTitleAsSpan.Contains("generic".AsSpan(), StringComparison.InvariantCultureIgnoreCase) ) {
                this.ControllerQuality = UsbControllerQuality.OK;
            }
        }
    }
}

