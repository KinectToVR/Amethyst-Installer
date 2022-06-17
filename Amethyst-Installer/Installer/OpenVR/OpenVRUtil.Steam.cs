using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {

        /// <summary>
        /// Returns the Steam install directory, or an empty string if Steam couldn't be found
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static string GetSteamInstallDirectory() {
            // Get Steam Directory from Registry, starting with 64-bit and falling back to 32-bit
            string steamInstallDirectory = ( string ) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", string.Empty);
            if ( Directory.Exists(steamInstallDirectory) )
                return steamInstallDirectory;
            else {
                steamInstallDirectory = ( string ) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", string.Empty);
                if ( Directory.Exists(steamInstallDirectory) )
                    return steamInstallDirectory;
            }

            Logger.Warn($"Failed to locate Steam Install directory. Is Steam installed on this system?");
            return string.Empty;
        }

    }
}
