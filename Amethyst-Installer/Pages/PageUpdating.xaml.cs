using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using Microsoft.NodejsTools.SharedProject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Interaction logic for PageUpdating.xaml
    /// </summary>
    public partial class PageUpdating : UserControl, IInstallerPage {

        private void UpdateHandler () {
            StartDownloadSequence();
        }

        public static bool OpenAmethystOnSuccess = false;

        private DownloadItem m_downloadControl;
        private InstallModuleProgress m_installingControl;

        private int m_downloadIndex = 0;
        private TimeoutClock m_timer;
        private long m_lastTotalBytesDownloaded = 0;
        private long m_totalBytesDownloaded = 0;
        private long m_transferSpeed = 0;

        private int m_installedModuleCount = 0;

        #region Downloading

        private void OnAllDownloadsComplete() {
            StartUpdateSequence();
        }

        private void StartDownloadSequence() {

            MainWindow.Instance.sidebar_download.State = TaskState.Busy;

            m_timer = new TimeoutClock(1000); // Update every second
            m_timer.Elapsed += Timer_Elapsed;
            m_timer.Start();

            // TODO: Download progress in taskbar

            // https://docs.microsoft.com/en-us/dotnet/api/system.windows.shell.taskbariteminfo?view=netframework-4.6.2

            // Reset progress bar
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            // Create download control
            m_downloadControl = new DownloadItem();
            m_downloadControl.Title = "...";
            m_downloadControl.DownloadedBytes = 0;
            m_downloadControl.TotalBytes = 1;
            m_downloadControl.Completed = false;
            m_downloadControl.IsPending = false;
            m_downloadControl.DownloadFailed = false;
            m_downloadControl.OnRetry += downloadModule_Retry;

            displayedItem.Content = m_downloadControl;

            m_downloadIndex = 0;
            DownloadModule(m_downloadIndex);
        }

        private void DownloadModule(int index) {

            Logger.Info(index);
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];
            m_downloadControl.Title = moduleToInstall.DisplayName;
            m_downloadControl.TotalBytes = moduleToInstall.DownloadSize;
            m_downloadControl.DownloadedBytes = 0;
            m_downloadControl.IsPending = false;
            m_downloadControl.DownloadFailed = false;
            m_downloadControl.IsErrorCritical = moduleToInstall.IsCritical;
            m_downloadControl.ErrorMessage = moduleToInstall.IsCritical ? Localisation.Download_FailureCritical : Localisation.Download_Failure;
            m_downloadControl.Completed = false;
            m_downloadControl.Tag = index;

            // Reset download progress state
            m_timer.Stop();
            m_timer.Start();
            m_lastTotalBytesDownloaded = 0;
            m_totalBytesDownloaded = 0;
            m_transferSpeed = 0;

            try {
                Task.Run(() => {
                    try {
                        Download.DownloadFileAsync(moduleToInstall.Remote.MainUrl, moduleToInstall.Remote.Filename, Constants.AmethystTempDirectory, DownloadModule_ProgressCallback, OnDownloadComplete, m_downloadIndex).GetAwaiter().GetResult();
                    } catch ( Exception e ) {
                        Logger.Fatal($"Failed to download file {moduleToInstall.Remote.Filename}!");
                        Logger.Fatal(Util.FormatException(e));
                        m_downloadControl.Dispatcher.Invoke(() => OnDownloadFailed(index));
                    }
                });
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
            Logger.Info($"Retrying to download module with id {InstallerStateManager.ModulesToInstall[( int ) downloadItem.Tag].Id}...");
            // DownloadModule(( int ) downloadItem.Tag);
            DownloadModule(( int ) downloadItem.Tag);

        }

        // Progress update
        public void DownloadModule_ProgressCallback(long value, int identifier) {
            m_downloadControl.Dispatcher.Invoke(() => {

                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // if ( identifier == m_downloadIndex && m_currentProgressControl != null ) {
                if ( m_downloadControl != null ) {

                    m_downloadControl.DownloadedBytes = value;
                    m_totalBytesDownloaded = value;

                    // Update taskbar progress
                    if ( identifier == m_downloadIndex ) {
                        MainWindow.Instance.taskBarItemInfo.ProgressValue = m_downloadControl.DownloadedBytes / ( double ) m_downloadControl.TotalBytes;
                    }
                }
            });
        }

        private void OnDownloadComplete() {
            m_downloadControl.Dispatcher.Invoke(() => {

                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // There is a VERY slim chance of a file not setting downloaded bytes to total bytes due to multithreading
                // Hence we set it here to be safe
                m_downloadControl.DownloadedBytes = m_downloadControl.TotalBytes;

                // TODO: Verify checksum
                string filePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, InstallerStateManager.ModulesToInstall[( int ) m_downloadControl.Tag].Remote.Filename));
                if ( Util.GetChecksum(filePath) != InstallerStateManager.ModulesToInstall[m_downloadIndex].Remote.Checksum ) {

                    m_downloadControl.DownloadFailed = true;
                    Logger.Fatal("Invalid checksum!");
                    OnDownloadFailed(m_downloadIndex);
                    return;
                }

                m_downloadControl.Completed = true;
                m_downloadIndex++;

                if ( m_downloadIndex == InstallerStateManager.ModulesToInstall.Count ) {
                    // ActionButtonPrimary.Visibility = Visibility.Visible;
                    MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;
                    OnAllDownloadsComplete();
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
                if ( m_downloadControl != null )
                    m_downloadControl.Dispatcher.Invoke(() => m_downloadControl.TransferSpeed = m_transferSpeed);
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

            // TODO: Track failure attempts, and auto-retry if under some threshold

            m_downloadControl.DownloadFailed = true;
            m_downloadControl.IsPending = false;

            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            m_timer.Stop();
        }

        #endregion

        #region Updating

        private void StartUpdateSequence() {

            // Marquee progress
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            m_installingControl = new InstallModuleProgress();
            m_installingControl.Title = "...";
            m_installingControl.State = TaskState.Default;
            m_installingControl.LogInfo(LogStrings.WaitingForExecution);

            displayedItem.Content = m_installingControl;

            m_installedModuleCount = 0;

            InstallModule(m_installedModuleCount);
        }

        private void InstallModule(int index) {

            var module = InstallerStateManager.ModulesToInstall[index];
            var moduleBase = InstallerStateManager.ModuleTypes[module.Install.Type];
            moduleBase.Module = module; // This is for expected behaviour

            m_installingControl.ClearLog();
            m_installingControl.State = TaskState.Busy;
            m_installingControl.Title = module.DisplayName;
            TaskState outState = TaskState.Busy;

            Task.Run(() => {
                if ( moduleBase.Install(module.Remote.Filename, InstallerStateManager.AmethystInstallDirectory, ref m_installingControl, out outState) ) {

                    // Try executing post operations
                    if ( module.Install.Post != null ) {
                        if ( InstallerStateManager.ModulePostOps.ContainsKey(module.Install.Post) ) {
                            var modulePost = InstallerStateManager.ModulePostOps[module.Install.Post];
                            modulePost.OnPostOperation(ref m_installingControl);
                        } else {
                            if ( module.Install.Post.Length > 0 ) {
                                Logger.Warn($"Unknown post module {module.Install.Post}!");
                            }
                        }
                    }

                    // TODO: Handle failure

                    m_installingControl.Dispatcher.Invoke(() => {
                        OnModuleInstalled();
                        m_installingControl.State = outState;
                    });
                } else {
                    m_installingControl.Dispatcher.Invoke(() => OnModuleFailed(ref m_installingControl));
                }
            });
        }

        private void OnModuleInstalled() {
            m_installedModuleCount++;
            if ( m_installedModuleCount == InstallerStateManager.ModulesToInstall.Count ) {
                MainWindow.Instance.sidebar_install.State = TaskState.Checkmark;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                // SoundPlayer.PlaySound(SoundEffect.Focus);

                m_installingControl.LogInfo(LogStrings.UpdateSuccessful);
                // Wait 3 seconds
                Task.Run(() => {
                    Thread.Sleep(3000);
                    if ( OpenAmethystOnSuccess ) {
                        m_installingControl.Dispatcher.Invoke(() => m_installingControl.LogInfo(LogStrings.OpeningAmethyst));

                        // Launch updater
                        SystemUtility.ExecuteProcessUnElevated(
                            Path.GetFullPath(Path.Combine(InstallerStateManager.AmethystInstallDirectory, "Amethyst.exe")),
                            "/afterupdate",
                            InstallerStateManager.AmethystInstallDirectory,
                            ShowWindow.SW_NORMAL);

                        Thread.Sleep(3000);
                    }

                    // Kill updater
                    m_installingControl.Dispatcher.Invoke(() => Util.Quit(ExitCodes.UpdateSuccess));
                });

            } else {
                m_installingControl.State = TaskState.Busy;
                InstallModule(m_installedModuleCount);
            }
        }

        private void OnModuleFailed(ref InstallModuleProgress control) {
            control.State = TaskState.Error;
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            MainWindow.Instance.sidebar_install.State = TaskState.Error;
            SoundPlayer.PlaySound(SoundEffect.Error);

            Util.ShowMessageBox(string.Format(Localisation.InstallFailure_Modal_Description, control.Title), Localisation.InstallFailure_Modal_Title, MessageBoxButton.OK);
            Util.Quit(ExitCodes.ExceptionInstall);
        }

        #endregion

        #region Boilerplate
        public PageUpdating() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Updating;
        }

        public string GetTitle() {
            return string.Empty;
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = false;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;
        }

        public void OnSelected() {
            MainWindow.Instance.SetSidebarHidden(true);
            MainWindow.Instance.SetButtonsHidden(true);

            UpdateHandler();
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        #endregion
    }
}
