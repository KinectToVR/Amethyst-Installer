using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageInstallOptions.xaml
    /// </summary>
    public partial class PageInstallOptions : UserControl, IInstallerPage {

        private List<InstallableItem> installableItemControls;
        private Module m_currentModule;

        private long m_currentModuleDownloadSize = 0;
        private long m_currentModuleInstallSize = 0;

        private long m_totalDownloadSize = 0;
        private long m_totalInstallSize = 0;

        /*
- Sometimes animations break, re-toggling the animation fixes it from my testing. I'm still looking for a solution for this.
- Installing Amethyst on top of an existing Amethyst install explodes the installer. This is due to no uninstall workflow existing yet.
- Visual C++ Redist isn't downloaded nor installed yet.
- You cannot install anything Kinect. This is not implemented.
        */

        public PageInstallOptions() {
            InitializeComponent();
            installableItemControls = new List<InstallableItem>();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.InstallOptions;
        }

        public string GetTitle() {
            return Localisation.Page_InstallOptions_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            SoundPlayer.PlaySound(SoundEffect.MoveNext);
            MainWindow.Instance.SetPage(InstallerState.InstallDestination);
            InstallerStateManager.ModulesToInstall.Clear();
            // Clear memory
            for (int i = 0; i < installableItemControls.Count; i++ ) {
                Module a = (Module)installableItemControls[i].Tag;
                // TODO: Queue everything that has been selected into a buffer somewhere
                if ( installableItemControls[i].Checked ) {
                    // TODO: Handle dependency chains
                    InstallerStateManager.ModulesToInstall.Add(a);
                }

            }
            installOptionsContainer.Children.Clear();
            installableItemControls.Clear();
        }

        public void OnSelected() {

            // TODO: Loop through JSON
            for (int i = 0; i < InstallerStateManager.API_Response.Modules.Count; i++ ) {

                var currentModule = InstallerStateManager.API_Response.Modules[i];
                if ( !currentModule.Visible ) {
                    continue;
                }

                var currentControl = new InstallableItem();
                currentControl.Title = currentModule.DisplayName;
                currentControl.Description = currentModule.Summary;
                currentControl.Checked = currentModule.Required;
                currentControl.Disabled = currentModule.Required;
                currentControl.Margin = new Thickness(0, 0, 0, 8);
                currentControl.OnMouseClickReleased += InstallOptionMouseReleaseHandler;
                currentControl.OnToggled += InstallOptionCheckToggledHandler;
                currentControl.Tag = currentModule;

                installOptionsContainer.Children.Add(currentControl);
                installableItemControls.Add(currentControl);

                // Select the first item, Amethyst
                if ( i == 0 ) {
                    InstallOptionMouseReleaseHandler(currentControl, null);
                }
            }
        }

        private void InstallOptionCheckToggledHandler(object sender, RoutedEventArgs e) {

            // Update right hand side
            CalculateInstallSize(m_currentModule);

            downloadSize.Content = Util.SizeSuffix(m_currentModuleDownloadSize);
            installSize.Content = Util.SizeSuffix(m_currentModuleInstallSize);

            totalDownloadSize.Content = Util.SizeSuffix(m_totalDownloadSize);
            totalInstallSize.Content = Util.SizeSuffix(m_totalInstallSize);
        }

        private void InstallOptionMouseReleaseHandler(object sender, MouseButtonEventArgs e) {

            if ( e != null )
                SoundPlayer.PlaySound(SoundEffect.Invoke);
            InstallableItem selectedItem = sender as InstallableItem;

            // Handle background
            for ( int i = 0; i < installableItemControls.Count; i++ ) {
                if ( installableItemControls[i] != selectedItem ) {
                    installableItemControls[i].Background = new SolidColorBrush(Colors.Transparent);
                }
            }

            m_currentModule = selectedItem.Tag as Module;

            // Update right hand side
            CalculateInstallSize(m_currentModule);

            fullTitle.Text = m_currentModule.DisplayName;
            fullDescription.Text = m_currentModule.Description;

            // TODO: Better calculation, check dependency chain, proper storage units
            downloadSize.Content = Util.SizeSuffix(m_currentModuleDownloadSize);
            installSize.Content = Util.SizeSuffix(m_currentModuleInstallSize);

            totalDownloadSize.Content = Util.SizeSuffix(m_totalDownloadSize);
            totalInstallSize.Content = Util.SizeSuffix(m_totalInstallSize);
        }

        private void CalculateInstallSize(Module currentModule) {

            m_currentModuleDownloadSize = 0;
            m_currentModuleInstallSize  = 0;
            m_totalDownloadSize         = 0;
            m_totalInstallSize          = 0;

            Logger.Info("Re-calculating file sizes...");

            for ( int i = 0; i < installableItemControls.Count; i++ ) {

                var module = ( Module ) installableItemControls[i].Tag;

                // Directly read the checkbox because when toggling the chexbox rather than clicking the control directly the
                // "Checked" property's state is deferred till after this method is executed, however the "itemCheckbox.IsChecked"
                // property is reliable for this page's purposes
                bool isChecked = installableItemControls[i].Disabled ?
                    true : (installableItemControls[i].itemCheckbox?.IsChecked ?? false);

                // Collect the current root module's file sizes
                if ( isChecked ) {
                    m_totalDownloadSize += module.DownloadSize;
                    m_totalInstallSize += module.FileSize;
                }

                if ( module == currentModule ) {
                    m_currentModuleDownloadSize += module.DownloadSize;
                    m_currentModuleInstallSize += module.FileSize;
                }

                // Collect the dependency module's file sizes
                for ( int j = 0; j < module.Depends.Count; j++ ) {

                    var thisModule = InstallerStateManager.API_Response.Modules[InstallerStateManager.ModuleIdLUT[module.Depends[j]]];

                    if ( isChecked ) {
                        m_totalDownloadSize += thisModule.DownloadSize;
                        m_totalInstallSize += thisModule.FileSize;

                        // For dependency in X
                        Logger.Info(thisModule.DisplayName);
                    }

                    if ( module == currentModule ) {
                        m_currentModuleDownloadSize += thisModule.DownloadSize;
                        m_currentModuleInstallSize += thisModule.FileSize;
                    }
                }

                if ( isChecked )
                    // Module :D
                    Logger.Info(module.DisplayName);
            }

        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
