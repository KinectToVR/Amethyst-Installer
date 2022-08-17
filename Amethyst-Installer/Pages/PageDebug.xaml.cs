using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDebug.xaml
    /// </summary>
    public partial class PageDebug : UserControl, IInstallerPage {
        public PageDebug() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Debug;
        }

        public string GetTitle() {
            return "Debug";
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            SoundPlayer.PlaySound(SoundEffect.MovePrevious);
            MainWindow.Instance.GoToLastPage();
        }

        public void OnSelected() {

        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Back;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void button_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            MessageBox.Show("big chungus", "no way");
            Util.ShowMessageBox("big chungus", "no way");
        }

        private void openvrbtn_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox(OpenVRUtil.RuntimePath(), "SteamVR Runtime Dir");
        }
        private void kinectmic_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);

            // Check if the Kinect microphone is muted, and if so, prompt the user to enable it.
            if ( KinectUtil.KinectMicrophoneDisabled() ) {
                // Open sound control panel on the recording tab
                System.Diagnostics.Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");
            }

            Util.ShowMessageBox($"Disabled: {KinectUtil.KinectMicrophoneDisabled()}", "Kinect Mic Test");
        }
        private void currentuser_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox(CurrentUser.GetCurrentlyLoggedInUsername(), "Current user");
        }

        private void shadowTest_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"IsOnShadow: {CloudPCUtil.IsRunningOnShadow()}", "Shadow Status");
        }

        private void steamVRSettingsDetails_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"HMDModel: {OpenVRUtil.GetSteamVRHmdModel()}\nHMDManufacturer: {OpenVRUtil.GetSteamVRHmdManufacturer()}", "OpenVR HMD");
        }

        private void steamVRPlayspace_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            var playspaceBounds = OpenVRUtil.GetPlayspaceBounds();
            Util.ShowMessageBox($"Size: {playspaceBounds.x}, {playspaceBounds.y}", "SteamVR playspace bounds");
        }

        private void plutosphereTest_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"PlutoSphere: {CloudPCUtil.IsOnPlutoSphere()}", "PlutoSphere Status");
        }

        private void driverSearch_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Driver Search: {OpenVRUtil.GetDriverPath(driverSearchBox.Text)}", "Driver search");
        }
        private void driverAdd_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            OpenVRUtil.RegisterSteamVrDriver(driverAddBox.Text);
            Shell.OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "openvr", "openvrpaths.vrpath")));
            Util.ShowMessageBox($"Driver Registered!", "Driver search");
        }
    }
}
