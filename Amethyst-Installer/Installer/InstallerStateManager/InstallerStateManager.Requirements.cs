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

        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsSystemSupported = true;

        private static void ComputeRequirements() {

            // TODO: 

        }

    }
}
