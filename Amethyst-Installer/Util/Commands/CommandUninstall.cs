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
        public string[] Aliases { get => new string[] { "u" }; set {  } }

        public void Execute(params string[] parameters) {
            // App.InitialPage = Installer.InstallerState.Uninstall;

            if (Util.ShowMessageBox("Are you sure you want to uninstall Amethyst?", "Uninstalling Amethyst", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                Util.ShowMessageBox("Successfully uninstalled Amethyst!", "Uninstalling Amethyst", MessageBoxButton.OK);
                UninstallUtil.UninstallAmethyst();
                // if (Util.ShowMessageBox("Would you like to remove all calibration data and settings as well?", "Uninstalling Amethyst", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                //     UninstallUtil.RemoveCalibrationData();
                // }
            }

            return;
        }
    }
}
