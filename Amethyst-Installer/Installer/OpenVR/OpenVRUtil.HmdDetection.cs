﻿using amethyst_installer_gui.Installer.OpenVR;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace amethyst_installer_gui.Installer {
    public static partial class OpenVRUtil {

        public static VRConnectionType ConnectionType = VRConnectionType.Unknown;
        public static VRHmdType HmdType = VRHmdType.Unknown;
        public static VRTrackingType TrackingType = VRTrackingType.Unknown;
        public static string AlvrInstallPath = string.Empty;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSteamVRHmdModel() {
            if ( s_steamvrSettings == null ) {
                return "";
            }
            if ( s_steamvrSettings["LastKnown"] == null ) {
                return "";
            }
            return s_steamvrSettings["LastKnown"]["HMDModel"] ?? "";
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetSteamVRHmdManufacturer() {
            if ( s_steamvrSettings == null ) {
                return "";
            }
            if ( s_steamvrSettings["LastKnown"] == null ) {
                return "";
            }
            return s_steamvrSettings["LastKnown"]["HMDManufacturer"] ?? "";
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
            } else if ( hmdModel == "pvrserver" || hmdManufacturer == "viritualis res" ) { // PhoneVR (yeah look it up on GitHub its a real thing)
                return true;
            } else if ( hmdModel == "hades" && hmdManufacturer == "" ) {
                // https://github.com/HadesVR/HadesVR
                // cool project, if you can do this you're capable of getting past our installer.
                return true;
            } // @TODO: More hell whenever we find more phone "vr" headset apps people use

            return false;
        }

        /// <summary>
        /// Returns if the user uses lighthouses or not
        /// </summary>
        private static bool UsesLighthouseTracking() {
            return s_steamvrSettings["driver_lighthouse"] != null;
        }

        /// <summary>
        /// Attempts to detect the user's SteamVR headset
        /// </summary>
        public static void DetectHeadset() {
            var hmdModel = GetSteamVRHmdModel().ToLowerInvariant();
            var hmdManufacturer = GetSteamVRHmdManufacturer().ToLowerInvariant();

            // For debugging purposes
            Logger.Info($"Steam VR Last HMD Model: {hmdModel}");
            Logger.Info($"Steam VR Last HMD Manufacturer: {hmdManufacturer}");

            // PhoneVR
            if ( IsPhoneVR(hmdModel, hmdManufacturer) ) {
                HmdType = VRHmdType.Phone;
                ConnectionType = VRConnectionType.Unknown;
                TrackingType = VRTrackingType.Unknown;
                return;
            }

            // Assume any HMD manufactured by pimax is a pimax HMD, I don't feel like dealing with a lot of punctuation hell
            if ( hmdManufacturer.Contains("pimax") ) {
                hmdManufacturer = "pimax";
            }

            // Detect proper headsets

            switch ( hmdManufacturer ) {

                // WMR
                case "windowsmr":
                    HmdType = VRHmdType.WMR;
                    ConnectionType = VRConnectionType.Tethered;
                    // Reverb G2 users using konkles moment:
                    TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.MixedReality;
                    return;

                // Lighthouse Headsets
                // Valve
                case "valve":
                    ConnectionType = VRConnectionType.Tethered;
                    TrackingType = VRTrackingType.Lighthouse;
                    switch ( hmdModel ) {
                        case "index":
                            HmdType = VRHmdType.Index;
                            return;
                        default:
                            HmdType = VRHmdType.Deckard;
                            Logger.Info("OMG HI BRAD!!!");
                            return;
                    }

                // HTC Headsets
                case "htc":
                    // Yes I know the wireless adapter exists, but for the purposes of this installer I'm not
                    // bothering with detecting it, as this property is designed mainly for Quest
                    ConnectionType = VRConnectionType.Tethered;
                    TrackingType = VRTrackingType.Lighthouse;
                    if ( hmdModel.Contains("pro") ) {
                        HmdType = VRHmdType.VivePro;
                    } else if ( hmdModel.Contains("cosmos") ) {
                        TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.MixedReality;
                        HmdType = VRHmdType.ViveCosmos;
                    } else {
                        HmdType = VRHmdType.Vive;
                    }
                    return;

                // Pimax Headsets
                case "pimax":
                    ConnectionType = VRConnectionType.Tethered;
                    TrackingType = VRTrackingType.Lighthouse;
                    HmdType = VRHmdType.Pimax;
                    return;


                // Hell (Oculus)
                case "oculus":
                    ConnectionType = VRConnectionType.Tethered;
                    TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.Oculus;
                    switch ( hmdModel ) {
                        case "oculus rift cv1":
                            HmdType = VRHmdType.Rift;
                            return;
                        case "oculus rift s":
                            HmdType = VRHmdType.RiftS;
                            return;
                        case "oculus quest":
                            HmdType = VRHmdType.Quest;
                            TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.Quest;
                            DetectQuestConnectionMethod();
                            return;
                        case "miramar": // Most common HMD name I've seen when using ALVR, ALVR is special and I fucking hate it
                        case "oculus quest2":
                        case "oculus quest 2":
                            HmdType = VRHmdType.Quest2;
                            TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.Quest;
                            DetectQuestConnectionMethod();
                            return;
                    }
                    return;

                // Pico Neo
                case "pico":
                    // @TODO: Revise further because yeahhhhh
                    ConnectionType = VRConnectionType.Tethered;
                    TrackingType = UsesLighthouseTracking() ? VRTrackingType.Lighthouse : VRTrackingType.Oculus;
                    switch ( hmdModel ) {
                        case "pico neo":
                            HmdType = VRHmdType.PicoNeo;
                            return;
                        case "pico neo 2":
                            HmdType = VRHmdType.PicoNeo2;
                            return;
                        case "pico neo 3":
                            HmdType = VRHmdType.PicoNeo3;
                            return;
                    }
                    return;
            }
        }

        public static void DetectQuestConnectionMethod() {
            ConnectionType = VRConnectionType.Unknown;

            try {
                // Try searching for ALVR
                var alvrUninstallEntry = UninstallUtil.GetUninstallEntry("ALVR");
                AlvrInstallPath = alvrUninstallEntry?.InstallLocation ?? "";
                var alvrLastUsageDate = DateTime.MinValue;
                if ( Directory.Exists(AlvrInstallPath) ) {
                    Logger.Info($"Detected an ALVR install at \"{AlvrInstallPath}\"!");
                    // I'm guessing all of this i have no clue if this will work or not
                    string alvrExecutablePath = Path.GetFullPath(Path.Combine(AlvrInstallPath, "ALVR Launcher.exe"));
                    if ( File.Exists(alvrExecutablePath) ) {
                        alvrLastUsageDate = File.GetLastAccessTime(alvrExecutablePath);
                    }
                }

                // Try searching for VD
                string vdPath = ( string ) Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Virtual Desktop, Inc.\Virtual Desktop Streamer", "Path", "");
                var vdLastUsageDate = DateTime.MinValue;
                if ( Directory.Exists(vdPath) ) {
                    Logger.Info($"Detected a Virtual Desktop Streamer install at \"{vdPath}\"!");
                    string vdExecutablePath = Path.GetFullPath(Path.Combine(vdPath, "VirtualDesktop.Streamer.exe"));
                    if ( File.Exists(vdExecutablePath) ) {
                        vdLastUsageDate = File.GetLastAccessTime(vdExecutablePath);
                    }
                }

                // Compare last access time?
                // Is this even a good idea?
                // Given based on what I'm reading this is prone to false positives...
                // ONLY ONE WAY TO FIND OUT: TEST AND THROW IT IN THE WILD

                if ( alvrLastUsageDate != DateTime.MinValue && vdLastUsageDate != DateTime.MinValue ) {
                    // ALVR and VD are both installed on this machine
                    if ( alvrLastUsageDate < vdLastUsageDate ) {
                        ConnectionType = VRConnectionType.VirtualDesktop;
                    } else {
                        ConnectionType = VRConnectionType.ALVR;
                    }
                } else if ( alvrLastUsageDate != DateTime.MinValue ) {
                    // Only ALVR was found on this machine
                    ConnectionType = VRConnectionType.ALVR;
                } else if ( vdLastUsageDate != DateTime.MinValue ) {
                    // Only VD was found on this machine
                    ConnectionType = VRConnectionType.VirtualDesktop;
                } else {
                    Logger.Info($"Couldn't find a Virtual Desktop or ALVR install, assuming Oculus Link / Air Link!");
                    
                    // We are able to distinguish between Oculus Link and Oculus Air Link
                    ConnectionType = VRConnectionType.OculusLink;

                    string oculusCachePath = Path.GetFullPath(Path.Combine(Constants.Userprofile, "AppData", "Local", "Oculus", "DeviceCache.json"));
                    if ( File.Exists(oculusCachePath) ) {
                        OculusDeviceCache deviceCache = JsonConvert.DeserializeObject<OculusDeviceCache>(File.ReadAllText(oculusCachePath));
                        if (deviceCache != null && deviceCache.Devices != null && deviceCache.Devices.Count > 0) {
                            for ( int i = 0; i < deviceCache.Devices.Count; i++ ) {
                                var device = deviceCache.Devices[i];
                                if ( device != null) {
                                    if ( device.Type == "headset" && // If the device is an HMD
                                        device.SupportsOculusLink == true) {
                                        // Assume we have a Quest if it supports Oculus Link
                                        ConnectionType = device.IsUsingAirLink ? VRConnectionType.OculusAirLink : VRConnectionType.OculusLink;
                                    }
                                }
                            }
                        }
                    }

                }
            } catch ( Exception e ) {
                Logger.Error("Failed to detect Quest connection method!");
                Logger.Error(Util.FormatException(e));
            }
        }
    }
}