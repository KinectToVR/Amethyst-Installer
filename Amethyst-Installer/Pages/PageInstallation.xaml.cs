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
using System.Windows.Shell;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageInstallation.xaml
    /// </summary>
    public partial class PageInstallation : UserControl, IInstallerPage {

        private List<InstallModuleProgress> m_installControls;
        private int m_installedModuleCount = 0;
        private bool m_failedToInstall = false;

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

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
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

        public void OnSelected() {

            // Marquee progress
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            // Create controls
            for (int i = 0; i < InstallerStateManager.ModulesToInstall.Count; i++ ) {

                var module = InstallerStateManager.ModulesToInstall[i];
                if ( !InstallerStateManager.ModuleTypes.ContainsKey(module.Install.Type )) {
                    Logger.Warn($"Module of type {module.Install.Type} couldn't be found! Skipping...");
                    continue;
                }
                var moduleBase = InstallerStateManager.ModuleTypes[module.Install.Type];

                Logger.Info($"Installing module {module.DisplayName} of type {module.Install.Type}...");

                InstallModuleProgress installControl = new InstallModuleProgress();
                installControl.Title = module.DisplayName;
                // installControl.State = ( TaskState ) (i % 5);
                installControl.State = TaskState.Default;
                if ( i != InstallerStateManager.ModulesToInstall.Count )
                    installControl.Margin = new Thickness(0, 0, 0, 8);
                installControl.LogInfo(LogStrings.WaitingForExecution);

                installationListContainer.Children.Add(installControl);
                m_installControls.Add(installControl);
            }

            m_installedModuleCount = 0;

            InstallModule(m_installedModuleCount);

            // TODO: Implement
            // MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
            // MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;

            // TODO: If installing Kinect 360 SDK, show EULA after extracting
            // I have no fucking clue how that will even be implemented but that is a problem for future me!
        }

        private void InstallModule(int index) {

            var module = InstallerStateManager.ModulesToInstall[index];
            var moduleBase = InstallerStateManager.ModuleTypes[module.Install.Type];
            moduleBase.Module = module; // This is for expected behaviour
            var control =  m_installControls[index];
            control.ClearLog();
            control.State = TaskState.Busy;
            control.BringIntoView();
            TaskState outState = TaskState.Question;

            Task.Run(() => {
                if ( moduleBase.Install(module.Remote.Filename, InstallerStateManager.AmethystInstallDirectory, ref control, out outState) ) {
                    // TODO: Handle failure
                    MainWindow.Instance.ActionButtonPrimary.Dispatcher.Invoke(() => {
                        OnModuleInstalled();
                        control.State = outState;
                    });
                } else {
                    control.Dispatcher.Invoke(() => OnModuleFailed(ref control));
                }
            });
        }

        private void OnModuleInstalled() {
            m_installedModuleCount++;
            if ( m_installedModuleCount == InstallerStateManager.ModulesToInstall.Count ) {
                MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
                MainWindow.Instance.sidebar_install.State = TaskState.Checkmark;
                MainWindow.Instance.taskBarItemInfo.ProgressValue = 0;
                MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
                // SoundPlayer.PlaySound(SoundEffect.Focus);
            } else {
                InstallModule(m_installedModuleCount);
            }
        }

        private void OnModuleFailed(ref InstallModuleProgress control) {
            m_failedToInstall = true;
            control.State = TaskState.Error;
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.Error;
            MainWindow.Instance.sidebar_install.State = TaskState.Error;
            SoundPlayer.PlaySound(SoundEffect.Error);

            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Exit;

            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonSecondary.Content = Localisation.Installer_Action_Discord;

            Util.ShowMessageBox(String.Format(Localisation.InstallFailure_Modal_Description, control.Title), Localisation.InstallFailure_Modal_Title, MessageBoxButton.OK);
        }

        // Force only the first button to have focus
        public void OnFocus() {
#if DEBUG
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
#else
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Hidden;
#endif
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
            MainWindow.Instance.sidebar_install.State = Controls.TaskState.Busy;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) {
            // Open Discord
            Process.Start(Constants.DiscordInvite);
            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
