using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDownloading.xaml
    /// </summary>
    public partial class PageDownloading : UserControl, IInstallerPage {

        private DownloadItem m_currentProgressControl;
        private bool m_nextButtonVisibile = false;

        public PageDownloading() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Downloading;
        }

        public string GetTitle() {
            return Localisation.Manager.Page_Download_Title;
        }

        public void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            Util.HandleKeyboardFocus(e);

            if ( MainWindow.HandleSpeedrun() ) {
                DownloadManager.Stop();

                // Advance to next page
                MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;
                SoundPlayer.PlaySound(SoundEffect.MoveNext);
                MainWindow.Instance.SetPage(InstallerState.Installation);
            }
        }

        public void OnSelected() {
            MainWindow.Instance.sidebar_download.State = TaskState.Busy;

            // Reset progress bar
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            // Populate install shit
            for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {

                var moduleToInstall = InstallerStateManager.ModulesToInstall[i];

                DownloadItem downloadItem = new DownloadItem();
                downloadItem.Margin = new Thickness(0, 0, 0, 12);
                
                // Last item should have 0 margin
                if ( i == InstallerStateManager.ModulesToInstall.Count - 1 )
                    downloadItem.Margin = new Thickness(0);

                downloadItem.Title = InstallerStateManager.ModuleStrings[moduleToInstall.Id].Title;
                downloadItem.DownloadedBytes = 0;
                downloadItem.TotalBytes = moduleToInstall.DownloadSize;

                downloadItem.Tag = moduleToInstall;
                downloadItem.OnRetry += downloadModule_Retry;

                downloadContent.Children.Add(downloadItem);
            }

            // Setup events
            DownloadManager.OnDownloadingNewModule += DownloadNewModule;
            DownloadManager.OnTransferSpeedChanged += TransferSpeedChanged;
            DownloadManager.OnInvalidChecksum += OnInvalidChecksum;
            DownloadManager.OnDownloadFailed += DownloadFailed;
            DownloadManager.OnDownloadProgressChanged += DownloadProgressChanged;
            DownloadManager.OnDownloadComplete += DownloadComplete;
            DownloadManager.OnAllDownloadsComplete += DownloadedAllModules;

            // Start downloading
            DownloadManager.Init();
        }

        private void DownloadNewModule(int index) {
            Dispatcher.Invoke(() => {
                InstallerStateManager.CanClose = false;
                Logger.Info(index);
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

                var moduleToInstall = InstallerStateManager.ModulesToInstall[index];
                m_currentProgressControl = ( DownloadItem ) downloadContent.Children[index];
                m_currentProgressControl.IsPending = false;
                m_currentProgressControl.DownloadFailed = false;
                m_currentProgressControl.IsErrorCritical = moduleToInstall.IsCritical;
                m_currentProgressControl.ErrorMessage = moduleToInstall.IsCritical ? Localisation.Manager.Download_FailureCritical : Localisation.Manager.Download_Failure;
                m_currentProgressControl.Completed = false;
                m_currentProgressControl.DownloadedBytes = 0;
                m_currentProgressControl.Tag = index;

                // Scroll it into view
                m_currentProgressControl.BringIntoView();
            });
        }

        private void downloadModule_Retry(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);

            // Attempt redownload
            DownloadItem downloadItem = (DownloadItem) sender;
            Logger.Info($"Retrying to download module with id {InstallerStateManager.ModulesToInstall[( int ) downloadItem.Tag].Id}...");
            InstallerStateManager.CanClose = false;
            DownloadManager.DownloadModule(( int ) downloadItem.Tag);
        }

        // Progress update
        public void DownloadProgressChanged(long value, int identifier) {
            downloadContent.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // If this callback is from the identifier associated with the current control and the current control isn't null
                var thisControl = ( DownloadItem ) downloadContent.Children[identifier];
                if ( thisControl != null ) {
                    thisControl.DownloadedBytes = value;

                    // Update taskbar progress
                    if ( identifier == DownloadManager.DownloadIndex ) {
                        MainWindow.Instance.taskBarItemInfo.ProgressValue = thisControl.DownloadedBytes / ( double ) thisControl.TotalBytes;
                    }
                }
            });
        }

        private void DownloadComplete() {
            downloadContent.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // There is a VERY slim chance of a file not setting downloaded bytes to total bytes due to multithreading
                // Hence we set it here to be safe
                m_currentProgressControl.DownloadedBytes = m_currentProgressControl.TotalBytes;
                m_currentProgressControl.Completed = true;
            });
        }

        private void DownloadedAllModules() {
            downloadContent.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;
                InstallerStateManager.CanClose = false;

                m_nextButtonVisibile = true;
                ActionButtonPrimary.Visibility = Visibility.Visible;
                MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;

                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 1.0;
            });
        }

        private void TransferSpeedChanged() {
            try {
                // Force update the UI on the UI thread
                if ( m_currentProgressControl != null )
                    m_currentProgressControl.Dispatcher.Invoke(() => m_currentProgressControl.TransferSpeed = DownloadManager.TransferSpeed);
            } catch ( Exception ex ) {
                Logger.Fatal(Util.FormatException(ex));
            }
        }

        private void OnInvalidChecksum() {
            m_currentProgressControl.Dispatcher.Invoke(() => {
                m_currentProgressControl.ErrorMessage = Localisation.Manager.Download_FailureChecksum;
                m_currentProgressControl.DownloadFailed = true;
                m_currentProgressControl.IsPending = false;

                InstallerStateManager.CanClose = true;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
            });
        }

        private void DownloadFailed() {
            m_currentProgressControl.Dispatcher.Invoke(() => {
                var moduleToInstall = InstallerStateManager.ModulesToInstall[(int)m_currentProgressControl.Tag];
                m_currentProgressControl.ErrorMessage = moduleToInstall.IsCritical ? Localisation.Manager.Download_FailureCritical : Localisation.Manager.Download_Failure;
                m_currentProgressControl.DownloadFailed = true;
                m_currentProgressControl.IsPending = false;

                InstallerStateManager.CanClose = true;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
            });
        }

        // Force only the first button to have focus
        public void OnFocus() {
            // @TODO: From here we automatically go to the installation page
#if DEBUG
            ActionButtonPrimary.Visibility = Visibility.Visible;
#else
            ActionButtonPrimary.Visibility = m_nextButtonVisibile ? Visibility.Visible : Visibility.Hidden;
#endif
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Manager.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }


        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}