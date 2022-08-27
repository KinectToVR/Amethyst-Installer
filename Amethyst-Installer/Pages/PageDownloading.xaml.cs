using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Installer.Modules;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Shell;
using TimeoutClock = System.Timers.Timer;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDownloading.xaml
    /// </summary>
    public partial class PageDownloading : UserControl, IInstallerPage {

        private DownloadItem m_currentProgressControl;
        private int m_downloadIndex = 0;

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

        public void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            // Advance to next page
            m_timer.Stop();
            MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;
            SoundPlayer.PlaySound(SoundEffect.MoveNext);
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }

        public void OnSelected() {

            MainWindow.Instance.sidebar_download.State = TaskState.Busy;

            m_timer = new TimeoutClock(1000); // Update every second
            m_timer.Elapsed += Timer_Elapsed;
            m_timer.Start();

            // TODO: Download progress in taskbar

            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.shell.taskbariteminfo?view=netframework-4.6.2

            // Reset progress bar
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

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
            m_downloadIndex = 0;
            DownloadModule(m_downloadIndex);

        }

        private void DownloadModule(int index) {

            Logger.Info(index);
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];
            m_currentProgressControl = ( DownloadItem ) downloadContent.Children[index];
            m_currentProgressControl.IsPending = false;
            m_currentProgressControl.DownloadFailed = false;
            m_currentProgressControl.IsErrorCritical = moduleToInstall.IsCritical;
            m_currentProgressControl.Completed = false;
            m_currentProgressControl.DownloadedBytes = 0;
            m_currentProgressControl.Tag = index;

            // Scroll it into view
            m_currentProgressControl.BringIntoView();

            // Reset download progress state
            m_timer.Stop();
            m_timer.Start();
            m_lastTotalBytesDownloaded = 0;
            m_totalBytesDownloaded = 0;
            m_transferSpeed = 0;

            try {
                Task.Run(() =>
                    Download.DownloadFileAsync(moduleToInstall.Remote.MainUrl, moduleToInstall.Remote.Filename, Constants.AmethystTempDirectory, DownloadModule_ProgressCallback, OnDownloadComplete, m_downloadIndex).GetAwaiter().GetResult()
                );
            } catch ( OperationCanceledException ) {
                OnDownloadFailed(index);
            } catch ( TimeoutException ) {
                OnDownloadFailed(index);
            }
        }

		private void downloadModule_Retry(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);

            // Attempt redownload

            // TODO: Track retry attempts
            DownloadItem downloadItem = (DownloadItem) sender;
            // DownloadModule(( int ) downloadItem.Tag);
            DownloadModule(( int ) downloadItem.Tag);

        }

        // Progress update
        public void DownloadModule_ProgressCallback(long value, int identifier) {
            downloadContent.Dispatcher.Invoke(() => {

                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // If this callback is from the identifier associated with the current control and the current control isn't null
                var thisControl = ( DownloadItem ) downloadContent.Children[identifier];
                // if ( identifier == m_downloadIndex && m_currentProgressControl != null ) {
                if ( thisControl != null ) {

                    thisControl.DownloadedBytes = value;
                    m_totalBytesDownloaded = value;

                    // Update taskbar progress
                    if ( identifier == m_downloadIndex ) {
                        MainWindow.Instance.taskBarItemInfo.ProgressValue = thisControl.DownloadedBytes / ( double ) thisControl.TotalBytes;
                    }
                }
            });
        }

        private void OnDownloadComplete() {
            downloadContent.Dispatcher.Invoke(() => {

                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // There is a VERY slim chance of a file not setting downloaded bytes to total bytes due to multithreading
                // Hence we set it here to be safe
                m_currentProgressControl.DownloadedBytes = m_currentProgressControl.TotalBytes;

                // TODO: Verify checksum
                string filePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, InstallerStateManager.ModulesToInstall[( int ) m_currentProgressControl.Tag].Remote.Filename));
                if ( Util.GetChecksum(filePath) != InstallerStateManager.ModulesToInstall[m_downloadIndex].Remote.Checksum ) {

                    m_currentProgressControl.DownloadFailed = true;
                    Logger.Fatal("Invalid checksum!");
                    OnDownloadFailed(m_downloadIndex);
                    return;
                }

                m_currentProgressControl.Completed = true;
                m_downloadIndex++;

                if ( m_downloadIndex == InstallerStateManager.ModulesToInstall.Count ) {
                    ActionButtonPrimary.Visibility = Visibility.Visible;
                    MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;
                    Logger.Info("Downloaded all modules successfully!");
                } else {
                    Logger.Info("Download complete!");
                    // DownloadModule(m_downloadIndex);
                    DownloadModule(m_downloadIndex);
                }
            });
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e) {

            try {

                // Calculate the transfer speed every second
                m_transferSpeed = m_totalBytesDownloaded - m_lastTotalBytesDownloaded;
                m_lastTotalBytesDownloaded = m_totalBytesDownloaded;

                // Force update the UI on the UI thread
                if ( m_currentProgressControl != null )
                    m_currentProgressControl.Dispatcher.Invoke(() => m_currentProgressControl.TransferSpeed = m_transferSpeed);
            } catch ( Exception ex ) {
                Logger.Fatal(Util.FormatException(ex));
            }
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

            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
        }

        // Force only the first button to have focus
        public void OnFocus() {
            // From here we automatically go to the installation page
            // TODO: we need an actual downloader lmao
#if DEBUG
            ActionButtonPrimary.Visibility = Visibility.Visible;
#else
            ActionButtonPrimary.Visibility = Visibility.Hidden;
#endif
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }


        public void OnButtonPrimary(object sender, RoutedEventArgs e) {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
	}
}
