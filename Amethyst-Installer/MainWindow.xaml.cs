using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Dictionary<InstallerState, IInstallerPage> Pages = new Dictionary<InstallerState, IInstallerPage>();
        // Basically an undo buffer ; max of 5 steps
        private List<InstallerState> pageStack = new List<InstallerState>(5);
        private int pageStackPointer = 0;

        /// <summary>
        /// Returns the current instance of the <see cref="MainWindow"/>
        /// </summary>
        public static MainWindow Instance { get { return (Application.Current.MainWindow as MainWindow); } }

        public MainWindow()
        {
            InitializeComponent();

            // Init pages
            Pages.Add(InstallerState.Welcome, new PageWelcome());
            Pages.Add(InstallerState.InstallOptions, new PageInstallOptions());
            Pages.Add(InstallerState.InstallDestination, new PageInstallDestination());
            Pages.Add(InstallerState.SystemRequirements, new PageSystemRequirements());
            Pages.Add(InstallerState.Downloading, new PageDownloading());
            Pages.Add(InstallerState.Installation, new PageInstallation());
            Pages.Add(InstallerState.Done, new PageDone());

            Pages.Add(InstallerState.Logs, new PageLogs());
            Pages.Add(InstallerState.EULA, new PageEULA());
            Pages.Add(InstallerState.Exception, new PageException());

            // Set default page to welcome
            SetPage(InstallerState.Welcome);
        }

        #region Win UI 3 Window Functionality

        // Dragging
        private void Titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
        private void Titlebar_MouseRightUp(object sender, MouseButtonEventArgs e)
        {
            SystemCommands.ShowSystemMenu(this, PointToScreen(Mouse.GetPosition(this)));
        }

        // Titlebar buttons
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Logger.Info($"User closed installer!");
            Close();
        }

        private void Minimise_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region Installer pages

        public IInstallerPage CurrentInstallerPage
        {
            get { return (IInstallerPage)GetValue(CurrentInstallerPageProperty); }
            set { SetValue(CurrentInstallerPageProperty, value); }
        }

        public static readonly DependencyProperty CurrentInstallerPageProperty =
            DependencyProperty.Register("CurrentInstallerPage", typeof(IInstallerPage), typeof(MainWindow), new PropertyMetadata(null, new PropertyChangedCallback(CurrentInstallerPageChanged)));


        private static void CurrentInstallerPageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var mainWindowInstance = (d as MainWindow);
            var newPage = (IInstallerPage)e.NewValue;

            // Reset action button state to enabled on page load
            mainWindowInstance.ActionButtonPrimary.IsEnabled = true;
            mainWindowInstance.ActionButtonSecondary.IsEnabled = true;
            mainWindowInstance.ActionButtonTertiary.IsEnabled = true;

            // Set new page to view container
            mainWindowInstance.PageView.Content = newPage;
            newPage.OnFocus();
            mainWindowInstance.windowTitle.Text = newPage.GetTitle();

            // Reset if not logs
            if (newPage.GetInstallerState() != InstallerState.Logs)
                mainWindowInstance.viewBntCount = 0;
        }

        public void SetPage(InstallerState destinatonTab)
        {
            CurrentInstallerPage = Pages[destinatonTab];
            Logger.Info($"Changing installer page to {destinatonTab}");
            CurrentInstallerPage.OnSelected();

            // Reset the stack
            pageStack.Clear();
            pageStackPointer = 0;
            pageStack.Add(destinatonTab);

            // Update checkmarks in sidebar
            sidebar_welcome.State =         destinatonTab < InstallerState.Welcome ?                TaskState.Default : TaskState.Checkmark;
            sidebar_installOptions.State =  destinatonTab < InstallerState.InstallOptions ?         TaskState.Default : TaskState.Checkmark;
            sidebar_location.State =        destinatonTab < InstallerState.InstallDestination ?     TaskState.Default : TaskState.Checkmark;
            sidebar_sysreq.State =          destinatonTab < InstallerState.SystemRequirements ?     TaskState.Default : TaskState.Checkmark;
            sidebar_download.State =        destinatonTab < InstallerState.Downloading ?            TaskState.Default : TaskState.Checkmark;
            sidebar_install.State =         destinatonTab < InstallerState.Installation ?           TaskState.Default : TaskState.Checkmark;
            sidebar_done.State =            destinatonTab < InstallerState.Done ?                   TaskState.Default : TaskState.Checkmark;
        }

        // Used to take flow from current to some other page
        public void OverridePage(InstallerState target)
        {
            if (pageStack.Count > 0 && pageStack[pageStackPointer] == InstallerState.Logs)
                return;
            pageStack.Add(target);
            Logger.Info($"Overriding view to page {target}...");
            pageStackPointer++;
            CurrentInstallerPage = Pages[target];
        }
        // Goes down the page stack
        public void GoToLastPage()
        {
            pageStackPointer--;
            CurrentInstallerPage = Pages[pageStack[pageStackPointer]];
            Logger.Info($"Changing view to previous page {pageStack[pageStackPointer]}...");
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e)
        {
            CurrentInstallerPage.OnButtonPrimary(sender, e);
        }
        private void ActionButtonSecondary_Click(object sender, RoutedEventArgs e)
        {
            CurrentInstallerPage.OnButtonSecondary(sender, e);
        }
        private void ActionButtonTertiary_Click(object sender, RoutedEventArgs e)
        {
            CurrentInstallerPage.OnButtonTertiary(sender, e);
        }

        #endregion

        int viewBntCount = 0;

        string[] altLogsBtnStrings = new string[]
                {
                    "Chungus Bungus",
                    "Among Us",
                    "LOGS LOGS LOGS LOGS LOGS LOGS LOGS LOGS LOGS LOGS LOGS LOGS",
                };
        bool altLogsBtnTxtActive = false;

        private void viewLogsBtn_Click(object sender, RoutedEventArgs e)
        {
            if (altLogsBtnTxtActive == false && viewBntCount > 2 && pageStack[pageStackPointer] == InstallerState.Logs)
            {
                altLogsBtnTxtActive = true;
                viewLogsBtn.Content = altLogsBtnStrings[new Random().Next(0, altLogsBtnStrings.Length)];
            }
            viewBntCount++;
            OverridePage(InstallerState.Logs);
        }
    }
}
