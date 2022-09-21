using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui {
    /// <summary>
    /// A collection of functions to make interfacing with Windows easier
    /// </summary>
    public static class WindowsUtils {

        /// <summary>
        /// Returns the display string of the currently installed Windows release, such as 20H2
        /// </summary>
        public static string GetDisplayVersion() {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            object buildNumber;

            buildNumber = registryKey.GetValue("DisplayVersion");
            if (buildNumber == null) {
                buildNumber = registryKey.GetValue("CurrentBuildNumber");
            }

            return buildNumber == null ? Environment.Version.Build.ToString() : buildNumber.ToString();
        }

        /// <summary>
        /// Returns the "patch" of the currently installed Windows version ; This would be the number after the build number in winver.
        /// </summary>
        public static Version GetVersion() {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var majorNumber = registryKey.GetValue("CurrentMajorVersionNumber");
            var minorNumber = registryKey.GetValue("CurrentMinorVersionNumber");
            var revisionNumber = registryKey.GetValue("CurrentBuildNumber");
            var buildNumber = registryKey.GetValue("UBR");

            int revNumAsInt32 = 0;
            int.TryParse(( string ) revisionNumber ?? "0", out revNumAsInt32);
            return new Version(
                ( int ) ( majorNumber ?? 0 ),
                ( int ) ( minorNumber ?? 0 ),
                revNumAsInt32,
                ( int ) ( buildNumber ?? 0 )
            );
        }

        /// <summary>
        /// Opens a registry, and creates it if it doesn't exist
        /// </summary>
        /// <param name="registryKey">Registry Key</param>
        /// <param name="name">Name or path of the subkey to open.</param>
        /// <param name="writable">Set to true if you need write access to the key.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RegistryKey GetKey(RegistryKey registryKey, string name, bool writable = false) {
            
            RegistryKey finalKey = registryKey.OpenSubKey(name, writable);
            if ( finalKey == null ) {
                finalKey = registryKey.CreateSubKey(name, writable);
            }

            return finalKey;
        }

        /// <summary>
        /// List of Windows versions
        /// </summary>
        public enum WindowsMajorReleases : int {
            Win11_22H2 = 22621,
            Win11_21H2 = 22000,
            Win10_21H2 = 19044,
            Win10_21H1 = 19043,
            Win10_20H2 = 19042,
            Win10_2004 = 19041,
            Win10_1909 = 18363,
            Win10_1903 = 18362,
            Win10_1809 = 17763,
            Win10_1803 = 17134,
            Win10_1709 = 16299,
            Win10_1703 = 15063,
            Win10_1607 = 14393,
            Win10_1511 = 10586,
            Win10_1507 = 10240,
        }
    }
}
