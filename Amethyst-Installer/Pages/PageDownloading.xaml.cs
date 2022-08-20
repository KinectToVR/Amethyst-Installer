using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using TimeoutClock = System.Timers.Timer;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDownloading.xaml
    /// </summary>
    public partial class PageDownloading : UserControl, IInstallerPage {

        private DownloadItem m_currentProgressControl;

        private TimeoutClock m_timer;
        private long m_lastTotalBytesDownloaded = 0;
        private long m_totalBytesDownloaded = 0;
        private long m_transferSpeed = 0;

        public PageDownloading() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Downloading;
        }

        public string GetTitle() {
            return Localisation.Page_Download_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            m_timer.Stop();
            MainWindow.Instance.sidebar_download.State = Controls.TaskState.Checkmark;
            SoundPlayer.PlaySound(SoundEffect.MoveNext);
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }

        public async void OnSelected() {

            MainWindow.Instance.sidebar_download.State = Controls.TaskState.Busy;

            m_timer = new TimeoutClock(1000); // Update every second
            m_timer.Elapsed += Timer_Elapsed;
            m_timer.Start();

            /*

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
                    downloadItem.ErrorMessage = isCriticalError ? Localisation.Download_FailureCritical : Localisation.Download_Failure;
                    downloadItem.IsErrorCritical = isCriticalError;
                    downloadItem.DownloadFailed = true;
                } else {
                    downloadItem.DownloadedBytes = 0;
                }

                downloadContent.Children.Add(downloadItem);
            }

            */


            // Populate install shit
            for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {

                var moduleToInstall = InstallerStateManager.ModulesToInstall[i];

                DownloadItem downloadItem = new DownloadItem();
                downloadItem.Margin = new Thickness(0, 0, 0, 12);
                downloadItem.Title = moduleToInstall.DisplayName;
                downloadItem.DownloadedBytes = 0;
                downloadItem.TotalBytes = moduleToInstall.DownloadSize;
                
                downloadItem.Tag = moduleToInstall;
                downloadItem.OnRetry += downloadModule_Retry;

                downloadContent.Children.Add(downloadItem);
            }

            // DownloadModule(0).GetAwaiter().GetResult();
            await DownloadModule(0);

        }

        private async Task DownloadModule(int index) {

            Logger.Info(index);

            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];
            m_currentProgressControl = ( DownloadItem ) downloadContent.Children[index];
            m_currentProgressControl.IsPending = false;
            m_currentProgressControl.DownloadFailed = false;
            m_currentProgressControl.IsErrorCritical = moduleToInstall.IsCritical;
            m_currentProgressControl.Completed = false;
            m_currentProgressControl.DownloadedBytes = 0;
            m_currentProgressControl.Tag = index;

            // Reset download progress state
            m_timer.Stop();
            m_timer.Start();
            m_lastTotalBytesDownloaded = 0;
            m_totalBytesDownloaded = 0;
            m_transferSpeed = 0;

            try {
                await Download.DownloadFileAsync(moduleToInstall.Remote.MainUrl, moduleToInstall.Remote.Filename, Constants.AmethystTempDirectory, DownloadModule_ProgressCallback, 30.0f);
            } catch ( OperationCanceledException ) {
                OnDownloadFailed(index);
            } catch ( TimeoutException ) {
                OnDownloadFailed(index);
            }
        }

		private async void downloadModule_Retry(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);

            // Attempt redownload

            // TODO: Track retry attempts
            DownloadItem downloadItem = (DownloadItem) sender;
            await DownloadModule(( int ) downloadItem.Tag);

        }

        // Progress update
        public void DownloadModule_ProgressCallback(long value) {
            if ( m_currentProgressControl != null ) {

                m_currentProgressControl.DownloadedBytes = value;
                m_totalBytesDownloaded = value;
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {

            // Calculate the transfer speed every second
            m_transferSpeed = m_totalBytesDownloaded - m_lastTotalBytesDownloaded;
            m_lastTotalBytesDownloaded = m_totalBytesDownloaded;

            // Force update the UI on the UI thread
            if ( m_currentProgressControl != null)
                m_currentProgressControl.Dispatcher.Invoke(() => m_currentProgressControl.TransferSpeed = m_transferSpeed);

        }

        private void OnDownloadFailed(int index) {

            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];

            if ( moduleToInstall.IsCritical ) {
                Logger.Fatal($"Critical download \"{moduleToInstall.Remote.Filename}\" timed out! Halting downloads...");
            } else {
                Logger.Error($"Download \"{moduleToInstall.Remote.Filename}\" timed out!");
            }

            m_currentProgressControl.DownloadFailed = true;
            m_currentProgressControl.IsPending = false;
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
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void button_Click(object sender, RoutedEventArgs e) {
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }
	}
}
