using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public abstract class PostBase {

        public abstract void OnPostOperation(ref InstallModuleProgress control);

    }
}
