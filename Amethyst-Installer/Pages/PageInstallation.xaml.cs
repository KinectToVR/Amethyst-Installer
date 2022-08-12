using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
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

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageInstallation.xaml
    /// </summary>
    public partial class PageInstallation : UserControl, IInstallerPage {
        public PageInstallation() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Installation;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Install_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            MainWindow.Instance.sidebar_install.State = Controls.TaskState.Checkmark;
            MainWindow.Instance.SetPage(InstallerState.Done);
        }

        public void OnSelected() {

            for (int i = 0; i < 10; i++ ) {

                InstallModuleProgress installControl = new InstallModuleProgress();
                installControl.Title = "amogus";
                installControl.State = ( TaskState ) (i % 5);
                if ( i != 10 )
                    installControl.Margin = new Thickness(0, 0, 0, 8);

                installControl.LogInfo("FUCK");
                installControl.LogWarning("SHIT");
                installControl.LogError("AAAA");

                installationListContainer.Children.Add(installControl);
            }
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
            MainWindow.Instance.sidebar_install.State = Controls.TaskState.Busy;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
