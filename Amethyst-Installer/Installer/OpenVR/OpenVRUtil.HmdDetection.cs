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

        private static string[] s_ModelBlackList = new string[] {
            "PVRServer",
            "Vridge",
        };
        private static string[] s_ManufacturerBlackList = new string[] {
            "PVRServer",
            "Riftcat",
        };

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
        public static bool IsPhoneVR() {
            var hmdModel = GetSteamVRHmdManufacturer().ToLowerInvariant();
            var hmdManufacturer = GetSteamVRHmdManufacturer().ToLowerInvariant();

            if ( hmdModel.Contains("lhr-00000000") ) { // TrinusVR
                return true;
            } else if ( hmdModel == "mobile headset" && hmdManufacturer == "ivry" ) { // iVRy
                return true;
            } else if ( hmdModel == "driver4vr" || hmdManufacturer == "driver4vr" ) { // Bonjour, this is Greg Driver
                return true;
            } else if ( hmdModel == "vridge" || hmdManufacturer == "riftcat" ) { // Riftcat
                return true;
            } // TODO: More hell AAAAAAAAAAAAAAAAAAAAA

            return false;
        }

        public static void DetectHeadset() {

        }
    }
}
