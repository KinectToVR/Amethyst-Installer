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
        private Dictionary<InstallerState, IInstallerPage> Pages = new Dictionary<InstallerState, IInstallerPage>();

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
            
            // Set default page to welcome
            SetTab(InstallerState.Welcome);
        }

        #region Win UI 3 Window Functionality
        
        // Dragging
        private void Titlebar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
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
            (d as MainWindow).PageView.Content = (IInstallerPage)e.NewValue;
        }

        public void SetTab(InstallerState destinatonTab)
        {
            CurrentInstallerPage = Pages[destinatonTab];
            CurrentInstallerPage.OnSelected();
            Logger.Info($"Changing installer state to {destinatonTab}");

            windowTitle.Text = CurrentInstallerPage.GetTitle();

            // Update checkmarks in sidebar
            // IDK how we're going to handle ?s yet
            sidebar_welcome.State           = destinatonTab < InstallerState.Welcome               ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_installOptions.State    = destinatonTab < InstallerState.InstallOptions        ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_location.State          = destinatonTab < InstallerState.InstallDestination    ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_sysreq.State            = destinatonTab < InstallerState.SystemRequirements    ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_download.State          = destinatonTab < InstallerState.Downloading           ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_install.State           = destinatonTab < InstallerState.Installation          ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
            sidebar_done.State              = destinatonTab < InstallerState.Done                  ? SiderbarTaskState.Default : SiderbarTaskState.Checkmark;
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e)
        {
            // TODO: route into currentpage?
            SetTab(CurrentInstallerPage.GetInstallerState() + 1);
        }

        #endregion
    }
}
