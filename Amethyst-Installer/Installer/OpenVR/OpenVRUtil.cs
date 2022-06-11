using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
                // Exception e = new Win32Exception();
                // throw new DllNotFoundException("Unable to load openvr_api.dll", e);
                Logger.Fatal("Failed to load openvr_api.dll");
                Logger.Warn("Falling back to openvrpaths.vrpath...");
                s_failedToInit = true;
            }

            Logger.Info("Successfully loaded openvr_api.dll!");
        }
    }
}
