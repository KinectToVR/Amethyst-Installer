using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using amethyst_installer_gui.PInvoke;
using Microsoft.Kinect;
using static amethyst_installer_gui.PInvoke.SetupApi;

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

            // I hate the Kinect drivers WHY DOES THIS HAPPEN
            DownloadAndInstallGenericAudioDriver();

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
            throw new NotImplementedException();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMemoryIntegrityEnabled () {

            // Check whether memory integrity is enabled or not in Windows Defender / Windows Security.
            // We use NtQuerySystemInformation to get an accurate measurement, as despite the MSDN docs recommending you
            // to use the registry, the registry is not reliable (the value might not exist, and the memory isolation
            // state is independent from the registry. The registry value only serves as an override).
            return NtDll.IsCodeIntegrityEnabled();
        }

        public static void DownloadAndInstallGenericAudioDriver() {

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
            // 1. Locate this inf file
            // 2. Locate the device node for the Kinect microphone
            // 3. Tell Windows to use wdma_usb.inf as the driver    
            // 4. Pray and hope Windows doesn't shit itself


            // 
            // InstallHinfSection(NULL,NULL,TEXT("DefaultInstall 132 path-to-inf\infname.inf"),0); 

            
        }

        // Assigns a driver to a device programmatically
        public static bool AssignDriverToDeviceId(string deviceID) {
            // Based on sample at https://web.archive.org/web/20221013072029/https://www.betaarchive.com/wiki/index.php/Microsoft_KB_Archive/889763

            IntPtr DeviceInfoSet = SetupApi.SetupDiCreateDeviceInfoList(IntPtr.Zero, IntPtr.Zero);
            if (DeviceInfoSet == SetupApi.INVALID_HANDLE_VALUE) {
                return false;
            }

            SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);

            return false;
        }
    }
}
