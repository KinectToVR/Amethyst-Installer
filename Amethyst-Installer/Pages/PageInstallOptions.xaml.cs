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

        public PageInstallOptions() {
            InitializeComponent();
            installableItemControls = new List<InstallableItem>();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.InstallOptions;
        }

        public string GetTitle() {
            return Properties.Resources.Page_InstallOptions_Title;
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

                installOptionsContainer.Children.Remove(installableItemControls[i]);
            }
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
                currentControl.Tag = currentModule;

                installOptionsContainer.Children.Add(currentControl);
                installableItemControls.Add(currentControl);

                // Select the first item, Amethyst
                if ( i == 0 ) {
                    InstallOptionMouseReleaseHandler(currentControl, null);
                }
            }
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

            // Update right hand side
            CalculateInstallSize();

            Module currentModule = selectedItem.Tag as Module;

            fullTitle.Text = currentModule.DisplayName;
            fullDescription.Text = currentModule.Description;

            // TODO: Better calculation, check dependency chain, proper storage units
            downloadSize.Content = Util.SizeSuffix(currentModule.DownloadSize);
            installSize.Content = currentModule.FileSize + " MegaFarts";
        }

        private void CalculateInstallSize() {
            // TODO: Also calculate total install size here
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
