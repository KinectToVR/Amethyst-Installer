using amethyst_installer_gui.Controls;
using System;
using System.Diagnostics;
using System.IO;

namespace amethyst_installer_gui.Installer.Modules {
    public class DarkModule : ModuleBase {
        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            bool success = ExtractFiles(sourceFile, ref control);
            success = success && InstallFiles(ref control);

            state = success ? TaskState.Checkmark : TaskState.Error;
            return success;
        }

        private bool ExtractFiles(string sourceFile, ref InstallModuleProgress control) {

            try {

                // Execute on file first
                string darkExecutablePath = Path.GetFullPath(Path.Combine(
                        Constants.AmethystTempDirectory,
                        (string)InstallerStateManager.API_Response.Modules[InstallerStateManager.ModuleIdLUT["wix"]].Install.Items[0],
                        "dark.exe"));

                string inputFileFullPath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, sourceFile));

                Logger.Info(string.Format(LogStrings.ExtractingDark, sourceFile));
                control.LogInfo(string.Format(LogStrings.ExtractingDark, sourceFile));

                // dark.exe {sourceFile} -x {outDir}
                var procStart = new ProcessStartInfo() {
                    FileName = darkExecutablePath,
                    WorkingDirectory = Constants.AmethystTempDirectory,
                    Arguments = $"{inputFileFullPath} -x {Constants.AmethystTempDirectory}",
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                };
                var proc = Process.Start(procStart);
                proc.WaitForExit();

                Logger.Info(string.Format(LogStrings.ExtractDarkSuccess, sourceFile));
                control.LogInfo(string.Format(LogStrings.ExtractDarkSuccess, sourceFile));
            } catch ( Exception e ) {

                Logger.Fatal($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}:\n{Util.FormatException(e)})");
                control.LogError($"{string.Format(LogStrings.FailedExtractDark, sourceFile)}! {LogStrings.ViewLogs}");
                return false;
            }

            return true;
        }

        private bool InstallFiles(ref InstallModuleProgress control) {

            // Execute each installer
            for ( int i = 0; i < Module.Install.Items.Count; i++ ) {
                var installFile = (string) Module.Install.Items[i];
                try {

                    string pathToInstaller = Path.Combine(Constants.AmethystTempDirectory, "AttachedContainer", installFile);

                    // msi /qn /norestart

                    Logger.Info(string.Format(LogStrings.InstallingDark, installFile));
                    control.LogInfo(string.Format(LogStrings.InstallingDark, installFile));

                    var msiExecutableStart = new ProcessStartInfo() {
                        FileName = pathToInstaller,
                        WorkingDirectory = Path.Combine(Constants.AmethystTempDirectory, "AttachedContainer"),
                        Arguments = "/qn /norestart",
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden,
                    };
                    var msiExecutable = Process.Start(msiExecutableStart);
                    msiExecutable.WaitForExit();

                    Logger.Info(string.Format(LogStrings.InstallDarkSuccess, installFile));
                    control.LogInfo(string.Format(LogStrings.InstallDarkSuccess, installFile));
                } catch ( Exception e ) {
                    Logger.Fatal($"{string.Format(LogStrings.FailedInstallDark, installFile)}:\n{Util.FormatException(e)})");
                    control.LogError($"{string.Format(LogStrings.FailedInstallDark, installFile)}! {LogStrings.ViewLogs}");
                    return false;
                }
            }

            return true;
        }
    }
}
