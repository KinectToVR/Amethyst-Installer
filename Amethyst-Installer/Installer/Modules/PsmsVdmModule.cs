using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public class PsmsVdmModule : ModuleBase {

        const int VENDOR_ID     = 0x1415;
        const int PRODUCT_ID    = 0x2000;

        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            // Extract PSMSEX VDM to %AME_DIR%\PSMSEX\VDM

            string archivePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));

            // Extract VDM to it
            Logger.Info(string.Format(LogStrings.ExtractingArchive, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchive, sourceFile));

            if ( !InstallUtil.ExtractZipToDirectory(archivePath, Path.GetFullPath(Path.Combine(path, "PSMSEX"))) ) {
                // Failed to extract ZIP! Abort!
                Logger.Fatal($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}!)");
                control.LogError($"{string.Format(LogStrings.FailedExtractArchiveDiskFull, sourceFile)}! {LogStrings.ViewLogs}");

                state = TaskState.Error;
                return false;
            }

            Logger.Info(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));
            control.LogInfo(string.Format(LogStrings.ExtractingArchiveSuccess, sourceFile));

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
                    Logger.Fatal($"{string.Format(LogStrings.PsmsVdmDefaultSettingsFailure, sourceFile)}!)");
                    control.LogError($"{string.Format(LogStrings.PsmsVdmDefaultSettingsFailure, sourceFile)}! {LogStrings.ViewLogs}");
                }
            }

            // Drivers
            Logger.Info(LogStrings.PsmsInstallDrivers);
            control.LogInfo(LogStrings.PsmsInstallDrivers);
            try {
                // wdi-simple.exe -n "USB Playstation Eye Camera" -f "USB Playstation Eye Camera.inf" -m "Nam Tai E&E Products Ltd. or OmniVision Technologies, Inc." -v "5141" -p "8192" -t 1

                string libusbDir = Path.GetFullPath(Path.Combine(path, "PSMSEX", "PSMSVirtualDeviceManager", "libusb_driver"));

                var installDriversProc = Process.Start(new ProcessStartInfo() {
                    FileName = Path.Combine(libusbDir, "wdi-simple.exe"),
                    WorkingDirectory = libusbDir,
                    Arguments = "-n \"USB Playstation Eye Camera\" -f \"USB Playstation Eye Camera.inf\" -m \"Nam Tai E&E Products Ltd. or OmniVision Technologies, Inc.\" -v \"5141\" -p \"8192\" -t 1",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    StandardOutputEncoding = Encoding.UTF8,
                    StandardErrorEncoding = Encoding.UTF8,
                    CreateNoWindow = true,
                    UseShellExecute = false
                });

                var stdout = installDriversProc.StandardOutput.ReadToEnd();
                var stderr = installDriversProc.StandardError.ReadToEnd();

                if ( installDriversProc.WaitForExit(30000) ) {
                    if ( stdout.Length > 0 )
                        Logger.Info(stdout);
                    if ( stderr.Length > 0 )
                        Logger.Error(stderr);

                    if ( installDriversProc.ExitCode == 0 ) {
                        Logger.Info(LogStrings.PsmsInstallDriversSuccess);
                        control.LogInfo(LogStrings.PsmsInstallDriversSuccess);
                    } else {
                        Logger.Fatal($"{string.Format(LogStrings.PsmsInstallDriversBadCode, installDriversProc.ExitCode)}!)");
                        control.LogError($"{string.Format(LogStrings.PsmsInstallDriversBadCode, installDriversProc.ExitCode)}! {LogStrings.ViewLogs}");

                        state = TaskState.Error;
                        return false;
                    }
                } else {
                    installDriversProc.Kill();

                    if ( stdout.Length > 0 )
                        Logger.Info(stdout);
                    if ( stderr.Length > 0 )
                        Logger.Error(stderr);

                    Logger.Fatal(LogStrings.PsmsInstallDriversTimeout);
                    control.LogError($"{LogStrings.PsmsInstallDriversTimeout} {LogStrings.ViewLogs}");

                    state = TaskState.Error;
                    return false;
                }

            } catch ( Exception ex) {
                Logger.Fatal(LogStrings.PsmsInstallDriversFailure);
                Logger.Fatal(ex.ToString());
                control.LogError($"{LogStrings.PsmsInstallDriversFailure}! {LogStrings.ViewLogs}");

                state = TaskState.Error;
                return false;
            }

            state = TaskState.Checkmark;
            return true;
        }

        // @TODO: Nuke %Appdata%\PSMoveService
    }
}
