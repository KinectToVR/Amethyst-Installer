using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using Microsoft.NodejsTools.SharedProject;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageUpdating.xaml
    /// </summary>
    public partial class PageUpdating : UserControl, IInstallerPage {

        private void UpdateHandler () {
            if ( InstallerStateManager.ModulesToInstall == null || InstallerStateManager.ModulesToInstall.Count == 0 )
                return;
            StartDownloadSequence();
        }

        public static bool OpenAmethystOnSuccess = false;

        private DownloadItem m_downloadControl;
        private InstallModuleProgress m_installingControl;

        private int m_installedModuleCount = 0;

        #region Downloading

        private void OnAllDownloadsComplete() {
            StartUpdateSequence();
        }

        private void StartDownloadSequence() {

            MainWindow.Instance.sidebar_download.State = TaskState.Busy;

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

            // Setup events
            DownloadManager.OnDownloadingNewModule += DownloadNewModule;
            DownloadManager.OnTransferSpeedChanged += TransferSpeedChanged;
            DownloadManager.OnInvalidChecksum += DownloadInvalidChecksum;
            DownloadManager.OnDownloadFailed += DownloadFailed;
            DownloadManager.OnDownloadProgressChanged += DownloadProgressChanged;
            DownloadManager.OnDownloadComplete += DownloadComplete;
            DownloadManager.OnAllDownloadsComplete += DownloadedAllModules;

            // Start downloading
            DownloadManager.Init();
        }

        private void DownloadNewModule(int index) {
            Dispatcher.Invoke(() => {
            Logger.Info(index);
                InstallerStateManager.CanClose = false;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

                var moduleToInstall = InstallerStateManager.ModulesToInstall[index];
                m_downloadControl.Title = InstallerStateManager.ModuleStrings[moduleToInstall.Id].Title;
                m_downloadControl.TotalBytes = moduleToInstall.DownloadSize;
                m_downloadControl.DownloadedBytes = 0;
                m_downloadControl.IsPending = false;
                m_downloadControl.DownloadFailed = false;
                m_downloadControl.IsErrorCritical = moduleToInstall.IsCritical;
                m_downloadControl.ErrorMessage = moduleToInstall.IsCritical ? Localisation.Manager.Download_FailureCritical : Localisation.Manager.Download_Failure;
                m_downloadControl.Completed = false;
                m_downloadControl.Tag = index;
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
            m_downloadControl.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                if ( m_downloadControl != null ) {
                    m_downloadControl.DownloadedBytes = value;

                    // Update taskbar progress
                    if ( identifier == DownloadManager.DownloadIndex ) {
                        MainWindow.Instance.taskBarItemInfo.ProgressValue = m_downloadControl.DownloadedBytes / ( double ) m_downloadControl.TotalBytes;
                    }
                }
            });
        }

        private void DownloadComplete() {
            m_downloadControl.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                // There is a VERY slim chance of a file not setting downloaded bytes to total bytes due to multithreading
                // Hence we set it here to be safe
                m_downloadControl.DownloadedBytes = m_downloadControl.TotalBytes;
                m_downloadControl.Completed = true;
            });
        }

        private void DownloadedAllModules() {
            m_downloadControl.Dispatcher.Invoke(() => {
                // Check if we closed the app first, assume exit if instance is null
                if ( MainWindow.Instance == null )
                    return;

                MainWindow.Instance.sidebar_download.State = TaskState.Checkmark;

                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 1.0;
                
                OnAllDownloadsComplete();
            });
        }

        private void TransferSpeedChanged() {
            try {
                // Force update the UI on the UI thread
                if ( m_downloadControl != null )
                    m_downloadControl.Dispatcher.Invoke(() => m_downloadControl.TransferSpeed = DownloadManager.TransferSpeed);
            } catch ( Exception ex ) {
                Logger.Fatal(Util.FormatException(ex));
            }
        }

        private void DownloadInvalidChecksum() {
            m_downloadControl.Dispatcher.Invoke(() => {

                InstallerStateManager.CanClose = true;

                m_downloadControl.ErrorMessage = Localisation.Manager.Download_FailureChecksum;
                m_downloadControl.DownloadFailed = true;
                m_downloadControl.IsPending = false;

                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
            });
        }
        

        private void DownloadFailed() {
            m_downloadControl.Dispatcher.Invoke(() => {

                InstallerStateManager.CanClose = true;

                var moduleToInstall = InstallerStateManager.ModulesToInstall[(int)m_downloadControl.Tag];
                m_downloadControl.ErrorMessage = moduleToInstall.IsCritical ? Localisation.Manager.Download_FailureCritical : Localisation.Manager.Download_Failure;
                m_downloadControl.DownloadFailed = true;
                m_downloadControl.IsPending = false;

                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
            });
        }

        #endregion

        #region Updating

        private void StartUpdateSequence() {

            InstallerStateManager.CanClose = false;

            // Marquee progress
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            m_installingControl = new InstallModuleProgress();
            m_installingControl.Title = "...";
            m_installingControl.State = TaskState.Default;
            m_installingControl.LogInfo(LogStrings.WaitingForExecution);

            displayedItem.Content = m_installingControl;

            m_installedModuleCount = 0;

            InstallManager.OnAllModulesComplete += OnInstalledAllModules;
            InstallManager.OnModuleFailed += OnModuleFailed;
            InstallManager.OnModuleInstalled += OnModuleInstalled;
            InstallManager.Init();

            InstallModule(m_installedModuleCount);
        }

        private void InstallModule(int index) {

            InstallerStateManager.CanClose = false;

            var module = InstallerStateManager.ModulesToInstall[index];

            // Setup the control
            m_installingControl.ClearLog();
            m_installingControl.State = TaskState.Busy;
            m_installingControl.Title = InstallerStateManager.ModuleStrings[module.Id].Title;
            m_installingControl.BringIntoView();

            // Execute the install process on a separate thread
            Task.Run(() => InstallManager.InstallModule(index, ref m_installingControl));
        }

        private void OnInstalledAllModules() {
            Dispatcher.Invoke(() => {
                MainWindow.Instance.sidebar_install.State = TaskState.Checkmark;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                // SoundPlayer.PlaySound(SoundEffect.Focus);

                m_installingControl.LogInfo(LogStrings.UpdateSuccessful);

                InstallerStateManager.CanClose = true;
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

                        // Give the process time to start
                        Thread.Sleep(3000);
                    }

                    // Kill updater
                    m_installingControl.Dispatcher.Invoke(() => {

                        // If we are updating, start a separate cmd process to copy the installer to the target destination.
                        // This solves the edge case of the updater being executed from the Amethyst install directory.
                        if ( InstallerStateManager.IsUpdating ) {
                            string selfExecutable = Assembly.GetExecutingAssembly().Location;
                            var amethystInstallerExecutable = Path.GetFullPath(Path.Combine(InstallerStateManager.AmethystInstallDirectory, "Amethyst-Installer.exe"));
                            
                            // If we are updating, update the installer binary using cmd, then also clean the temp dir once we're done
                            // We only clean temp here instead of through Util.Quit(); so that we don't encounter the *slim* chance of
                            // cleaning temp while we're copying (not that it should ever occur)
                            var clearDirProc = Process.Start(new ProcessStartInfo() {
                                FileName = "cmd.exe",
                                Arguments = $"/C timeout 10 && copy /Y /B /V \"{selfExecutable}\" /B \"{amethystInstallerExecutable}\" && timeout 5 && rmdir /Q /S \"{Constants.AmethystTempDirectory}\"",
                                WindowStyle = ProcessWindowStyle.Hidden,
                                CreateNoWindow = true
                            });
                        }
                        Util.Quit(ExitCodes.UpdateSuccess, cleanTemp: !InstallerStateManager.IsUpdating);
                    });
                });
            });
        }

        private void OnModuleInstalled(TaskState state, int index) {
            Dispatcher.Invoke(() => {
                // Update UI state
                m_installingControl.State = state;

                index++;
                if ( index < InstallerStateManager.ModulesToInstall.Count )
                    InstallModule(index);
            });
        }

        private void OnModuleFailed(int index) {
            Dispatcher.Invoke(() => {

                InstallerStateManager.CanClose = true;
                m_installingControl.State = TaskState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.sidebar_install.State = TaskState.Error;
                SoundPlayer.PlaySound(SoundEffect.Error);

                Util.ShowMessageBox(string.Format(Localisation.Manager.InstallFailure_Modal_Description, m_installingControl.Title), Localisation.Manager.InstallFailure_Modal_Title, MessageBoxButton.OK);
                Util.Quit(ExitCodes.ExceptionInstall);
            });
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
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Manager.Installer_Action_Next;
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
