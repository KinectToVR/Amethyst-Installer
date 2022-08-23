namespace amethyst_installer_gui.Installer
{
    public static class LogStrings
    {
        public const string ExtractingAmethyst                  = "Extracting Amethyst to {0}...";
        public const string ExtractAmethystFailed               = "Failed to extract Amethyst";
        public const string RegisteringAmethystDriver           = "Registering Amethyst driver...";
        public const string CheckingAmethystDriverConflicts     = "Checking for conflicting SteamVR drivers...";
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

        public const string InstallingExe                       = "Installing {0}...";
        public const string FailedInstallExe                    = "Failed to install {0}";
        public const string FailedExeInvalidExitCode            = "{0} returned unexpected exit code {1}!";
        public const string InstallExeSuccess                   = "Successfully installed {0}!";

        public const string ViewLogs                            = "Please view the logs for more info.";
        public const string WaitingForExecution                 = "Waiting for previous tasks to finish...";
    }
}
