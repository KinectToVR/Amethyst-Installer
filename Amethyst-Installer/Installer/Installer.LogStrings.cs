namespace amethyst_installer_gui.Installer
{
    public static class LogStrings
    {
        // amethyst
        public const string ExtractingAmethyst                  = "Extracting Amethyst to {0}...";
        public const string ExtractAmethystFailed               = "Failed to extract Amethyst";
        public const string RegisteringAmethystDriver           = "Registering Amethyst SteamVR add-on...";
        public const string CheckingAmethystDriverConflicts     = "Checking for conflicting SteamVR add-ons...";
        public const string CreatingAmethystRegistryEntry       = "Creating registry entry...";
        public const string CreatingUninstallExecutable         = "Creating uninstall executable...";
        public const string CreateUninstallExecutableFail       = "Failed to create uninstall executable";
        public const string CreatingUninstallEntry              = "Registering Add/Remove Programs uninstall entry...";
        public const string CreateUninstallEntryFailed          = "Failed to register uninstall entry";
        public const string RemovingConflictingTrackerRoles     = "Removing conflicting tracker roles...";
        public const string FailRemoveConflictingTrackerRoles   = "Failed to remove conflicting tracker roles";
        public const string AssigningTrackerRoles               = "Assigning tracker roles...";
        public const string FailAssignTrackerRoles              = "Failed to assign tracker roles";
        public const string CreatingStartMenuEntry              = "Creating Start menu entry...";
        public const string FailedCreateStartMenuEntry          = "Failed to create Start menu entry";
        public const string CreatingDesktopEntry                = "Creating desktop shortcut...";
        public const string FailedCreateDesktopEntry            = "Failed to create desktop shortcut";
        public const string InstalledAmethystSuccess            = "Successfully installed Amethyst!";
        public const string CreatingUpgradeList                 = "Creating upgrade list...";
        public const string CreatingUninstallList               = "Creating uninstall list...";
        public const string CreateInstallerListsFailed          = "Failed to write installer config";
        public const string DisablingSteamVrHome                = "Disabling SteamVR home...";
        public const string FailDisableSteamVrHome              = "Failed to disable SteamVR home";
        public const string EnablingSteamVrAdvancedSettings     = "Enabling SteamVR advanced settings...";
        public const string FailEnableSteamVrAdvancedSettings   = "Failed to enable SteamVR advanced settings";
        public const string RegisteringAmethystProtocolLink     = "Registering Amethyst protocol link...";
        public const string FailRegisterAmethystProtocolLink    = "Failed to register Amethyst protocol link";
        public const string UpdateSuccessful                    = "Amethyst updated successfully!";
        public const string OpeningAmethyst                     = "Opening Amethyst...";

        // exe-module
        public const string InstallingExe                       = "Installing {0}...";
        public const string FailedInstallExe                    = "Failed to install {0}";
        public const string FailedExeInvalidExitCode            = "{0} returned unexpected exit code {1}!";
        public const string InstallExeSuccess                   = "Successfully installed {0}!";

        // extract-archive
        public const string ExtractingArchive                   = "Extracting archive {0}...";
        public const string ExtractingArchiveSuccess            = "Successfully extracted {0}!";
        public const string FailedExtractArchive                = "Failed to extract {0}";

        // dark-extract
        public const string ExtractingDark                      = "Extracting files from {0}...";
        public const string ExtractDarkSuccess                  = "Successfully extracted files from {0}!";
        public const string FailedExtractDark                   = "Failed to extract files from {0}";
        public const string InstallingDark                      = "Installing {0}...";
        public const string InstallDarkSuccess                  = "Successfully installed {0}!";
        public const string FailedInstallDark                   = "Failed to install {0}";
        public const string DarkExitCode                        = "Received exit code {0}.";

        // Kinect Microphone status
        public const string CheckingKinectMicrophone            = "Verifying that the Kinect microphone is on...";
        public const string KinectV1MicrophoneFound             = "Xbox 360 Kinect microphone found!";
        public const string KinectV2MicrophoneFound             = "Xbox One Kinect microphone found!";
        public const string KinectMicrophoneDisabled            = "The Kinect microphone is disabled! The Kinect for Windows SDK will not function properly. Please enable the microphone...";

        // Kinect auto-fixes
        public const string WaitingForDeviceApi                 = "Waiting (up to 30 seconds) for device driver update...";
        public const string ApplyingKinectFixes                 = "Attempting fixes...";
        public const string ApplyingKinectFixesSuccess          = "Successfully applied fixes!";
        public const string ApplyingKinectFixesFailure          = "Failed to apply fixes!";
        // Not Powered
        public const string TestNotPowered                      = "Checking for error: E_NUI_NOTPOWERED!";
        public const string NotPoweredDetected                  = "Detected the error: E_NUI_NOTPOWERED!";
        public const string NotPoweredFixed                     = "Successfully fixed: E_NUI_NOTPOWERED!";
        public const string NotPoweredFixFailure                = "Failed to fix: E_NUI_NOTPOWERED!";
        // Not Ready
        public const string TestNotReady                        = "Checking for error: E_NUI_NOREADY!";
        public const string NotReadyDetected                    = "Detected the error: E_NUI_NOREADY!";
        public const string NotReadyFixed                       = "Successfully fixed: E_NUI_NOREADY!";
        public const string NotReadyFixFailure                  = "Failed to fix: E_NUI_NOREADY!";
        
        // Memory Integrity
        public const string CheckingMemoryIntegrity             = "Checking if Memory Integrity is enabled...";
        public const string MemoryIntegrityEnabled              = "Memory Integrity detected! Please disable it, restart Windows, and re-run the installer.";
        
        // Drivers
        public const string DumpingDrivers                      = "Dumping Kinect for Windows drivers...";
        public const string InstallingDrivers                   = "Installing Kinect for Windows drivers...";

        public const string InstallAudioDriver                  = "Installing Kinect for Windows Audio Control...";
        public const string InstallAudioDriverSuccess           = "Successfully installed Kinect for Windows Audio Control!";
        public const string InstallAudioArrayDriver             = "Installing Kinect for Windows Audio Array Control...";
        public const string InstallAudioArrayDriverSuccess      = "Successfully installed Kinect for Windows Audio Array Control!";
        public const string InstallDeviceDriver                 = "Installing Kinect for Windows Device...";
        public const string InstallDeviceDriverSuccess          = "Successfully installed Kinect for Windows Device!";
        public const string InstallCameraDriver                 = "Installing Kinect for Windows Camera...";
        public const string InstallCameraDriverSuccess          = "Successfully installed Kinect for Windows Camera!";
        public const string InstallSecurityDriver               = "Installing Kinect for Windows Security Controller...";
        public const string InstallSecurityDriverSuccess        = "Successfully installed Kinect for Windows Security Controller!";
        public const string AssignMicrophoneDriver              = "Assigning driver \"(Generic USB Audio)\" for Kinect USB Audio!";
        public const string AssignMicrophoneDriverSuccess       = "Successfully assigned driver \"(Generic USB Audio)\" to Kinect USB Audio!";

        // common
        public const string ViewLogs                            = "Please view the logs for more info.";
        public const string WaitingForExecution                 = "Waiting on other tasks to finish...";
    }
}
