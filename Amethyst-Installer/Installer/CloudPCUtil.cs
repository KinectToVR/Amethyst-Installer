using NAudio.CoreAudioApi;
using System;
using System.IO;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// A utility class containing functions to determine if the user is running on a Shadow PC
    /// </summary>
    public static class CloudPCUtil {
        /// <summary>
        /// Returns whether the current machine is a Shadow PC
        /// </summary>
        public static bool IsRunningOnShadow() {
            if ( ShadowVROpenVRDriverExists() )
                return DetectedShadowVirtualAudioDevice();
            
            return false;
        }

        /// <summary>
        /// Shadow machines have an OpenVR driver stored at "C:/Program Files/Blade Group/ShadowVR".
        /// This method checks for it's presence
        /// </summary>
        private static bool ShadowVROpenVRDriverExists() {
            var shadowVRDriverPath = OpenVRUtil.GetDriverPath("ShadowVR");
            return shadowVRDriverPath.Length > 0 && Directory.Exists(shadowVRDriverPath);
        }

        /// <summary>
        /// Shadow machines have a custom virtual audio device they use to stream audio through their network onto the user's client.
        /// This method checks for it's presence
        /// </summary>
        private static bool DetectedShadowVirtualAudioDevice() {
            const string shadowVirtAudioDeviceName = "Shadow Virtual Audio Device";

            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active | DeviceState.Disabled | DeviceState.Unplugged) ) {
                    if ( wasapi.DeviceFriendlyName == shadowVirtAudioDeviceName )
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns whether the current machine is running on PlutoSphere
        /// </summary>
        public static bool IsOnPlutoSphere() {
            // According to the PlutoSphere FAQ, "CloudXRRemoteHMD" is a necessary driver for PlutoSphere
            var plutosphereDriverPath = OpenVRUtil.GetDriverPath("CloudXRRemoteHMD");
            return plutosphereDriverPath.Length > 0 && Directory.Exists(plutosphereDriverPath);
        }

        /// <summary>
        /// Returns whether the current machine is running on NBVR
        /// </summary>
        public static bool IsOnNBVR() {
            var nbvrDriverPath = OpenVRUtil.GetDriverPath("nbvr_server");
            return nbvrDriverPath.Length > 0 && Directory.Exists(nbvrDriverPath);
        }
    }
}
