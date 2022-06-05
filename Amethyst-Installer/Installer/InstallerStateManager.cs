using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer
{
    public class InstallerStateManager
    {
        public static bool CanInstall = false;


        // TODO: Detect if using Shadow, prevent an install because Ame doesn't support networked environments
        public static bool IsSystemSupported = true;


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
