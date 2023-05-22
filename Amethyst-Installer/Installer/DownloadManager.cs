using System;
using System.Threading.Tasks;
using TimeoutClock = System.Timers.Timer;
using System.Timers;
using System.IO;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Handles downloading logic, retrying, etc.. Offers callbacks for UI to react to changes in download state.
    /// </summary>
    public static class DownloadManager {

        public static Action<int> OnDownloadingNewModule;
        public static Action<long, int> OnDownloadProgressChanged;
        public static Action OnDownloadComplete;
        public static Action OnAllDownloadsComplete;
        public static Action OnTransferSpeedChanged;
        public static Action OnInvalidChecksum;
        public static Action OnDownloadFailed;

        public static int DownloadIndex = 0;
        public static long TotalBytesDownloaded = 0;
        public static long TransferSpeed = 0;

        private static TimeoutClock m_timer;
        private static long m_lastTotalBytesDownloaded = 0;

        public static void Init() {
            m_timer = new TimeoutClock(1000); // Update every second
            m_timer.Elapsed += Timer_Elapsed;
            m_timer.Start();

            DownloadIndex = 0;
            DownloadModule(DownloadIndex);
        }

        public static void Stop() {
            m_timer.Stop();
        }

        public static void DownloadModule(int index) {
            Logger.Info(index);
            if ( OnDownloadingNewModule != null )
                OnDownloadingNewModule.Invoke(index);

            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];

            // Reset download progress state
            m_timer.Stop();
            m_timer.Start();
            m_lastTotalBytesDownloaded = 0;
            TotalBytesDownloaded = 0;
            TransferSpeed = 0;

            try {
                Task.Run(() => {
#if !DEBUG
                    try {
#endif
                        Download.DownloadFileAsync(moduleToInstall.Remote.MainUrl, moduleToInstall.Remote.Filename, Constants.AmethystTempDirectory, DownloadModule_ProgressCallback, DownloadComplete, DownloadIndex).GetAwaiter().GetResult();
#if !DEBUG
                    } catch ( Exception e ) {
                        Logger.Fatal($"Failed to download file {moduleToInstall.Remote.Filename}!");
                        Logger.Fatal(Util.FormatException(e));
                        DownloadFailed(index);
                    }
#endif
                });
            } catch ( OperationCanceledException ) {
                DownloadFailed(index);
            } catch ( TimeoutException ) {
                DownloadFailed(index);
            }
        }

        /// <summary>
        /// Timer elapsed handling. Handles computing the transfer speed.
        /// </summary>
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e) {
            try {
                // Calculate the transfer speed every second
                TransferSpeed = TotalBytesDownloaded - m_lastTotalBytesDownloaded;
                m_lastTotalBytesDownloaded = TotalBytesDownloaded;

                // Callback
                if ( OnTransferSpeedChanged != null )
                    OnTransferSpeedChanged.Invoke();
            } catch ( Exception ex ) {
                Logger.Fatal(Util.FormatException(ex));
            }
        }

        private static void DownloadModule_ProgressCallback(long value, int identifier) {
            TotalBytesDownloaded = value;
            if ( OnDownloadProgressChanged != null )
                OnDownloadProgressChanged(value, identifier);
        }

        private static void DownloadComplete() {
            if ( OnDownloadComplete != null )
                OnDownloadComplete.Invoke();

            // Verify checksum
            string filePath = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, InstallerStateManager.ModulesToInstall[DownloadIndex].Remote.Filename));
            if ( Util.GetChecksum(filePath) != InstallerStateManager.ModulesToInstall[DownloadIndex].Remote.Checksum ) {

                Logger.Fatal("Invalid checksum!");
                if ( OnInvalidChecksum != null )
                    OnInvalidChecksum.Invoke();
                DownloadFailed(DownloadIndex);
                return;
            }
            
            DownloadIndex++;

            // Did we download all modules or are the modules we are yet to download?
            if ( DownloadIndex == InstallerStateManager.ModulesToInstall.Count ) {
                Logger.Info("Downloaded all modules successfully!");
                if ( OnAllDownloadsComplete != null )
                    OnAllDownloadsComplete.Invoke();
            } else {
                Logger.Info("Download complete!");
                DownloadModule(DownloadIndex);
            }
        }

        private static void DownloadFailed(int index) {
            var moduleToInstall = InstallerStateManager.ModulesToInstall[index];

            if ( moduleToInstall.IsCritical ) {
                Logger.Fatal($"Critical download \"{moduleToInstall.Remote.Filename}\" timed out! Halting downloads...");
            } else {
                Logger.Error($"Download \"{moduleToInstall.Remote.Filename}\" timed out!");
            }

            // @TODO: Track failure attempts, and auto-retry if under some threshold

            if ( OnDownloadFailed != null )
                OnDownloadFailed.Invoke();

            m_timer.Stop();
        }
    }
}
