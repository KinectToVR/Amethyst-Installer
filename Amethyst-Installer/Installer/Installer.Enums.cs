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
        /// Amethyst mode selection, SteamVR or OSC select basically
        /// </summary>
        AmethystModeSelection = 1,
        /// <summary>
        /// What to install
        /// </summary>
        InstallOptions = 2,
        /// <summary>
        /// Destination
        /// </summary>
        InstallDestination = 3,
        /// <summary>
        /// System requirements
        /// </summary>
        SystemRequirements = 4,
        /// <summary>
        /// Downloading
        /// </summary>
        Downloading = 5,
        /// <summary>
        /// Installation
        /// </summary>
        Installation = 6,
        /// <summary>
        /// Install completed
        /// </summary>
        Done = 7,
        /// <summary>
        /// Logs viewer
        /// </summary>
        Logs = 8,
        /// <summary>
        /// EULA agreement
        /// </summary>
        EULA = 9,
        /// <summary>
        /// Exception handling
        /// </summary>
        Exception = 10,

        /// <summary>
        /// Updating Screen
        /// </summary>
        Updating = 11,

        /// <summary>
        /// Uninstall Screen
        /// </summary>
        Uninstall = 12,

        /// <summary>
        /// K2EX Upgrade screen
        /// </summary>
        K2EXUpgrading = 13,

        /// <summary>
        /// DEBUG
        /// </summary>
        Debug = 1000,

        /// <summary>
        /// Amethyst Installer can indeed run DOOM
        /// </summary>
        DooM = 666,
    }
}
