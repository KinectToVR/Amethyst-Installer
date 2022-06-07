using amethyst_installer_gui.Installer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Pages
{
    /// <summary>
    /// Interaction logic for PageException.xaml
    /// </summary>
    public partial class PageException : UserControl, IInstallerPage
    {
        public Exception currentException;

        public PageException()
        {
            InitializeComponent();
        }

        public InstallerState GetInstallerState()
        {
            return InstallerState.Exception;
        }

        public string GetTitle()
        {
            return Properties.Resources.Page_Exception_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e)
        {
            // Exit
            Application.Current.Shutdown(1);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e)
        {
            // Open Discord
            Process.Start(Constants.DiscordInvite);
        }

        public void OnButtonTertiary(object sender, RoutedEventArgs e)
        {
            // Copy Error
            Clipboard.SetText($"```\nUnhandled Exception: {currentException.GetType().Name} in {currentException.Source}: {currentException.Message}\n```");
        }

        public void OnFocus()
        {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Exit;

            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonSecondary.Content = Properties.Resources.Installer_Action_Discord;

            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Content = Properties.Resources.Installer_Action_CopyError;

            exceptionTraceBox.Text = $"Unhandled Exception: {currentException.GetType().Name} in {currentException.Source}: {currentException.Message}\n{currentException.StackTrace}";

            logsPathTextPre.Content = Properties.Resources.Logs_DirectoryIsLocatedHere + " ";
            logsPathLink.Text = Constants.AmethystLogsDirectory;
        }

        // Since we never know when we will hit this page (this page is a special case as it's exception handling) we'll handle everything on page focus
        public void OnSelected() {}

        private void logsPathLink_Click(object sender, RoutedEventArgs e)
        {
            // open logs dir with the current log file selected
            Process.Start("explorer.exe", $"/select,{Logger.LogFilePath}");
        }
    }
}
