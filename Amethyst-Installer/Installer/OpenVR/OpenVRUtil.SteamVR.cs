using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
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
        private static bool s_steamvrSettingsExists = false;
        private static string s_steamvrSettingsPath;
        private static string s_openvrPathsPath;

        /// <summary>
        /// Returns whether SteamVR is currently running or not
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsSteamVrRunning() {
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
                return Valve.VR.OpenVR.IsRuntimeInstalled() ? Valve.VR.OpenVR.RuntimePath() : string.Empty;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SaveSteamVrSettings() {
            if ( s_steamvrSettings != null ) {
                string newSteamVrSettingsContents = JsonConvert.SerializeObject(s_steamvrSettings, Formatting.Indented);
                File.WriteAllText(s_steamvrSettingsPath, newSteamVrSettingsContents);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SaveOpenVrPaths() {
            if ( s_openvrpaths != null ) {
                string newOpenVrVrPathsContents = JsonConvert.SerializeObject(s_openvrpaths, Formatting.Indented);
                File.WriteAllText(s_openvrPathsPath, newOpenVrVrPathsContents);
            }
        }

        public static Vec2 GetPlayspaceBounds() {

            // In a perfect world, we would just run
            // 
            // float sizeX = 0f, sizeY = 0f;
            // Valve.VR.EVRInitError initErr = Valve.VR.EVRInitError.None;
            // Valve.VR.OpenVR.Init(ref initErr);
            // Valve.VR.OpenVR.Chaperone.GetPlayAreaSize(ref sizeX, ref sizeY);

            // However, we are using SteamVR, so instead WE DO THIS:
            double sizeX = 0.0, sizeY = 0.0;

            // TODO: not be a lazy piece of shit
            // Oculus => config/oculus/driver_oculus.vrchap
            // Lighthouse => config/chaperone_info.vrchap
            // WMR => config/chaperone_info.vrchap

            string vrchapPath = "";
            bool isSteamVrDefaultChaperoneFile = false;
            if ( s_openvrpaths != null ) {
                // Try getting SteamVR Settings from OpenVR Paths
                for ( int i = 0; i < s_openvrpaths.config.Count; i++ ) {
                    if ( Directory.Exists(s_openvrpaths.config[i]) ) {
                        vrchapPath = s_openvrpaths.config[i];
                    }
                }
            } else {
                // Fallback to Steam Directory
                var steamInstallDirectory = GetSteamInstallDirectory();
                if ( Directory.Exists(steamInstallDirectory) ) {
                    vrchapPath = Path.Combine(steamInstallDirectory, "config");
                }
            }

            switch ( HmdType ) {
                case VRHmdType.Rift:
                case VRHmdType.RiftS:
                case VRHmdType.Quest:
                case VRHmdType.Quest2:
                    // Oculus
                    vrchapPath = Path.Combine(vrchapPath, "oculus", "driver_oculus.vrchap");
                    break;
                case VRHmdType.Vive:
                case VRHmdType.VivePro:
                case VRHmdType.Pimax:
                case VRHmdType.Index:
                case VRHmdType.Deckard:
                    // Lighthouse

                case VRHmdType.WMR:
                    // WMR

                default:
                    // Assume the playspace data is stored in chaperone_info.vrchap
                    vrchapPath = Path.Combine(vrchapPath, "chaperone_info.vrchap");
                    isSteamVrDefaultChaperoneFile = true;
                    break;
            }

            if ( !File.Exists(vrchapPath) && isSteamVrDefaultChaperoneFile == false ) {
                vrchapPath = Path.Combine(vrchapPath, "chaperone_info.vrchap");
            }
            // If the file still doesn't exist, assume the user has never run SteamVR on their system
            if ( !File.Exists(vrchapPath) ) {
                return new Vec2(0.0, 0.0);
            }
            OpenVrPlayspace steamVrChaperone = JsonConvert.DeserializeObject<OpenVrPlayspace>(File.ReadAllText(vrchapPath));
            if ( steamVrChaperone != null && steamVrChaperone.Universes != null ) {
                int latestChaperoneIndex = 0;
                // get the newest steamvr chaperone universe
                // this assumes that the higher the numerical value of the "universeID" string the newer the playspace is
                // we don't check via date because that would require a custom DateTime parse and this installer is already
                // over-engineered enough
                ulong latestID = 0, currentID = 0; // declared here because of garbage collection hell
                bool latestSuccess = false, currentSuccess = false;
                for ( int i = 0; i < steamVrChaperone.Universes.Count; i++ ) {
                    if ( steamVrChaperone.Universes[i].UniverseID != null && steamVrChaperone.Universes[i].PlayArea != null ) {
                        // reset
                        latestSuccess = false;
                        currentSuccess = false;

                        latestSuccess = ulong.TryParse(steamVrChaperone.Universes[latestChaperoneIndex].UniverseID, out latestID);
                        currentSuccess = ulong.TryParse(steamVrChaperone.Universes[i].UniverseID, out currentID);
                        if ( latestSuccess && currentSuccess && currentID > latestID ) {
                            latestChaperoneIndex = i;
                        }
                    }
                }
                sizeX = ( double ) steamVrChaperone.Universes[latestChaperoneIndex].PlayArea[0];
                sizeY = ( double ) steamVrChaperone.Universes[latestChaperoneIndex].PlayArea[1];
            }

            return new Vec2(sizeX, sizeY);
        }
    }
}
