using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace amethyst_installer_gui.Installer.Modules {
    public class ExeModule : ModuleBase {
        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            bool success = true;

            try {
                for ( int i = 0; i < Module.Install.Items.Count; i++ ) {
                    JObject currentExecutable = ( JObject ) Module.Install.Items[i];

                    string executableName = sourceFile;
                    if ( currentExecutable.ContainsKey("executable") ) {
                        executableName = currentExecutable["executable"].ToString();
                    }

                    ProcessStartInfo procInfo;
                    if ( currentExecutable.ContainsKey("arguments") ) {
                        procInfo = new ProcessStartInfo() {
                            FileName = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, executableName)),
                            Arguments = currentExecutable["arguments"].ToString(),
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        };
                    } else {
                        procInfo = new ProcessStartInfo() {
                            FileName = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, executableName)),
                            CreateNoWindow = true,
                            WindowStyle = ProcessWindowStyle.Hidden,
                        };
                    }
                    Logger.Info(string.Format(LogStrings.InstallingExe, Module.DisplayName));
                    control.LogInfo(string.Format(LogStrings.InstallingExe, Module.DisplayName));
                    var executableInstall = Process.Start(procInfo);
                    executableInstall.WaitForExit(5 * 60 * 1000);
                    int[] exitCodeSuccess = new int [] { 0 };

                    // Try fetch exit code from JSON
                    if ( currentExecutable.ContainsKey("successful_exit_codes") ) {
                        exitCodeSuccess = (( JArray ) currentExecutable["successful_exit_codes"]).ToObject<int[]>();
                    }

                    if ( false && exitCodeSuccess.Contains(executableInstall.ExitCode) ) {
                        Logger.Info($"Received exit code {executableInstall.ExitCode}");
                        Logger.Info(string.Format(LogStrings.InstallExeSuccess, Module.DisplayName));
                        control.LogInfo(string.Format(LogStrings.InstallExeSuccess, Module.DisplayName));
                        success = success && true;
                    } else {
                        Logger.Fatal(string.Format(LogStrings.FailedExeInvalidExitCodeTryManual, Module.DisplayName, executableInstall.ExitCode));
                        control.LogError(string.Format(LogStrings.FailedExeInvalidExitCodeTryManual, Module.DisplayName, executableInstall.ExitCode));

                        // Try executing via GUI
                        procInfo = new ProcessStartInfo() {
                            FileName = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, executableName)),
                            CreateNoWindow = false,
                            WindowStyle = ProcessWindowStyle.Normal,
                        };
                        Logger.Info(string.Format(LogStrings.InstallingExe, Module.DisplayName));
                        control.LogInfo(string.Format(LogStrings.InstallingExe, Module.DisplayName));
                        executableInstall = Process.Start(procInfo);
                        // Wait for it to create a window
                        while ( string.IsNullOrEmpty(executableInstall.MainWindowTitle) ) {
                            Thread.Sleep(100);
                            executableInstall.Refresh();
                            if ( executableInstall.HasExited )
                                break;
                        }
                        if ( !executableInstall.HasExited ) {
                            // Focus the window because people are blind
                            Kernel.FocusProcess(executableInstall);
                            executableInstall.WaitForExit(30 * 60 * 1000); // 30 mins * 60 secs * 1000ms
                        }

                        // Handle exit codes
                        if ( exitCodeSuccess.Contains(executableInstall.ExitCode) ) {
                            Logger.Info($"Received exit code {executableInstall.ExitCode}");
                            Logger.Info(string.Format(LogStrings.InstallExeSuccess, Module.DisplayName));
                            control.LogInfo(string.Format(LogStrings.InstallExeSuccess, Module.DisplayName));
                            success = success && true;
                        } else {
                            Logger.Fatal(string.Format(LogStrings.FailedExeInvalidExitCode, Module.DisplayName, executableInstall.ExitCode));
                            control.LogError(string.Format(LogStrings.FailedExeInvalidExitCode, Module.DisplayName, executableInstall.ExitCode));
                            success = success && false;
                        }
                    }
                }
            } catch ( Exception ex ) {
                Logger.Fatal($"{string.Format(LogStrings.FailedInstallExe, Module.DisplayName)}:\n{Util.FormatException(ex)})");
                control.LogError($"{string.Format(LogStrings.FailedInstallExe,Module.DisplayName)}! {LogStrings.ViewLogs}");
                success = success && false;
            }

            state = success ? TaskState.Checkmark : TaskState.Error;
            return success;
        }
    }
}
