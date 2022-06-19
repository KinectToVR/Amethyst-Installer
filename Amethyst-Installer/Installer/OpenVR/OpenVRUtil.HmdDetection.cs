using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    public static partial class OpenVRUtil {

        public static VRConnectionType ConnectionType = VRConnectionType.Unknown;
        public static VRHmdType HmdType = VRHmdType.Unknown;
        public static VRTrackingType TrackingType = VRTrackingType.Unknown;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSteamVRHmdModel() {
            if ( s_steamvrSettings == null )
                return "";
            return s_steamvrSettings["LastKnown"]["HMDModel"];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSteamVRHmdManufacturer() {
            if ( s_steamvrSettings == null )
                return "";
            return s_steamvrSettings["LastKnown"]["HMDManufacturer"];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPhoneVR(string hmdModel, string hmdManufacturer) {

            if ( hmdModel.Contains("lhr-00000000") ) { // TrinusVR
                return true;
            } else if ( hmdModel == "mobile headset" && hmdManufacturer == "ivry" ) { // iVRy
                return true;
            } else if ( hmdModel == "driver4vr" || hmdManufacturer == "driver4vr" ) { // Bonjour, this is Greg Driver
                return true;
            } else if ( hmdModel == "vridge" || hmdManufacturer == "riftcat" ) { // Riftcat
                return true;
            } else if ( hmdModel == "pvrserver" || hmdManufacturer == "viritualis res" ) { // PhoneVR
                return true;
            } // TODO: More hell AAAAAAAAAAAAAAAAAAAAA

            return false;
        }

        /// <summary>
        /// Attempts to detect the user's SteamVR headset
        /// </summary>
        public static void DetectHeadset() {
            var hmdModel = GetSteamVRHmdManufacturer().ToLowerInvariant();
            var hmdManufacturer = GetSteamVRHmdManufacturer().ToLowerInvariant();

            // PhoneVR
            if ( IsPhoneVR(hmdModel, hmdManufacturer) ) {
                HmdType = VRHmdType.Phone;
                ConnectionType = VRConnectionType.Unknown;
                TrackingType = VRTrackingType.Unknown;
                return;
            }

            // Detect proper headsets

            // WMR
            if ( hmdManufacturer == "windowsmr" ) {
                HmdType = VRHmdType.WMR;
                ConnectionType = VRConnectionType.Tethered;
                // Reverb G2 users using konkles moment:
                TrackingType = s_steamvrSettings["driver_lighthouse"] == null ? VRTrackingType.MixedReality : VRTrackingType.Lighthouse;
                return;
            }

            // Lighthouse
            if ( hmdManufacturer == "valve" || hmdManufacturer == "htc" ) {
                ConnectionType = VRConnectionType.Tethered;
                TrackingType = VRTrackingType.Lighthouse;
                HmdType = VRHmdType.WMR;
                return;
            }

            // Hell (Oculus)
            if ( hmdManufacturer == "oculus" ) {
                HmdType = VRHmdType.WMR;
                ConnectionType = VRConnectionType.Tethered;
                TrackingType = VRTrackingType.MixedReality;
                return;
            }
        }
    }
}
