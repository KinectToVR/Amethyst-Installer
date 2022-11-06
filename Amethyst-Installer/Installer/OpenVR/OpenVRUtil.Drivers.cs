using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {

        private static string s_vrpathreg;

        /// <summary>
        /// Registers an OpenVR driver
        /// </summary>
        /// <param name="driverPath">The path to the driver</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterSteamVrDriver(string driverPath) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            string driverDirectory = Path.GetDirectoryName(driverPath);
            if ( !s_failedToInit ) {
                if ( File.Exists(s_vrpathreg) ) {
                    Logger.Info("Using vrpathreg...");
                    var args = $"adddriver \"{driverPath}\"";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = s_vrpathreg,
                        Arguments = args,
                        WorkingDirectory = driverDirectory,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                    Logger.Info("Waiting for vrpathreg...");
                    vrpathregProc.WaitForExit(10000);
                    Logger.Info("Done!");
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                        case 2: // Driver installed more than once
                            return;
                        case 1: // Driver not present
                            break;
                        case -1: // Configuration or permission problem
                            Logger.Fatal($"Failed to register using vrpathreg:: Configuration or permission problem");
                            break;
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: {vrpathregProc.ExitCode}\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            if ( s_openvrpaths != null ) {
                if ( !s_openvrpaths.external_drivers.Contains(driverPath) ) {
                    // Driver is not registered!
                    s_openvrpaths.external_drivers.Add(driverPath);
                    Logger.Info("Saving openvrpaths...");
                    SaveOpenVrPaths();
                    Logger.Info("Done!");
                }
            }
        }

        /// <summary>
        /// Returns the path of an OpenVR driver
        /// </summary>
        /// <param name="drivername">The driver's name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDriverPath(string drivername) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            if ( !s_failedToInit ) {
                if ( File.Exists(s_vrpathreg) ) {
                    var args = $"finddriver \"{drivername}\"";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = s_vrpathreg,
                        Arguments = args,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                        StandardOutputEncoding = System.Text.Encoding.UTF8,
                    });
                    var output = vrpathregProc.StandardOutput.ReadToEnd();
                    vrpathregProc.WaitForExit(10000);
                    output = output.Split(Environment.NewLine[0])[0];
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                            return output;
                        case 1: // Driver not present
                            return "";
                        case 2: // Driver installed more than once
                            return output;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: {vrpathregProc.ExitCode}\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            if ( s_openvrpaths != null && s_openvrpaths.external_drivers != null && s_openvrpaths.external_drivers.Count > 0 ) {
                for ( int i = 0; i < s_openvrpaths.external_drivers.Count; i++ ) {
                    // Make sure the driver directory even exists
                    if ( Directory.Exists(s_openvrpaths.external_drivers[i]) ) {

                        // Attempt to load the driver manifest
                        var driverManifestPath = Path.GetFullPath(Path.Combine(s_openvrpaths.external_drivers[i], "driver.vrdrivermanifest"));
                        try {
                            var driverManifest = JsonConvert.DeserializeObject<OpenVrDriverManifest>(File.ReadAllText(driverManifestPath));
                            // JSON safety checks
                            if ( driverManifest == null || driverManifest.Name == null) {
                                Logger.Error($"Invalid OpenVR driver manifest at \"{driverManifestPath}\"! The manifest may be corrupt or invalid...");
                                continue;
                            }
                            if ( driverManifest.Name.ToLowerInvariant() == drivername.ToLowerInvariant() ) {
                                return s_openvrpaths.external_drivers[i];
                            }
                        } catch ( Exception e ) {
                            Logger.Error($"Invalid OpenVR driver manifest at \"{driverManifestPath}\"! The manifest may be corrupt or invalid...");
                            Logger.Error(Util.FormatException(e));
                        }
                    }
                }
            }

            return "";
        }

        /// <summary>
        /// Removes a driver if it exists
        /// </summary>
        /// <param name="path">File path for the specified driver</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveDriver(string path) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            path = Path.GetFullPath(path);

            if ( !s_failedToInit ) {
                if ( File.Exists(s_vrpathreg) ) {
                    var args = $"removedriver \"{path}\"";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = s_vrpathreg,
                        Arguments = args,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    });
                    vrpathregProc.WaitForExit(10000);
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                            return;
                        case 1: // Driver not present
                            return;
                        case 2: // Driver installed more than once
                            return;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: {vrpathregProc.ExitCode}\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            if ( s_openvrpaths != null && s_openvrpaths.external_drivers != null && s_openvrpaths.external_drivers.Count > 0 ) {
                for ( int i = 0; i < s_openvrpaths.external_drivers.Count; i++ ) {
                    try {
                        // Cleanup invalids
                        if ( !Directory.Exists(s_openvrpaths.external_drivers[i]) ) {
                            s_openvrpaths.external_drivers.RemoveAt(i);
                            i--;
                            continue;
                        } else {
                            var driverManifestPath = Path.GetFullPath(Path.Combine(s_openvrpaths.external_drivers[i], "driver.vrdrivermanifest"));
                            if ( !File.Exists(driverManifestPath) ) {
                                s_openvrpaths.external_drivers.RemoveAt(i);
                                i--;
                                continue;
                            }
                        }
                        // Check if the driver matches, if so, remove
                        if ( s_openvrpaths.external_drivers[i].ToLowerInvariant() == path.ToLowerInvariant() ) {
                            s_openvrpaths.external_drivers.RemoveAt(i);
                            i--;
                        }
                    } catch (Exception e) {
                        Logger.Fatal($"Failed to read info for driver at {path}!");
                        Logger.Fatal(Util.FormatException(e));
                    }
                }
                SaveOpenVrPaths();
            }
        }

        /// <summary>
        /// Removes a driver if it exists
        /// </summary>
        /// <param name="path">File path for the specified driver</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RemoveDriversWithName(string driverName) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            if ( !s_failedToInit ) {
                if ( File.Exists(s_vrpathreg) ) {
                    var args = $"removedriverswithname \"{driverName}\"";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = s_vrpathreg,
                        Arguments = args,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    });
                    vrpathregProc.WaitForExit(10000);
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                            return;
                        case 1: // Driver not present
                            return;
                        case 2: // Driver installed more than once
                            return;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: {vrpathregProc.ExitCode}\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            if ( s_openvrpaths != null && s_openvrpaths.external_drivers != null && s_openvrpaths.external_drivers.Count > 0 ) {
                for ( int i = 0; i < s_openvrpaths.external_drivers.Count; i++ ) {
                    // Make sure the driver directory even exists
                    if ( Directory.Exists(s_openvrpaths.external_drivers[i]) ) {

                        // Attempt to load the driver manifest
                        var driverManifestPath = Path.GetFullPath(Path.Combine(s_openvrpaths.external_drivers[i], "driver.vrdrivermanifest"));
                        try {
                            var driverManifest = JsonConvert.DeserializeObject<OpenVrDriverManifest>(File.ReadAllText(driverManifestPath));
                            // JSON safety checks
                            if ( driverManifest == null || driverManifest.Name == null ) {
                                Logger.Error($"Invalid OpenVR driver manifest at \"{driverManifestPath}\"! The manifest may be corrupt or invalid...");
                                continue;
                            }
                            if ( driverManifest.Name.ToLowerInvariant() == driverName.ToLowerInvariant() ) {
                                s_openvrpaths.external_drivers.RemoveAt(i);
                                i--;
                            }
                        } catch ( Exception e ) {
                            Logger.Error($"Invalid OpenVR driver manifest at \"{driverManifestPath}\"! The manifest may be corrupt or invalid...");
                            Logger.Error(Util.FormatException(e));
                        }
                    }
                }
                SaveOpenVrPaths();
            }
        }

        /// <summary>
        /// Forces a SteamVR driver to be enabled
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ForceEnableDriver(string driverId) {

            return ForceDriverState(driverId, true);
        }

        /// <summary>
        /// Forces a SteamVR driver to be disabled
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ForceDisableDriver(string driverId) {

            return ForceDriverState(driverId, false);
        }

        /// <summary>
        /// Forces a SteamVR driver to the specified state
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ForceDriverState(string driverId, bool state) {

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
                if ( s_steamvrSettings[driverId] == null )
                    s_steamvrSettings[driverId] = new JObject();

                s_steamvrSettings[driverId]["enable"] = state;
                s_steamvrSettings[driverId]["blocked_by_safe_mode"] = false;
                SaveSteamVrSettings();
                return true;

            } catch ( Exception ex ) {
                // Oh no, the user has an antivirus probably
                // This isn't a critical exception, so we catch it
                Logger.Error(Util.FormatException(ex));
                Logger.Warn("The user might have to enable the driver manually in SteamVR.");
                return false;
            }
        }
    }
}
