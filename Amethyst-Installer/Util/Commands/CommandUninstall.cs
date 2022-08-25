using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.CompilerServices;

namespace amethyst_installer_gui.Commands {

    [Verb("uninstall", aliases: new string[] { "x" }, HelpText = "Starts the uninstall workflow", Hidden = false)]
    public class CommandUninstall : ICommand {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Execute() {
            // @TODO: Rework whenever we have a better upgrade workflow

            // App.InitialPage = Installer.InstallerState.Uninstall;

            if (Util.ShowMessageBox("Are you sure you want to uninstall Amethyst?", "Uninstalling Amethyst", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                UninstallUtil.UninstallAmethyst();
                Util.ShowMessageBox("Successfully uninstalled Amethyst!", "Uninstalling Amethyst", MessageBoxButton.OK);
            }

            CommandParser.HaltProgram();
            Util.Quit(ExitCodes.Command);
            return;
        }
    }
}
