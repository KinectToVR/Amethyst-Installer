using amethyst_installer_gui.Controls;

namespace amethyst_installer_gui.Installer.Modules {
    public abstract class PostBase {
        public abstract void OnPostOperation(ref InstallModuleProgress control);

    }
}
