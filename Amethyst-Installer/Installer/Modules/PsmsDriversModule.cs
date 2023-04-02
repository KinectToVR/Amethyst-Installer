using amethyst_installer_gui.Controls;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace amethyst_installer_gui.Installer.Modules {
    public class PsmsDriversModule : ModuleBase {

        const int VENDOR_ID     = 0x1415;
        const int PRODUCT_ID    = 0x2000;

        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {
            
            bool result = SetupPsmsDrivers(path, ref control);

            state = result ? TaskState.Checkmark : TaskState.Error;
            return result;
        }

        private bool SetupPsmsDrivers(string path, ref InstallModuleProgress control) {

            // Drivers
            Logger.Info(LogStrings.PsmsInstallDrivers);
            control.LogInfo(LogStrings.PsmsInstallDrivers);

            try {
                // wdi-simple.exe -n "USB Playstation Eye Camera" -f "USB Playstation Eye Camera.inf" -m "Nam Tai E&E Products Ltd. or OmniVision Technologies, Inc." -v "5141" -p "8192" -t 1

                string libusbDir = Path.GetFullPath(Path.Combine(path, "PSMSEX", "PSMSVirtualDeviceManager", "libusb_driver"));

                // Try using this helper executable provided with PSMS VDM to install the PSEye driver
                var installDriversProc = Process.Start(new ProcessStartInfo() {
                    FileName = Path.Combine(libusbDir, "wdi-simple.exe"),
                    WorkingDirectory = libusbDir,
                    Arguments = $"-n \"USB Playstation Eye Camera\" -f \"USB Playstation Eye Camera.inf\" -m \"Nam Tai E&E Products Ltd. or OmniVision Technologies, Inc.\" -v \"{VENDOR_ID}\" -p \"{PRODUCT_ID}\" -t 1",
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

                if ( installDriversProc.WaitForExit(300000) ) {
                    if ( stdout.Length > 0 )
                        Logger.Info(stdout);
                    if ( stderr.Length > 0 )
                        Logger.Error(stderr);

                    if ( installDriversProc.ExitCode == 0 ) {
                        // Success
                        Logger.Info(LogStrings.PsmsInstallDriversSuccess);
                        control.LogInfo(LogStrings.PsmsInstallDriversSuccess);
                        return true;
                    } else {
                        // Bad exit code, no special handling for exit codes yet
                        Logger.Fatal($"{string.Format(LogStrings.PsmsInstallDriversBadCode, installDriversProc.ExitCode)}!)");
                        control.LogError($"{string.Format(LogStrings.PsmsInstallDriversBadCode, installDriversProc.ExitCode)}! {LogStrings.ViewLogs}");
                        return false;
                    }
                } else {
                    // Driver installer helper timed out, abort...
                    installDriversProc.Kill();

                    if ( stdout.Length > 0 )
                        Logger.Info(stdout);
                    if ( stderr.Length > 0 )
                        Logger.Error(stderr);

                    Logger.Fatal(LogStrings.PsmsInstallDriversTimeout);
                    control.LogError($"{LogStrings.PsmsInstallDriversTimeout} {LogStrings.ViewLogs}");
                    return false;
                }
            } catch ( Exception ex ) {
                // Something else happened, report it so that we can fix it
                Logger.Fatal(LogStrings.PsmsInstallDriversFailure);
                Logger.Fatal(ex.ToString());
                control.LogError($"{LogStrings.PsmsInstallDriversFailure}! {LogStrings.ViewLogs}");
                return false;
            }
        }
    }
}
