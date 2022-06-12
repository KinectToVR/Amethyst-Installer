using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static class OpenVRUtil {

        /// <summary>
        /// Whether we failed to load openvr_api.dll ; triggers fallback behavior without raising an exception
        /// </summary>
        private static bool s_failedToInit = false;
        private static bool s_initialized = false;
        private static OpenVrPaths s_openvrpaths = null;

        public static void InitOpenVR() {
            if ( !s_initialized )
                LoadOpenVRAPI();
            else
                Logger.Warn("Attempted to initialize OpenVR, but OpenVR was already initialized!");
        }

        private static void LoadOpenVRAPI() {

            var ovrDll = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, "openvr_api.dll"));

            // Extract the openvr_api.dll binary to our temp directory
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.Resources.Binaries.openvr_api.dll") ) {
                using ( var file = new FileStream(ovrDll, FileMode.Create, FileAccess.Write) ) {
                    resource.CopyTo(file);
                }
            }

            // Load the openvr_api.dll unmanaged library using P/Invoke :D
            var result = Kernel.LoadLibrary(ovrDll);
            if ( result == IntPtr.Zero ) {
                Logger.Fatal("Failed to load openvr_api.dll!");
                Logger.Warn("Falling back to openvrpaths.vrpath...");
                s_failedToInit = true;
            } else {
                s_initialized = true;
                Logger.Info("Successfully loaded openvr_api.dll!");
            }
        }

        /// <summary>
        /// Returns whether SteamVR is currently running or not
        /// </summary>
        public static bool IsSteamVrRunning() {
            // Checks if vrserver is running
            return Process.GetProcessesByName("vrserver").Length > 0;
        }

        /// <summary>
        /// Returns the SteamVR installation directory
        /// </summary>
        /// <returns><see cref="string.Empty"/> if it fails to find a suitable directory</returns>
        public static string RuntimePath() {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");
            if ( s_failedToInit ) {

                // Try reading openvrpaths, and grab the runtime field.
                // From there: for each entry, try checking if the directory exists, and, if so, return it
                var vrpaths = TryGetOpenVrPaths();
                if ( vrpaths != null ) {
                    for (int i = 0; i < vrpaths.runtime.Count; i++ ) {
                        if ( Directory.Exists(vrpaths.runtime[i]) )
                            return vrpaths.runtime[i];
                    }
                }

                return string.Empty;
            } else
                return Valve.VR.OpenVR.RuntimePath();
        }

        /// <summary>
        /// Registers an OpenVR driver
        /// </summary>
        /// <param name="driverPath">The path to the driver</param>
        public static void RegisterSteamVrDriver(string driverPath) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            string driverDirectory = Path.GetDirectoryName(driverPath);

            if (!s_failedToInit) {
                string vrpathregPath = Path.GetFullPath(Path.Combine(Valve.VR.OpenVR.RuntimePath(), "bin", "win64", "vrpathreg.exe"));
                if ( File.Exists(vrpathregPath) ) {
                    // TODO: vrpathreg now returns error codes! use it for driver handling
                    var args = "";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = vrpathregPath,
                        Arguments = args,
                        WorkingDirectory = driverDirectory,
                    });
                    vrpathregProc.WaitForExit();
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                        case 2: // Driver installed more than once
                            return;
                        case 1: // Driver not present
                            break;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: -2\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }
            
            // TODO: Fallback to openvrpaths

            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the path of an OpenVR driver
        /// </summary>
        /// <param name="drivername">The driver's name</param>
        public static string GetDriverPath(string drivername) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            if ( !s_failedToInit ) {
                string vrpathregPath = Path.GetFullPath(Path.Combine(Valve.VR.OpenVR.RuntimePath(), "bin", "win64", "vrpathreg.exe"));
                if ( File.Exists(vrpathregPath) ) {
                    var args = $"finddriver {drivername}";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = vrpathregPath,
                        Arguments = args,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    });
                    var output = vrpathregProc.StandardOutput.ReadToEnd();
                    vrpathregProc.WaitForExit();
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                            return output;
                        case 1: // Driver not present
                            return "";
                        case 2: // Driver installed more than once
                            return output;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: -2\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            // TODO: Fallback to openvrpaths
            var openvrPaths = TryGetOpenVrPaths();
            if (openvrPaths.external_drivers.Count > 0) {

            }

            return "";
        }

        /// <summary>
        /// Returns a deserialized instance of openvrpaths.vrpath
        /// </summary>
        private static OpenVrPaths TryGetOpenVrPaths() {

            if ( s_openvrpaths == null ) {
                string vrpathsFile = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath"));
                if ( !File.Exists(vrpathsFile) ) {
                    Logger.Warn("openvrpaths.vrpath doesn't exist on the current system... Is SteamVR installed, and has it been run at least once?");
                    return null;
                }

                string vrpathsTxt = File.ReadAllText(vrpathsFile);

                s_openvrpaths = JsonConvert.DeserializeObject<OpenVrPaths>(vrpathsTxt);
            }
            return s_openvrpaths;
        }

        // TODO: steamvr.vrsettings check
        // If it doesn't exist ask the user to run SteamVR ONCE
    }
}
