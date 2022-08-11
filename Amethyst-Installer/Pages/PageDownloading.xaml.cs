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
    /// Interaction logic for PageDownloading.xaml
    /// </summary>
    public partial class PageDownloading : UserControl, IInstallerPage {
        public PageDownloading() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Downloading;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Download_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }

        public void OnSelected() {

            Random rng = new Random();

            // Populate install shit
            for ( int i = 0; i < 6; i++ ) {

                DownloadItem downloadItem = new DownloadItem();
                downloadItem.Margin = new Thickness(0, 0, 0, 12);
                downloadItem.Title = $"Module {i}";
                downloadItem.DownloadedBytes = rng.Next(0, 1024 * 1024 * 499);
                downloadItem.TotalBytes = rng.Next(20, 1024 * 1024 * 500) + downloadItem.DownloadedBytes;
                
                // TODO: Replace with the struct that we'll use to generate this
                downloadItem.Tag = downloadItem.Title;
                downloadItem.OnRetry += downloadModule_Retry;

                // Module fails

                if ( i == 0 ) {
                    downloadItem.IsPending = false;
                } else if ( i == 1 ) {

                    bool isCriticalError = true;

                    downloadItem.IsPending = false;
                    downloadItem.ErrorMessage = isCriticalError ? Properties.Resources.Download_FailureCritical : Properties.Resources.Download_Failure;
                    downloadItem.IsErrorCritical = isCriticalError;
                    downloadItem.DownloadFailed = true;
                } else {
                    downloadItem.DownloadedBytes = 0;
                }

                downloadContent.Children.Add(downloadItem);
            }

        }

        private void downloadModule_Retry(object sender, RoutedEventArgs e) {

            Logger.Info(( ( DownloadItem ) sender ).Title);

        }

        // Force only the first button to have focus
        public void OnFocus() {
            // From here we automatically go to the installation page
            // TODO: we need an actual downloader lmao
#if DEBUG
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
#else
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Hidden;
#endif
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void button_Click(object sender, RoutedEventArgs e) {
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }
    }
}
