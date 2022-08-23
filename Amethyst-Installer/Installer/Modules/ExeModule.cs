using amethyst_installer_gui.Controls;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    executableInstall.WaitForExit();
                    int[] exitCodeSuccess = new int [] { 0 };

                    // Try fetch exit code from JSON
                    if ( currentExecutable.ContainsKey("successful_exit_codes") ) {
                        exitCodeSuccess = (( JArray ) currentExecutable["successful_exit_codes"]).ToObject<int[]>();
                    }

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
