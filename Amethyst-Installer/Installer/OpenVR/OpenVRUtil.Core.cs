using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {

        /// <summary>
        /// Whether we failed to load openvr_api.dll ; triggers fallback behavior without raising an exception
        /// </summary>
        private static bool s_failedToInit = false;
        private static bool s_initialized = false;
        private static OpenVrPaths s_openvrpaths = null;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InitOpenVR() {
            if ( !s_initialized ) {
                LoadOpenVRAPI();
                LoadOpenVrPaths(true);
                LoadSteamVRSettings(true);
            } else
                Logger.Warn("Attempted to initialize OpenVR, but OpenVR was already initialized!");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        /// Loads openvrpaths.vrpath
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LoadOpenVrPaths(bool force = false) {

            if ( s_openvrpaths == null || force == true ) {
                string vrpathsFile = Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath"));
                if ( !File.Exists(vrpathsFile) ) {
                    Logger.Warn("openvrpaths.vrpath doesn't exist on the current system... Is SteamVR installed, and has it been run at least once?");
                    s_openvrpaths = null;
                    return;
                }

                string vrpathsTxt = File.ReadAllText(vrpathsFile);
                s_openvrpaths = JsonConvert.DeserializeObject<OpenVrPaths>(vrpathsTxt);
            }
        }
    }
}
