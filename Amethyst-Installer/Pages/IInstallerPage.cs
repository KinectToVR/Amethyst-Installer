using amethyst_installer_gui.Installer;

namespace amethyst_installer_gui.Pages
{
    public interface IInstallerPage
    {
        InstallerState GetInstallerState();
        string GetTitle();
        void OnSelected();
    }
}
