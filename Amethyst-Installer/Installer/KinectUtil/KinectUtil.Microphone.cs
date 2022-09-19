using amethyst_installer_gui.PInvoke;
using Microsoft.Win32;
using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static partial class KinectUtil {

        #region Microphone

        const string KinectV1MicrophoneFriendlyName = "Kinect USB Audio";
        const string KinectV2MicrophoneFriendlyName = "Xbox NUI Sensor";
        
        const int DEVICE_ENABLED    = 0x00000001;
        const int DEVICE_DISABLED   = 0x10000001;

        /// <summary>
        /// Check if the Kinect 360 microphone is muted
        /// </summary>
        public static bool KinectV1MicrophoneDisabled() {
            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled | DeviceState.Unplugged) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV1MicrophoneFriendlyName ) {
                        if ( wasapi.State != DeviceState.Active )
                            return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Check if the Kinect One microphone is muted
        /// </summary>
        public static bool KinectV2MicrophoneDisabled() {
            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled | DeviceState.Unplugged) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV2MicrophoneFriendlyName ) {
                        if ( wasapi.State != DeviceState.Active )
                            return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if the any Kinect microphone is muted
        /// </summary>
        public static bool KinectMicrophoneDisabled() {
            return KinectV1MicrophoneDisabled() || KinectV2MicrophoneDisabled();
        }

        /// <summary>
        /// Checks if there is a Kinect 360 microphone registered and available on this system
        /// </summary>
        public static bool KinectV1MicrophonePresent() {
            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled | DeviceState.Unplugged | DeviceState.Active) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV1MicrophoneFriendlyName )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is a Kinect One microphone registered and available on this system
        /// </summary>
        public static bool KinectV2MicrophonePresent() {
            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled | DeviceState.Unplugged | DeviceState.Active) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV2MicrophoneFriendlyName )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if there is any Kinect microphone registered and available on this system
        /// </summary>
        public static bool KinectMicrophonePresent() {
            return KinectV1MicrophonePresent() || KinectV2MicrophonePresent();
        }

        /// <summary>
        /// Attempts to fix a Kinect Microphone
        /// </summary>
        public static bool FixMicrophoneV1() {
            // Fixing an Audio device is hell lmfao

            // EW MANUAL METHOD!!!
            // Util.ShowMessageBox(Localisation.PostOp_Kinect_EnableMic_Description, Localisation.PostOp_Kinect_EnableMic_Title, MessageBoxButton.OK);

            // Open sound control panel on the recording tab
            // Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");

            // Automatic method :Amelia_PewPew:

            // So the device state is stored here in the registry
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture\XXXX
            //
            // Yeah OK you see where this is going now...

            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled | DeviceState.Unplugged | DeviceState.Active) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV1MicrophoneFriendlyName ) {
                        // Grab the GUID so that we don't search the registry
                        string microphoneGUID = wasapi.ID.Substring(wasapi.ID.IndexOf('{', 1));


                        AdvApi.EnablePrivilege("SeTakeOwnershipPrivilege");
                        using ( var audioDeviceReg = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\MMDevices\\Audio\\Capture\\{microphoneGUID}", RegistryKeyPermissionCheck.ReadWriteSubTree) ) {

                            // Take-own
                            AdvApi.EnablePrivilege(audioDeviceReg.Handle.DangerousGetHandle(), "SeTakeOwnershipPrivilege");

                            RegistrySecurity rs = audioDeviceReg.GetAccessControl();
                            string currentUserStr = Environment.UserDomainName + "\\" + Environment.UserName;
                            // rs.AddAccessRule(new RegistryAccessRule(currentUserStr, RegistryRights.WriteKey | RegistryRights.ReadKey | RegistryRights.Delete | RegistryRights.FullControl, AccessControlType.Allow));

                            // var admins = new NTAccount("Administrators");
                            var myAcc = new NTAccount(Environment.UserDomainName, Environment.UserName);
                            var ac = audioDeviceReg.GetAccessControl();
                            ac.SetOwner(myAcc);
                            ac.AddAccessRule(new RegistryAccessRule(myAcc, RegistryRights.ReadKey | RegistryRights.SetValue | RegistryRights.QueryValues | RegistryRights.FullControl, AccessControlType.Allow));
                            audioDeviceReg.SetAccessControl(ac);

                            // Force disable
                            audioDeviceReg.SetValue("DeviceState", DEVICE_ENABLED, RegistryValueKind.DWord);
                        }

                        return true;
                    }
                }
            }

            Logger.Info("Failed to find a valid Xbox 360 Kinect microphone!");
            return false;
        }

        #endregion
    }
}
