namespace amethyst_installer_gui.Installer
{
    public static class LogStrings
    {
        // amethyst
        public const string ExtractingAmethyst                  = "Extracting Amethyst to {0}...";
        public const string ExtractAmethystFailed               = "Failed to extract Amethyst";
        public const string RegisteringAmethystDriver           = "Registering Amethyst driver...";
        public const string CheckingAmethystDriverConflicts     = "Checking for conflicting SteamVR drivers...";
        public const string CreatingAmethystRegistryEntry       = "Creating registry entry...";
        public const string CreatingUninstallExecutable         = "Creating uninstall executable...";
        public const string CreateUninstallExecutableFail       = "Failed to create uninstall executable";
        public const string CreatingUninstallEntry              = "Registering uninstall entry...";
        public const string CreateUninstallEntryFailed          = "Failed to register uninstall entry";
        public const string RemovingConflictingTrackerRoles     = "Removing conflicting tracker roles...";
        public const string FailRemoveConflictingTrackerRoles   = "Failed to remove conflicting tracker roles";
        public const string AssigningTrackerRoles               = "Assigning tracker roles...";
        public const string FailAssignTrackerRoles              = "Failed to assign tracker roles";
        public const string CreatingStartMenuEntry              = "Creating start menu entry...";
        public const string FailedCreateStartMenuEntry          = "Failed to create start menu entry";
        public const string CreatingDesktopEntry                = "Creating desktop shortcut...";
        public const string FailedCreateDesktopEntry            = "Failed to create desktop shortcut";
        public const string InstalledAmethystSuccess            = "Successfully installed Amethyst!";

        // exe-module
        public const string InstallingExe                       = "Installing {0}...";
        public const string FailedInstallExe                    = "Failed to install {0}";
        public const string FailedExeInvalidExitCode            = "{0} returned unexpected exit code {1}!";
        public const string InstallExeSuccess                   = "Successfully installed {0}!";

        // extract-archive
        public const string ExtractingArchive                   = "Extracting archive {0}...";
        public const string ExtractingArchiveSuccess            = "Successfully extracted {0}!...";
        public const string FailedExtractArchive                = "Failed to extract {0}";

        // dark-extract
        public const string ExtractingDark                      = "Extracting files from {0}...";
        public const string ExtractDarkSuccess                  = "Successfully extracted files from {0}!";
        public const string FailedExtractDark                   = "Failed to extract files from {0}";
        public const string InstallingDark                      = "Installing {0}...";
        public const string InstallDarkSuccess                  = "Successfully installed {0}!";
        public const string FailedInstallDark                   = "Failed to install {0}";

        // Kinect Microphone status
        public const string CheckingKinectMicrophone            = "Verifying that the Kinect microphone is on...";
        public const string KinectV1MicrophoneFound             = "Xbox 360 Kinect microphone found!";
        public const string KinectV2MicrophoneFound             = "Xbox One Kinect microphone found!";
        public const string KinectMicrophoneDisabled            = "Kinect microphone is disabled! Kinect SDK will not function properly. Please enable the microphone...";

        // Kinect auto-fixes
        public const string ApplyingKinectFixes                 = "Attempting fixes...";
        // Not Powered
        public const string NotPoweredDetected                  = "Detected E_NUI_NOTPOWERED!";
        public const string NotPoweredFixed                     = "Successfully fixed E_NUI_NOTPOWERED!";
        public const string NotPoweredFixFailure                = "Failed to fix E_NUI_NOTPOWERED!";

        // common
        public const string ViewLogs                            = "Please view the logs for more info.";
        public const string WaitingForExecution                 = "Waiting for previous tasks to finish...";
    }
}
