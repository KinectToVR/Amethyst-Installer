using amethyst_installer_gui.Installer;
using System.Windows;

namespace amethyst_installer_gui.Pages
{
    public interface IInstallerPage
    {
        InstallerState GetInstallerState();
        string GetTitle();

        /// <summary>
        /// Called when a page takes focus
        /// </summary>
        void OnFocus();
        /// <summary>
        /// Called when a page gets loaded. Only gets called once
        /// </summary>
        void OnSelected();

        // Button handling
        void OnButtonPrimary(object sender, RoutedEventArgs e);
        void OnButtonSecondary(object sender, RoutedEventArgs e);
        void OnButtonTertiary(object sender, RoutedEventArgs e);
    }
}
