using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                Util.ShowMessageBox(Localisation.MustDisableMemoryIntegrity_Description, Localisation.MustDisableMemoryIntegrity_Title);

                // Open Windows Security on the Core Isolation page
                Process.Start("windowsdefender://coreisolation");
            }

            // Attempt to reinstall the Kinect Drivers

            // Force enable all known Kinect devices

            // Check again

            // If it persists, pnputil!

            // Once we have silent installations, chain load the installer into a silent install with a callback to this if necessary
        }

        public static bool MustFixNotReady() {

            // Load Kinect10.dll (if installed and check for E_NUI_NOTREADY)
            // It should be in System32 as the user should've installed it to reach this point
            // string kinect10dllPath = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "System32", "Kinect10.dll"));

            if ( KinectSensor.KinectSensors.Count > 0 ) {
                KinectSensor kinect = KinectSensor.KinectSensors[0];
                return kinect.Status == KinectStatus.NotReady;
            }

            return false;
        }

        private static bool IsMemoryIntegrityEnabled () {

            // Check whether memory integrity is enabled or not in Windows Defender / Windows Security via the registry
            // https://docs.microsoft.com/en-us/windows/security/threat-protection/device-guard/enable-virtualization-based-protection-of-code-integrity



            return false;
        }
    }
}
