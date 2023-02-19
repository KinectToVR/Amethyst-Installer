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
            // Util.ShowMessageBox(Localisation.Manager.PostOp_Kinect_EnableMic_Description, Localisation.Manager.PostOp_Kinect_EnableMic_Title, MessageBoxButton.OK);

            // Open sound control panel on the recording tab, and tell the user to enable the microphone since we can't simulate the keystrokes
            // ourselves
            // Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");

            // Automatic method :Amelia_PewPew:

            // So the device state is stored here in the registry
            // HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\MMDevices\Audio\Capture\XXXX
            //
            // Yeah OK you see where this is going now...
            // 
            // I pulled your leg it obviously would not be that easy...
            // If you try writing to any key there you will get an exception.
            //
            // So turns out: YOU CAN NOT CHANGE THE STATE OF AUDIO DEVICES PROGRAMMATICALLY USING PUBLIC FACING WINDOWS APIS
            // WANNA DO IT ANYWAY? GO REVERSE ENGINEER THE PRIVATE API!!
            //
            // Now you would say, why don't we open the control panel one, and use cursed Windows API stuff to automatically send all the
            // keystrokes necessary to disable the microphone programmatically?
            // 
            // Yeah no you can't get the window handles in Windows 11 (I haven't checked on 10, but probably the same thing) :D
            // 
            // So yeah let's just reverse engineer the Windows API and abuse COM to import the PolicyStore and change the AudioEndpointVisibility
            // to true :D
            //
            // Please send help I spent 3 whole days to get to this point...

            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Disabled) ) {
                    if ( wasapi.DeviceFriendlyName == KinectV1MicrophoneFriendlyName ) {
                        // Private windows API jumpscare
                        DevicePolicy.SetAudioEndpointState(wasapi.ID, true);
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
