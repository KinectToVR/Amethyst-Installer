using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public abstract class CheckBase {

        /// <summary>
        /// Returns whether this module should be ignored or not, due to it already existing. True makes the module install, false skips it
        /// </summary>
        public abstract bool CheckShouldInstall();
    }
}
