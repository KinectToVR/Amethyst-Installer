using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDebug.xaml
    /// </summary>
    public partial class PageDebug : UserControl, IInstallerPage {
        public PageDebug() {
            InitializeComponent();

            foreach ( var item in Enum.GetValues(typeof(SoundEffect)) ) {
                soundsBox.Items.Add(new ComboBoxItem() { Content = item.ToString(), Tag = ( SoundEffect ) item });
            }

            foreach ( var item in Enum.GetValues(typeof(InstallerState)) ) {
                desiredPageBox.Items.Add(new ComboBoxItem() { Content = item.ToString(), Tag = ( InstallerState ) item });
            }
            soundsBox.SelectedIndex = 0;
            desiredPageBox.SelectedIndex = 0;
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

        public void OnSelected() {}

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Manager.Installer_Action_Back;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
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

        private void throwException_Click(object sender, RoutedEventArgs e) {
            throw new Exception("This is an intentional exception!");
        }

        private void forceNoVrpath_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);

            // Equivalent of:
            // OpenVRUtil.s_failedToInit = false;

            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo handleField = typeof(OpenVRUtil).GetField("s_failedToInit", bindFlags);
            handleField.SetValue(null, true);
        }

        private void driverSearch_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Driver Search: {OpenVRUtil.GetDriverPath(driverSearchBox.Text)}", "Driver search");
        }
        private void driverAdd_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            OpenVRUtil.RegisterSteamVrDriver(driverAddBox.Text);
            Shell.OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Constants.Userprofile, "AppData", "Local", "openvr", "openvrpaths.vrpath")));
            Util.ShowMessageBox($"Driver Registered!", "Driver search");
        }
        private void driverRemove_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            OpenVRUtil.RemoveDriversWithName(driverRemoveBox.Text);
            Shell.OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Constants.Userprofile, "AppData", "Local", "openvr", "openvrpaths.vrpath")));
            Util.ShowMessageBox($"Driver Removed!", "Driver removal");
        }

        private void playSoundBtn_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(( SoundEffect ) ( ( ComboBoxItem ) soundsBox.SelectedItem ).Tag);
        }

        private void navigateToPageBtn_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.MoveNext);
            MainWindow.Instance.SetPage(( InstallerState ) ( ( ComboBoxItem ) desiredPageBox.SelectedItem ).Tag);
        }

        private void checkNotPowered_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Must fix E_NUI_NOTPOWERED: {KinectUtil.MustFixNotPowered()}", "Checking for E_NUI_NOTPOWERED...");
        }

        private void fixNotPowered_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            KinectUtil.FixNotPowered();
        }

        private void killProcTreeButton_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ForceKillProcess(killProcTreeBox.Text);
        }

        private void isElevated_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Is Elevated: {Util.IsCurrentProcessElevated()}", "Is process elevated");
        }

        private void fixMic_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Fixed mic: {KinectUtil.FixMicrophoneV1()}", "Fix");
        }

        private void checkNotReady_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"Must fix E_NUI_NOTREADY: {KinectUtil.MustFixNotReady()}", "Checking for E_NUI_NOTREADY...");
        }

        private void fixNotReady_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            InstallerStateManager.ForceFixNotReady = true;
            Util.ShowMessageBox($"E_NUI_NOTREADY will be fixed when attempting to install the Kinect for Windows v1.8 SDK.", "Fix Status");
        }

        private void archiveCreateButton_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            string sourceDirectory = Path.GetFullPath(archiveCreateBox.Text);
            string destinationFileName = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "archive.k2a"));
            K2Archive.CompressArchive(sourceDirectory, destinationFileName, 5);
            Util.ShowMessageBox($"Created archive archive.k2a ({new FileInfo(destinationFileName).Length} bytes)", "K2ArchiveResult");
            Shell.OpenFolderAndSelectItem(destinationFileName);
        }
        private void archiveExtractButton_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            string sourceFile = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "archive.k2a"));
            string destinationDirectory = Path.GetFullPath(archiveExtractBox.Text);
            K2Archive.ExtractArchive(sourceFile, destinationDirectory);
            Util.ShowMessageBox($"Extracted archive archive.k2a to {destinationDirectory}", "K2ArchiveResult");
            Shell.OpenFolderAndSelectItem(destinationDirectory);
        }
        private void codeIntegrityCheck_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Util.ShowMessageBox($"HVCI STATE: {NtDll.IsCodeIntegrityEnabled()}", "HVCI");
        }
        private void installUsbAudioDriver_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            System.Threading.Tasks.Task.Run(() => {
                KinectUtil.AssignGenericAudioDriver();
                Util.ShowMessageBox($"Your logs. Check them.\n\nNow.", "LOGS LOGS LOGS LOGS LOGS LOGS");
            });
        }

        private void nukeK2EX_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            if ( InstallerStateManager.K2EXDetected ) {
                Logger.Info(LogStrings.K2EXUninstallStart);

                bool result = K2EXUtil.NukeK2EX(InstallerStateManager.K2EXPath);
                if ( result ) {
                    Logger.Info(LogStrings.K2EXUninstallSuccess);
                    Util.ShowMessageBox($"Successfully uninstalled K2EX.", "Nuclear warfare status");
                } else {
                    Logger.Fatal(LogStrings.K2EXUninstallFailure);
                    Util.ShowMessageBox($"Failed to uninstall K2EX. You fucked up.", "Nuclear warfare status");
                }
            } else {
                Util.ShowMessageBox($"K2EX install could not be found.", "Nuclear warfare status");
            }
        }
    }
}
