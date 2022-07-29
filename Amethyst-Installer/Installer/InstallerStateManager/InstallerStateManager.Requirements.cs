using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static partial class InstallerStateManager {

        /// <summary>
        /// Whether the user can install Amethyst on this system at all.
        /// </summary>
        public static bool CanInstall = false;

        /// <summary>
        /// Whether SteamVR is installed and was found on this system.
        /// </summary>
        public static bool SteamVRInstalled = false;

        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsSystemSupported = true;

        /// <summary>
        /// Whether K2EX was found on the system, so that we remove it
        /// </summary>
        public static bool K2EXDetected = true;
        // public static bool K2EXDetected = true;
        public static string K2EXPath = string.Empty;

        private static void ComputeRequirements() {

            // TODO: actually compute the requirements for installing amethyst

            // Detect if we are even able to install Amethyst
            SteamVRInstalled = Directory.Exists(OpenVRUtil.RuntimePath());

            if ( SteamVRInstalled ) {
                // Detect the current SteamVR setup
                OpenVRUtil.DetectHeadset();
                OpenVRUtil.DetectQuestConnectionMethod();
            }

            // Try locating K2EX
            K2EXPath = K2EXUtil.LocateK2EX();
            K2EXDetected = K2EXPath.Length > 0 && Directory.Exists(K2EXPath);

        }

    }
}
