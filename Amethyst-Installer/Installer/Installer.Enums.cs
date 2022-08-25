namespace amethyst_installer_gui.Installer {
    public enum SystemSupported {
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

    public enum InstallerState {
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
        /// <summary>
        /// Logs viewer
        /// </summary>
        Logs = 7,
        /// <summary>
        /// EULA agreement
        /// </summary>
        EULA = 8,
        /// <summary>
        /// Exception handling
        /// </summary>
        Exception = 9,

        /// <summary>
        /// Updating Screen
        /// </summary>
        Updating = 10,

        /// <summary>
        /// Uninstall Screen
        /// </summary>
        Uninstall = 11,

        /// <summary>
        /// DEBUG
        /// </summary>
        Debug = 1000,
    }
}
