using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Commands {
    public class CommandUninstall : ICommand {

        public string Command { get => "uninstall"; set { } }
        public string Description { get => "Starts the uninstall workflow"; set { } }
        public string[] Aliases { get => new string[] { "x" }; set {  } }

        public bool Execute(string parameters) {
            // @TODO: Rework whenever we have a better upgrade workflow

            // App.InitialPage = Installer.InstallerState.Uninstall;

            if (Util.ShowMessageBox("Are you sure you want to uninstall Amethyst?", "Uninstalling Amethyst", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {

                App.Init();
                UninstallUtil.UninstallAmethyst();
                Util.ShowMessageBox("Successfully uninstalled Amethyst!", "Uninstalling Amethyst", MessageBoxButton.OK);
            }

            Util.Quit(ExitCodes.Command);
            return true;
        }
    }
}
