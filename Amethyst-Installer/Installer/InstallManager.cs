using amethyst_installer_gui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shell;
using System.Windows;
using Microsoft.NodejsTools.SharedProject;
using System.Threading;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Handles installation logic etc..., Offers callbacks for UI to react to changes in installation state.
    /// </summary>
    public static class InstallManager {

        public static Action OnInstallingNewModule;
        public static Action<TaskState, int> OnModuleInstalled;
        public static Action<int> OnModuleFailed;
        public static Action OnAllModulesComplete;

        private static int m_installedModuleCount;
        private static bool m_failedToInstall;

        /// <summary>
        /// Initializes the download manager
        /// </summary>
        public static void Init() {

            m_installedModuleCount = 0;
            m_failedToInstall = false;

            // Marquee progress
            // MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            // MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            // Create controls
            /*
             for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {

                var module = InstallerStateManager.ModulesToInstall[i];
                if ( !InstallerStateManager.ModuleTypes.ContainsKey(module.Install.Type) ) {
                    Logger.Warn($"Module of type {module.Install.Type} couldn't be found! Skipping...");
                    continue;
                }

                Logger.Info($"Queueing module {module.DisplayName} of type {module.Install.Type}...");
            }
             */

            // m_installedModuleCount = 0;

            // InstallModule(m_installedModuleCount);
        }

        public static void InstallModule(int index, ref InstallModuleProgress control) {
            var module = InstallerStateManager.ModulesToInstall[index];
            var moduleBase = InstallerStateManager.ModuleTypes[module.Install.Type];
            moduleBase.Module = module; // This is for expected behaviour
            // var control =  m_installControls[index];
            // control.ClearLog();
            // control.State = TaskState.Busy;
            // control.BringIntoView();

            if ( !InstallerStateManager.ModuleTypes.ContainsKey(module.Install.Type) ) {
                Logger.Warn($"Module of type {module.Install.Type} couldn't be found! Skipping...");
                return;
            }

            Logger.Info($"Installing module {module.DisplayName} of type {module.Install.Type}...");
            
            TaskState outState;

            // if ( OnInstallingNewModule != null )
            //     OnInstallingNewModule.Invoke();

            // Task.Run(() => {
                if ( moduleBase.Install(module.Remote.Filename, InstallerStateManager.AmethystInstallDirectory, ref control, out outState) ) {

                    // Try executing post operations
                    if ( module.Install.Post != null ) {
                        if ( InstallerStateManager.ModulePostOps.ContainsKey(module.Install.Post) ) {
                            var modulePost = InstallerStateManager.ModulePostOps[module.Install.Post];
                            modulePost.OnPostOperation(ref control);
                        } else {
                            if ( module.Install.Post.Length > 0 ) {
                                Logger.Warn($"Unknown post module {module.Install.Post}!");
                            }
                        }
                    }

                    // TODO: Handle failure

                    // ActionButtonPrimary.Dispatcher.Invoke(() => {
                        ModuleInstalled(outState);
                        // control.State = outState;
                    // });
                } else {
                    // control.Dispatcher.Invoke(() => OnModuleFailed(ref control));
                    ModuleFailed(index);
                }
            // });
        }

        private static void ModuleInstalled(TaskState state) {
            m_installedModuleCount++;

            // Install done! Tell the UI about it so that it can queue the next item.
            if ( OnModuleInstalled != null )
                OnModuleInstalled.Invoke(state, m_installedModuleCount - 1);

            if ( m_installedModuleCount == InstallerStateManager.ModulesToInstall.Count ) {
                // MainWindow.Instance.sidebar_install.State = TaskState.Checkmark;
                // MainWindow.Instance.taskBarItemInfo.ProgressValue = 0;
                // MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                // SoundPlayer.PlaySound(SoundEffect.Focus);

                // Complete!
                if ( OnAllModulesComplete != null )
                    OnAllModulesComplete.Invoke();

            } else {

                // m_installingControl.State = TaskState.Busy;


                // InstallModule(m_installedModuleCount);
            }
        }

        private static void ModuleFailed(int index) {

            if ( OnModuleFailed != null )
                OnModuleFailed.Invoke(index);

            // control.State = TaskState.Error;
            // MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            // MainWindow.Instance.sidebar_install.State = TaskState.Error;
            // SoundPlayer.PlaySound(SoundEffect.Error);
            // 
            // Util.ShowMessageBox(string.Format(Localisation.InstallFailure_Modal_Description, control.Title), Localisation.InstallFailure_Modal_Title, MessageBoxButton.OK);
            // Util.Quit(ExitCodes.ExceptionInstall);
        }
    }
}
