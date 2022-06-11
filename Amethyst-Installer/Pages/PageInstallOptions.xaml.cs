using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
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
    /// Interaction logic for PageInstallOptions.xaml
    /// </summary>
    public partial class PageInstallOptions : UserControl, IInstallerPage {
        public PageInstallOptions() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.InstallOptions;
        }

        public string GetTitle() {
            return Properties.Resources.Page_InstallOptions_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            MainWindow.Instance.SetPage(InstallerState.InstallDestination);
        }

        public void OnSelected() {

        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void button_Click(object sender, RoutedEventArgs e) {
            // TODO: Replace with WinUI3-esque custom dialog box
            MessageBox.Show("big chungus", "no way");
            Util.ShowMessageBox("big chungus", "no way");
        }

        private void openvrbtn_Click(object sender, RoutedEventArgs e) {
            Util.ShowMessageBox(OpenVRUtil.RuntimePath(), "SteamVR Runtime Dir");
        }
        private void kinectmic_Click(object sender, RoutedEventArgs e) {

            // TODO: Move to sysreq
            // Check if the Kinect microphone is muted, and if so, prompt the user to enable it.
            if ( KinectUtil.KinectMicrophoneDisabled() ) {
                // Open sound control panel on the recording tab
                System.Diagnostics.Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");
            }

            Util.ShowMessageBox($"Disabled: {KinectUtil.KinectMicrophoneDisabled()}", "Kinect Mic Test");
        }
        private void currentuser_Click(object sender, RoutedEventArgs e) {
            Util.ShowMessageBox(CurrentUser.GetCurrentlyLoggedInUsername(), "Current user");
        }
    }
}
