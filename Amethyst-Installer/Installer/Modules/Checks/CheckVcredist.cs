using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules.Checks {
    public class CheckVcredist : CheckBase {
        public override bool CheckShouldInstall(in Module module) {

            // @TODO: Check registry at HKLM\SOFTWARE\Microsoft\VisualStudio\14.0\VC\Runtimes\X64
            // We check if its higher than 14.32.XXXXX.X

            return true;
        }
    }
}
