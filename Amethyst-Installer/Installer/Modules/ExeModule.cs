using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public class ExeModule : ModuleBase {
        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            state = TaskState.Question;

            return false;
        }
    }
}
