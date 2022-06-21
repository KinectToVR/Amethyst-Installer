using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// A utility class containing functions to determine if the user is running on a Shadow PC
    /// </summary>
    public static class CloudPCUtil {
        /// <summary>
        /// Returns whether the current machine is a Shadow PC
        /// </summary>
        public static bool IsRunningOnShadow() {
            bool ShadowVRDriverFound = ShadowVROpenVRDriverExists();
            bool ShadowAudioDriverFound = DetectedShadowVirtualAudioDevice();

            return ShadowVRDriverFound && ShadowAudioDriverFound;
        }

        /// <summary>
        /// Shadow machines have an OpenVR driver stored at "C:/Program Files/Blade Group/ShadowVR".
        /// This method checks for it's presence
        /// </summary>
        private static bool ShadowVROpenVRDriverExists() {
            string ShadowVRPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Blade Group", "ShadowVR");
            Logger.Info(ShadowVRPath);
            return Directory.Exists(ShadowVRPath);
        }

        /// <summary>
        /// Shadow machines have a custom virtual audio device they use to stream audio through their network onto the user's client.
        /// This method checks for it's presence
        /// </summary>
        private static bool DetectedShadowVirtualAudioDevice() {
            const string shadowVirtAudioDeviceName = "Shadow Virtual Audio Device";

            using ( var enumerator = new MMDeviceEnumerator() ) {
                foreach ( MMDevice wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.All) ) {
                    // Skip devices which aren't plugged in (otherwise we'd get a COM Exception upon querying their friendly names)
                    if ( wasapi.State == DeviceState.NotPresent )
                        continue;
                    // Skip "Render" devices, i.e. Playback devices like headphone, speakers, etc.
                    if ( wasapi.DataFlow == DataFlow.Render )
                        continue;

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
    }
}
