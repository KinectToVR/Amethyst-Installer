using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageLogs.xaml
    /// </summary>
    public partial class PageLogs : UserControl, IInstallerPage {

        private static Queue<UILogMessage> s_queuedConsoleMessages = new Queue<UILogMessage>();
        private bool scrollToBottomQueued = false;

        public PageLogs() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Logs;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Logs_Title;
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Content = Properties.Resources.Installer_Action_Back;

            logsPathTextPre.Content = Properties.Resources.Logs_DirectoryIsLocatedHere + " ";
            logsPathLink.Text = Constants.AmethystLogsDirectory;

            scrollToBottomQueued = true;
        }

        private void logsPathLink_Click(object sender, RoutedEventArgs e) {
            // open logs dir with the current log file selected
            Shell.OpenFolderAndSelectItem(Logger.LogFilePath);
        }

        internal static void LogLine(string msg, ConsoleColor color) {
            if ( MainWindow.Instance != null && s_queuedConsoleMessages.Count > 0 ) {
                // Clear the textbox first, getting rid of the first newline
                ( MainWindow.Instance.Pages[InstallerState.Logs] as PageLogs ).logMessagesBox.Document.Blocks.Clear();
                while ( s_queuedConsoleMessages.Count > 0 ) {
                    var currMessage = s_queuedConsoleMessages.Dequeue();
                    LogLineInternal(currMessage.message, currMessage.color);
                }
            }
            if ( MainWindow.Instance == null )
                s_queuedConsoleMessages.Enqueue(new UILogMessage(msg, color));
            else
                LogLineInternal(msg, color);
        }

        private static void LogLineInternal(string msg, ConsoleColor color) {
            ( MainWindow.Instance.Pages[InstallerState.Logs] as PageLogs ).logMessagesBox.Dispatcher.Invoke(() => {
                Paragraph paragraph = new Paragraph();
                Run run = new Run(msg);
                run.Foreground = Constants.ConsoleBrushColors[( int ) color];
                paragraph.Inlines.Add(run);
                ( MainWindow.Instance.Pages[InstallerState.Logs] as PageLogs ).logMessagesBox.Document.Blocks.Add(paragraph);
            });
        }

        // Since this page can be opened at any time we can't trust this function
        public void OnSelected() {
            MainWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Hidden;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) {
            MainWindow.Instance.GoToLastPage();
            SoundPlayer.PlaySound(SoundEffect.MovePrevious);
        }

        private struct UILogMessage {
            public string message;
            public ConsoleColor color;

            public UILogMessage(string msg, ConsoleColor color) : this() {
                this.message = msg;
                this.color = color;
            }
        }

        private void logMessagesBox_LayoutUpdated(object sender, EventArgs e) {
            // Yup this is what sort of cursed workaround I had to use to get the textbox to automagically scroll to the bottom on Focus()
            // This workaround is necessary as when this page has just been initialised, no layout updates have been called, therefore ScrollToBottom fails
            // This event (LayoutUpdated) gets fired AFTER the page layout has been calculated, allowing us to actually scroll to the bottom now
            if ( scrollToBottomQueued && VisualTreeHelper.GetChildrenCount(logMessagesBox) > 0 ) {
                Border border = (Border)VisualTreeHelper.GetChild(logMessagesBox, 0);
                ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
                scrollViewer.ScrollToBottom();
                scrollToBottomQueued = false;
            }
        }
    }
}
