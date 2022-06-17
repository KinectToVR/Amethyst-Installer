using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // If it doesn't exist ask the user to run SteamVR ONCE
        private static dynamic s_steamvrSettings;
        private static bool s_steamvrSettingsExists;
        private static string s_steamvrSettingsPath;

        /// <summary>
        /// Returns whether SteamVR is currently running or not
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSteamVrRunning() {
            // Checks if vrserver is running
            return Process.GetProcessesByName("vrserver").Length > 0 || Process.GetProcessesByName("vrmonitor").Length > 0;
        }

        /// <summary>
        /// Returns the SteamVR installation directory
        /// </summary>
        /// <returns><see cref="string.Empty"/> if it fails to find a suitable directory</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string RuntimePath() {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");
            if ( s_failedToInit ) {

                // Try reading openvrpaths, and grab the runtime field.
                // From there: for each entry, try checking if the directory exists, and, if so, return it
                if ( s_openvrpaths != null ) {
                    try {
                        for ( int i = 0; i < s_openvrpaths.runtime.Count; i++ ) {
                            if ( Directory.Exists(s_openvrpaths.runtime[i]) )
                                return s_openvrpaths.runtime[i];
                        }
                    } catch ( Exception ) {
                        Logger.Error($"Failed to fetch runtime argument from OpenVR Paths, is OpenVR Paths corrupt?");
                    }
                }

                return string.Empty;
            } else
                return Valve.VR.OpenVR.RuntimePath();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeSteamVRSettingsPath() {
            if ( s_openvrpaths != null ) {
                // Try getting SteamVR Settings from OpenVR Paths
                for ( int i = 0; i < s_openvrpaths.config.Count; i++ ) {
                    if ( Directory.Exists(s_openvrpaths.config[i]) ) {
                        s_steamvrSettingsPath = Path.Combine(s_openvrpaths.config[i], "steamvr.vrsettings");
                        s_steamvrSettingsExists = File.Exists(s_steamvrSettingsPath);
                        return;
                    }
                }
            }

            // Fallback to Steam Directory
            var steamInstallDirectory = GetSteamInstallDirectory();
            if ( Directory.Exists(steamInstallDirectory) ) {
                s_steamvrSettingsPath = Path.Combine(steamInstallDirectory, "config", "steamvr.vrsettings");
                s_steamvrSettingsExists = File.Exists(s_steamvrSettingsPath);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LoadSteamVRSettings(bool force = false) {
            if ( s_steamvrSettings == null || force == true ) {
                ComputeSteamVRSettingsPath();

                if ( s_steamvrSettingsExists ) {
                    var steamVRSettingsTxt = File.ReadAllText(s_steamvrSettingsPath);
                    s_steamvrSettings = JObject.Parse(steamVRSettingsTxt);
                }
            }
        }
    }
}
