using amethyst_installer_gui.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace amethyst_installer_gui.Installer.Modules {
    public class PsmsVdmModule : ModuleBase {

        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            // Extract PSMSEX VDM to %AME_DIR%\PSMSEX\VDM

            bool result             = true;
            bool resultNonCritical  = true;

            result = result && ExtractVDM(sourceFile, path, ref control);
            resultNonCritical = resultNonCritical && SetupPsmsSettings(path, ref control);
            resultNonCritical = resultNonCritical && CreateShortcuts(path, ref control);

            state = result ? TaskState.Checkmark : TaskState.Error;
            return true;
        }

        private bool ExtractVDM(string sourceFile, string path, ref InstallModuleProgress control) {

            string archivePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));

            // Extract VDM to it
            Logger.Info(string.Format(LogStrings.ExtractingArchive, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchive, sourceFile));

            if ( !InstallUtil.ExtractZipToDirectory(archivePath, Path.GetFullPath(Path.Combine(path, "PSMSEX"))) ) {
                // Failed to extract ZIP! Abort!
                Logger.Fatal($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}!)");
                control.LogError($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}! {LogStrings.ViewLogs}");
                return false;
            }

            Logger.Info(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));

            return true;
        }

        private bool SetupPsmsSettings(string path, ref InstallModuleProgress control) {

            // Setup settings file if it doesn't exist already, to tell VDM where PSMSEX is
            string vdmConfigPath = Path.GetFullPath(Path.Combine(path, "PSMSEX", "PSMSVirtualDeviceManager", "settings.ini"));
            if ( !File.Exists(vdmConfigPath) ) {

                Logger.Info(LogStrings.PsmsVdmDefaultSettings);
                control.LogInfo(LogStrings.PsmsVdmDefaultSettings);

                try {
                    string psmsPath = Path.GetFullPath(Path.Combine(path, "PSMSEX", "PSMoveService", "PSMoveService.exe"));
                    string contents = $"[Settings]\r\nPSMoveServiceLocation={psmsPath}";
                    File.WriteAllText(vdmConfigPath, contents);

                    Logger.Info(LogStrings.PsmsVdmDefaultSettingsSuccess);
                    control.LogInfo(LogStrings.PsmsVdmDefaultSettingsSuccess);
                } catch ( Exception ) {
                    Logger.Fatal($"{LogStrings.PsmsVdmDefaultSettingsFailure}.");
                    control.LogError($"{LogStrings.PsmsVdmDefaultSettingsFailure}! {LogStrings.ViewLogs}");
                    return false;
                }
            }

            return true;
        }

        private bool CreateShortcuts(string path, ref InstallModuleProgress control) {

            // Shortcut
            string psmsVdmDir = Path.GetFullPath(Path.Combine(Path.Combine(path, "PSMSEX", "PSMSVirtualDeviceManager")));
            string vdmExecutablePath = Path.GetFullPath(Path.Combine(path, "PSMSEX", "PSMSVirtualDeviceManager", "PSMSVirtualDeviceManager.exe"));

            // Start menu shortcut
            if ( InstallerStateManager.CreateStartMenuEntry ) {

                Logger.Info(LogStrings.CreatingStartMenuEntry);
                control.LogInfo(LogStrings.CreatingStartMenuEntry);

                try {
                    Util.CreateExecutableShortcut(
                        Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "..", "ProgramData", "Microsoft", "Windows", "Start Menu", "Programs", "PSMS Virtual Device Manager.lnk")),
                        psmsVdmDir,
                        vdmExecutablePath,
                        "Launch VDM");
                } catch ( Exception e ) {
                    control.LogError($"{LogStrings.FailedCreateStartMenuEntry}! {LogStrings.ViewLogs}");
                    Logger.Fatal($"{LogStrings.FailedCreateStartMenuEntry}:\n{Util.FormatException(e)})");
                }
            }

            // Desktop shortcut
            if ( InstallerStateManager.CreateDesktopShortcut ) {

                Logger.Info(LogStrings.CreatingDesktopEntry);
                control.LogInfo(LogStrings.CreatingDesktopEntry);

                try {
                    Util.CreateExecutableShortcut(
                        Path.GetFullPath(@"C:\Users\Public\Desktop\PSMS Virtual Device Manager.lnk"),
                        psmsVdmDir,
                        vdmExecutablePath,
                        "Launch VDM");
                } catch ( Exception e ) {
                    control.LogError($"{LogStrings.FailedCreateDesktopEntry}! {LogStrings.ViewLogs}");
                    Logger.Fatal($"{LogStrings.FailedCreateDesktopEntry}:\n{Util.FormatException(e)})");
                }
            }

            return true;
        }

        // @TODO: Nuke %Appdata%\PSMoveService
    }
}