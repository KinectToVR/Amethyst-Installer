using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static class KinectUtil {

        #region Microphone

        const string KinectV1MicrophoneFriendlyName = "Kinect USB Audio";
        const string KinectV2MicrophoneFriendlyName = "Xbox NUI Sensor";

        /// <summary>
        /// Check if the Kinect 360 microphone is muted
        /// </summary>
        public static bool KinectV1MicrophoneDisabled() {
            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All) ) {
                    // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                    if ( wasapi.State == DeviceState.NotPresent )
                        continue;
                    // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                    if ( wasapi.DataFlow == DataFlow.Render )
                        continue;

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
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All) ) {
                    // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                    if ( wasapi.State == DeviceState.NotPresent )
                        continue;
                    // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                    if ( wasapi.DataFlow == DataFlow.Render )
                        continue;

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
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All) ) {
                    // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                    if ( wasapi.State == DeviceState.NotPresent )
                        continue;
                    // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                    if ( wasapi.DataFlow == DataFlow.Render )
                        continue;

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
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All) ) {
                    // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                    if ( wasapi.State == DeviceState.NotPresent )
                        continue;
                    // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                    if ( wasapi.DataFlow == DataFlow.Render )
                        continue;

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

        #endregion

        // TODO: Detect SDKs
    }
}
