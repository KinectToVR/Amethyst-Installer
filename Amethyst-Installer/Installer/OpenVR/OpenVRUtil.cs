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

        public static void InitOpenVR() {
            if ( !s_initialized )
                LoadOpenVRAPI();
            else
                Logger.Warn("Attempted to initialize OpenVR, but OpenVR was already initialized!");
        }

        private static void LoadOpenVRAPI() {

            var ovrDll = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, "openvr_api.dll"));

            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream("amethyst_installer_gui.Resources.Binaries.openvr_api.dll") ) {
                using ( var file = new FileStream(ovrDll, FileMode.Create, FileAccess.Write) ) {
                    resource.CopyTo(file);
                }
            }

            // Load the openvr_api.dll unmanaged library using P/Invoke :D
            var result = Kernel.LoadLibrary(ovrDll);
            if ( result == IntPtr.Zero ) {
                Logger.Fatal("Failed to load openvr_api.dll");
                Logger.Warn("Falling back to openvrpaths.vrpath...");
                s_failedToInit = true;
            }

            s_initialized = true;
            Logger.Info("Successfully loaded openvr_api.dll!");
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
        /// <returns>null if it fails to find a suitable directory</returns>
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

                return null;
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

            if (!s_failedToInit) {

            }

            // TODO: vrpathreg now returns error codes! use it for driver handling

            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a deserialized instance of openvrpaths.vrpath
        /// </summary>
        private static OpenVrPaths TryGetOpenVrPaths() {

            string vrpathsFile = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath"));
            if ( !File.Exists(vrpathsFile) ) {
                Logger.Warn("openvrpaths.vrpath doesn't exist on the current system... Is SteamVR installed and has it been run once?");
                return null;
            }

            string vrpathsTxt = File.ReadAllText(vrpathsFile);

            return JsonConvert.DeserializeObject<OpenVrPaths>(vrpathsTxt);
        }

        // TODO: steamvr.vrsettings check
        // If it doesn't exist ask the user to run SteamVR ONCE
    }
}
