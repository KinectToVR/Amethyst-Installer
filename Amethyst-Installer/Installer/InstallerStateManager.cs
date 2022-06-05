using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer
{
    /// <summary>
    /// Class that handles the installer's global state.
    /// UI updates this class' state, then this class calls all the actual logic.
    /// </summary>
    public static class InstallerStateManager
    {
        public static bool CanInstall = false;


        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsSystemSupported = true;


    }

    public enum SystemSupported
    {
        /// <summary>
        /// Refers to setups which are completely unsupported, such as Shadow Cloud PC
        /// </summary>
        Unsupported = 0,
        /// <summary>
        /// Refers to setups where the user has all the hardware necessary setup, along-side all the software components such as drivers and WASDK
        /// </summary>
        Supported = 1,
        /// <summary>
        /// Refers to setups where the user has all the hardware necessary setup and ready to go, but is missing at least one component such as drivers or WASDK
        /// </summary>
        RequiresSetup = 2,
    }

    public enum InstallerState
    {
        /// <summary>
        /// Welcome
        /// </summary>
        Welcome = 0,
        /// <summary>
        /// What to install
        /// </summary>
        InstallOptions = 1,
        /// <summary>
        /// Destination
        /// </summary>
        InstallDestination = 2,
        /// <summary>
        /// System requirements
        /// </summary>
        SystemRequirements = 3,
        /// <summary>
        /// Downloading
        /// </summary>
        Downloading = 4,
        /// <summary>
        /// Installation
        /// </summary>
        Installation = 5,
        /// <summary>
        /// Install completed
        /// </summary>
        Done = 6,
    }
}
