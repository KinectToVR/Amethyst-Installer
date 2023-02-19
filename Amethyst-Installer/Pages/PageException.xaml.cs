using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shell;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageException.xaml
    /// </summary>
    public partial class PageException : UserControl, IInstallerPage {
        public Exception currentException;

        public PageException() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Exception;
        }

        public string GetTitle() {
            return Localisation.Manager.Page_Exception_Title;
        }

        public void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            // Exit
            Util.Quit(ExitCodes.ExceptionUserClosed);
        }

        public void ActionButtonSecondary_Click(object sender, RoutedEventArgs e) {
            // Open Discord
            Process.Start(Constants.DiscordInvite);
            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }

        public void ActionButtonTertiary_Click(object sender, RoutedEventArgs e) {
            // Copy Error
            Clipboard.SetText($"```\nUnhandled Exception: {currentException.GetType().Name} in {currentException.Source}: {currentException.Message}\n```");
            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }

        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Manager.Installer_Action_Exit;

            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonSecondary.Content = Localisation.Manager.Installer_Action_Discord;

            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Content = Localisation.Manager.Installer_Action_CopyError;

            exceptionTraceBox.Text = Util.FormatException(currentException);

            logsPathTextPre.Content = Localisation.Manager.Logs_DirectoryIsLocatedHere + " ";
            logsPathLink.Text = Constants.AmethystLogsDirectory;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);

            // Reset taskbar progress
            MainWindow.Instance.taskBarItemInfo.ProgressState = TaskbarItemProgressState.None;
            MainWindow.Instance.taskBarItemInfo.ProgressValue = 0.0;
        }

        // Since we never know when we will hit this page (this page is a special case as it's exception handling) we'll handle everything on page focus
        public void OnSelected() { }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) {}
        public void OnButtonTertiary(object sender, RoutedEventArgs e) {}

        private void logsPathLink_Click(object sender, RoutedEventArgs e) {
            // open logs dir with the current log file selected
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Shell.OpenFolderAndSelectItem(Logger.LogFilePath);
        }
    }
}
