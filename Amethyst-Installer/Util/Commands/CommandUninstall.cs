using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace amethyst_installer_gui.Commands {
    public class CommandUninstall : ICommand {
        public string Command { get => "uninstall"; set { } }
        public string Description { get => "Starts the uninstall workflow"; set { } }
        public string[] Aliases { get => new string[] { "x" }; set { } }

        public bool Execute(ref string[] parameters) {
            // @TODO: Rework whenever we have a better upgrade workflow

            // App.InitialPage = Installer.InstallerState.Uninstall;

            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if ( InstallUtil.IsAmethystInstalledInDirectory(executingDirectory) ) {
                // If the installer is running from an existing amethyst install, make it copy itself to temp, then 
                // execute the installer from temp, passing the current arguments through, and terminate this process so that this file is free
                string tempAmeInstallerPath = Path.Combine(Constants.AmethystTempDirectory, "Amethyst-Installer.exe");
                File.Copy(Assembly.GetExecutingAssembly().Location, tempAmeInstallerPath, true);
                Process.Start(new ProcessStartInfo() {
                    FileName = tempAmeInstallerPath,
                    Arguments = string.Join("\" \"", App.Arguments),
                    WorkingDirectory = Constants.AmethystTempDirectory
                });
                Util.Quit(ExitCodes.Command);
                return true;
            }

            App.Init();
            UninstallUtil.UninstallAmethyst();
            Util.Quit(ExitCodes.Command);

            return true;
        }
    }
}