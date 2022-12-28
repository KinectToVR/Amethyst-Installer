using amethyst_installer_gui.Controls;
using System;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Handles installation logic etc..., Offers callbacks for UI to react to changes in installation state.
    /// </summary>
    public static class InstallManager {

        public static Action<TaskState, int> OnModuleInstalled;
        public static Action<int> OnModuleFailed;
        public static Action OnAllModulesComplete;

        private static int m_installedModuleCount;

        /// <summary>
        /// Initializes the download manager
        /// </summary>
        public static void Init() {
            m_installedModuleCount = 0;
        }

        public static void InstallModule(int index, ref InstallModuleProgress control) {
            var module = InstallerStateManager.ModulesToInstall[index];
            var moduleBase = InstallerStateManager.ModuleTypes[module.Install.Type];
            moduleBase.Module = module; // This is for expected behaviour

            if ( !InstallerStateManager.ModuleTypes.ContainsKey(module.Install.Type) ) {
                Logger.Warn($"Module of type {module.Install.Type} couldn't be found! Skipping...");
                return;
            }

            Logger.Info($"Installing module {module.Id} of type {module.Install.Type}...");

            TaskState outState;
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

                ModuleInstalled(outState);
            } else {
                ModuleFailed(index);
            }
        }

        private static void ModuleInstalled(TaskState state) {
            m_installedModuleCount++;

            // Install done! Tell the UI about it so that it can queue the next item.
            if ( OnModuleInstalled != null )
                OnModuleInstalled.Invoke(state, m_installedModuleCount - 1);

            if ( m_installedModuleCount == InstallerStateManager.ModulesToInstall.Count ) {
                // Complete!
                if ( OnAllModulesComplete != null )
                    OnAllModulesComplete.Invoke();
            }
        }

        private static void ModuleFailed(int index) {
            if ( OnModuleFailed != null )
                OnModuleFailed.Invoke(index);
        }
    }
}
