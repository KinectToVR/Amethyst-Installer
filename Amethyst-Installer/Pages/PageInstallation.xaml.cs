using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageInstallation.xaml
    /// </summary>
    public partial class PageInstallation : UserControl, IInstallerPage {

        private List<InstallModuleProgress> m_installControls;
        private int m_installedModuleCount = 0;
        private bool m_failedToInstall = false;
        private bool m_nextButtonVisible = false;

        public PageInstallation() {
            InitializeComponent();
            m_installControls = new List<InstallModuleProgress>();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Installation;
        }

        public string GetTitle() {
            return Localisation.Page_Install_Title;
        }

        public void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {

            if ( !MainWindow.HandleSpeedrun() )
                return;

            if ( m_failedToInstall ) {

                // Exit
                Util.Quit(ExitCodes.ExceptionInstall);

            } else {
                // Advance to next page
                MainWindow.Instance.sidebar_install.State = Controls.TaskState.Checkmark;
                SoundPlayer.PlaySound(SoundEffect.MoveNext);
                MainWindow.Instance.SetPage(InstallerState.Done);
            }
        }

        public void ActionButtonSecondary_Click(object sender, RoutedEventArgs e) {

            if ( !MainWindow.HandleSpeedrun() )
                return;

            // Open Discord
            Process.Start(Constants.DiscordInvite);
            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }


        public void OnSelected() {

            // Marquee progress
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
            MainWindow.Instance.sidebar_install.State = Controls.TaskState.Busy;

            InstallManager.OnAllModulesComplete += OnInstalledAllModules;
            InstallManager.OnModuleFailed += OnModuleFailed;
            InstallManager.OnModuleInstalled += OnModuleInstalled;
            InstallManager.Init();

            // Create controls
            for ( int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {

                var module = InstallerStateManager.ModulesToInstall[i];
                if ( !InstallerStateManager.ModuleTypes.ContainsKey(module.Install.Type) ) {
                    Logger.Warn($"Module of type {module.Install.Type} couldn't be found! Skipping...");
                    continue;
                }

                Logger.Info($"Installing module {module.DisplayName} of type {module.Install.Type}...");

                InstallModuleProgress installControl = new InstallModuleProgress();
                installControl.Title = module.DisplayName;
                installControl.State = TaskState.Default;
                if ( i != InstallerStateManager.ModulesToInstall.Count )
                    installControl.Margin = new Thickness(0, 0, 0, 8);
                installControl.LogInfo(LogStrings.WaitingForExecution);

                installationListContainer.Children.Add(installControl);
                m_installControls.Add(installControl);
            }

            m_installedModuleCount = 0;

            InstallModule(m_installedModuleCount);
        }

        private void InstallModule(int index) {

            // Setup the control
            var control =  m_installControls[index];
            control.ClearLog();
            control.State = TaskState.Busy;
            control.BringIntoView();
            InstallerStateManager.CanClose = false;

            // Execute the install process on a separate thread
            Task.Run(() => InstallManager.InstallModule(index, ref control));
        }

        private void OnInstalledAllModules() {
            Dispatcher.Invoke(() => {

                InstallerStateManager.CanClose = true;
                m_nextButtonVisible = true;
                ActionButtonPrimary.Visibility = Visibility.Visible;
                MainWindow.Instance.sidebar_install.State = TaskState.Checkmark;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                // SoundPlayer.PlaySound(SoundEffect.Focus);
            });
        }

        private void OnModuleInstalled(TaskState state, int index) {
            Dispatcher.Invoke(() => {
                // Update UI state
                var control = m_installControls[index];
                control.State = state;

                index++;
                if ( index < InstallerStateManager.ModulesToInstall.Count )
                    InstallModule(index);
            });
        }

        private void OnModuleFailed(int index) {

            var control = m_installControls[index];

            Dispatcher.Invoke(() => {
                InstallerStateManager.CanClose = true;
                m_failedToInstall = true;
                control.State = TaskState.Error;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
                MainWindow.Instance.sidebar_install.State = TaskState.Error;
                SoundPlayer.PlaySound(SoundEffect.Error);

                m_nextButtonVisible = true;
                ActionButtonPrimary.Visibility = Visibility.Visible;
                ActionButtonPrimary.Content = Localisation.Installer_Action_Exit;

                ActionButtonSecondary.Visibility = Visibility.Visible;
                ActionButtonSecondary.Content = Localisation.Installer_Action_Discord;

                Util.ShowMessageBox(String.Format(Localisation.InstallFailure_Modal_Description, control.Title), Localisation.InstallFailure_Modal_Title, MessageBoxButton.OK);
            });
        }

        // Force only the first button to have focus
        public void OnFocus() {
#if DEBUG
            ActionButtonPrimary.Visibility = Visibility.Visible;
#else
            ActionButtonPrimary.Visibility = m_nextButtonVisible ? Visibility.Visible : Visibility.Hidden;
#endif
            ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
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
