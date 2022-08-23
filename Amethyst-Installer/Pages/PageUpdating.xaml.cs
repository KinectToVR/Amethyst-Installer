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
    /// Interaction logic for PageUpdating.xaml
    /// </summary>
    public partial class PageUpdating : UserControl, IInstallerPage {
        public PageUpdating() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Updating;
        }

        public string GetTitle() {
            return string.Empty;
        }

        private void StartUpdateSequence() {
            // TODO: Stub
            var downloadModule = new DownloadItem();
            downloadModule.Title = "shitty jokes";
            downloadModule.DownloadedBytes = 20;
            downloadModule.TotalBytes = 50;
            downloadModule.Completed = false;
            downloadModule.IsPending = false;
            downloadModule.DownloadFailed = false;
            displayedItem.Content = downloadModule;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {

        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = false;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;

            MainWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Collapsed;
        }

        public void OnSelected() {
            MainWindow.Instance.SetSidebarHidden(true);
            MainWindow.Instance.SetButtonsHidden(true);

            StartUpdateSequence();
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
