using Microsoft.Win32;
using System;

namespace amethyst_installer_gui.Installer.Modules.Checks {
    public class CheckVcredist : CheckBase {
        public override bool CheckShouldInstall(in Module module) {

            // We check if its higher than 14.32.XXXXX.X

            try {
                var vsKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\X64");
                if ( vsKey != null ) {
                    int isInstalled = (int)vsKey.GetValue("Installed", 0);
                    return !(isInstalled == 1);
                }
            } catch ( Exception ex ) {
                Logger.Fatal(Util.FormatException(ex));
            }

            return true;
        }
    }
}
