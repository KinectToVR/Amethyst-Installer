﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SharpDX.Direct3D9;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Valve.VR;
using OVR = Valve.VR.OpenVR;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {

        // If it doesn't exist ask the user to run SteamVR ONCE
        private static dynamic s_steamvrSettings;
        private static bool s_steamvrSettingsExists = false;
        private static string s_steamvrSettingsPath;
        private static bool s_steamvrAppConfigExists = false;
        private static string s_steamvrAppConfigPath;
        private static string s_openvrPathsPath;

        public static string OpenVrPathsPath { get { return s_openvrPathsPath; } }

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

            if ( !s_failedToInit ) {
                if ( Valve.VR.OpenVR.IsRuntimeInstalled() ) {
                    return Valve.VR.OpenVR.RuntimePath();
                } else {
                    // Disable vrpathreg
                    s_failedToInit = true;
                }
            }

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

            // vrpaths is fucked AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
            // Disable vrpathreg
            s_failedToInit = true;
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam", "steamapps", "common", "SteamVR");
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
                    try {
                        var steamVRSettingsTxt = File.ReadAllText(s_steamvrSettingsPath);
                        s_steamvrSettings = JObject.Parse(steamVRSettingsTxt);
                    } catch {
                        s_steamvrSettingsExists = false;
                    }
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

        /// <summary>
        /// Disables SteamVR Home
        /// </summary>
        /// <returns>Whether SteamVR Home could be disabled successfully</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool DisableSteamVrHome() {

            try {
                // Try loading steam vr settings in case
                LoadSteamVRSettings();
                if ( s_steamvrSettings == null ) {
                    if ( s_steamvrSettings == null ) {
                        Logger.Warn("SteamVR Settings doesn't exist! Has SteamVR been run at least once?");
                        return false;
                    }
                }

                // Now try force enabling the driver
                if ( s_steamvrSettings["steamvr"] == null )
                    s_steamvrSettings["steamvr"] = new JObject();

                s_steamvrSettings["steamvr"]["enableHomeApp"] = false;
                SaveSteamVrSettings();
                return true;

            } catch ( Exception ex ) {
                // Oh no, the user has an antivirus probably
                // This isn't a critical exception, so we catch it
                Logger.Error(Util.FormatException(ex));
                Logger.Warn("SteamVR home couldn't be disabled!");
                return false;
            }
        }

        /// <summary>
        /// Enables SteamVR advanced settings
        /// </summary>
        /// <returns>Whether SteamVR Advanced Settings could be enabled or not</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EnableAdvancedSettings() {

            try {
                // Try loading steam vr settings in case
                LoadSteamVRSettings();
                if ( s_steamvrSettings == null ) {
                    if ( s_steamvrSettings == null ) {
                        Logger.Warn("SteamVR Settings doesn't exist! Has SteamVR been run at least once?");
                        return false;
                    }
                }

                // Now try force enabling the driver
                if ( s_steamvrSettings["steamvr"] == null )
                    s_steamvrSettings["steamvr"] = new JObject();

                s_steamvrSettings["steamvr"]["showAdvancedSettings"] = true;
                SaveSteamVrSettings();
                return true;

            } catch ( Exception ex ) {
                // Oh no, the user has an antivirus probably
                // This isn't a critical exception, so we catch it
                Logger.Error(Util.FormatException(ex));
                Logger.Warn("SteamVR advanced settings couldn't be enabled!");
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ComputeSteamVRAppConfigPath() {
            if ( s_openvrpaths != null ) {
                // Try getting SteamVR Settings from OpenVR Paths
                for ( int i = 0; i < s_openvrpaths.config.Count; i++ ) {
                    if ( Directory.Exists(s_openvrpaths.config[i]) ) {
                        s_steamvrAppConfigPath = Path.Combine(s_openvrpaths.config[i], "appconfig.json");
                        s_steamvrAppConfigExists = File.Exists(s_steamvrSettingsPath);
                        return;
                    }
                }
            }

            // Fallback to Steam Directory
            var steamInstallDirectory = GetSteamInstallDirectory();
            if ( Directory.Exists(steamInstallDirectory) ) {
                s_steamvrAppConfigPath = Path.Combine(steamInstallDirectory, "config", "appconfig.json");
                s_steamvrAppConfigExists = File.Exists(s_steamvrSettingsPath);
            }
        }

        /// <summary>
        /// Registers an overlay with OpenVR
        /// </summary>
        /// <param name="manifestPath">A path pointing to the manifest file</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RegisterOverlayAndAutoStart(string manifestPath, string manifestKey, bool startupMode) {

            if ( !File.Exists(manifestPath) ) {
                Logger.Error($"Manifest at \"{manifestPath}\" not found on disk!");
                return false;
            }

            if ( OVR.Applications is null ) {
                Logger.Error($"Failed to install manifest due to an unknown OpenVR error.");
                return false;
            }

            // Attempt registering the overlay with SteamVR
            if ( !OVR.Applications.IsApplicationInstalled(manifestKey) ) {
                var appError = OVR.Applications.AddApplicationManifest(manifestPath, false);

                if ( appError != EVRApplicationError.None ) {
                    Logger.Error($"Failed to install manifest.\n{appError}");
                }

                Logger.Error($"Installed manifest at {manifestPath}");
            }

            // Set startup mode
            OVR.Applications.SetApplicationAutoLaunch(manifestKey, startupMode);

            return true;
        }
    }
}
