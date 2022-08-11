using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
            var buildNumber = registryKey.GetValue("DisplayVersion").ToString();
            return buildNumber ?? Environment.Version.Build.ToString();
        }

        /// <summary>
        /// Returns the "patch" of the currently installed Windows version ; This would be the number after the build number in winver.
        /// </summary>
        public static int GetVersion() {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            var buildNumber = registryKey.GetValue("UBR");
            return ( int ) (buildNumber ?? int.MinValue);
        }

        /// <summary>
        /// List of Windows versions
        /// </summary>
        public enum WindowsMajorReleases : int {
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
