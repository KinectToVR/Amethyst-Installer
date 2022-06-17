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
            var hmdModel = GetSteamVRHmdManufacturer();
            var hmdManufacturer = GetSteamVRHmdManufacturer();

            if ( hmdModel.Contains("LHR-00000000") ) { // TrinusVR
                return true;
            } else if ( hmdModel == "Mobile Headset" && hmdManufacturer == "iVRy" ) { // iVRy
                return true;
            } else if ( hmdModel == "Driver4VR" || hmdManufacturer == "Driver4VR" ) { // Bonjour, this is Greg Driver
                return true;
            } else if ( hmdModel == "Vridge" || hmdManufacturer == "Riftcat" ) { // Riftcat
                return true;
            } // TODO: More hell AAAAAAAAAAAAAAAAAAAAA

            return false;
        }
    }
}
