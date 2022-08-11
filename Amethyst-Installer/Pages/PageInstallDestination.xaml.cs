using amethyst_installer_gui.Controls;
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
using Ookii.Dialogs.Wpf;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageInstallDestination.xaml
    /// </summary>
    public partial class PageInstallDestination : UserControl, IInstallerPage {

        private DriveSelectionControl currentlySelectedDriveControl;
        private Dictionary<string, DriveSelectionControl> driveControlDiskLetterPair;

        public PageInstallDestination() {
            InitializeComponent();
            driveControlDiskLetterPair = new Dictionary<string, DriveSelectionControl>();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.InstallDestination;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Location_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {

            // Ensure the directory is valid first
            try {
                var finalPath = Path.GetFullPath(pathTextbox.Text);
                DirectoryInfo dirInfo = new DirectoryInfo(finalPath);
                // Advance to next page
                MainWindow.Instance.SetPage(InstallerState.SystemRequirements);
            }
            catch (Exception) {
                // If we reach here the directory is invalid
                Util.ShowMessageBox(Properties.Resources.InstallDestination_InvalidPathDescription, Properties.Resources.InstallDestination_InvalidPathTitle, MessageBoxButton.OK);
            }
        }

        public void OnSelected() {
            // Fetch drive info
            var drives = DriveInfo.GetDrives();
            var systemDriveLetter = Path.GetPathRoot( Environment.GetFolderPath( Environment.SpecialFolder.Windows ));
            for ( int i = 0; i < drives.Length; i++ ) {

                // Make sure the drive is in a read / write state before doing anything with it
                if ( !drives[i].IsReady )
                    continue;

                var letter = drives[i].RootDirectory.ToString().Replace(Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty);
                var driveName = Shell.GetDriveLabel(drives[i].RootDirectory.ToString());
                var freeSpace = drives[i].AvailableFreeSpace;
                var totalSize = drives[i].TotalSize;

                // Create control
                var driveControl = new DriveSelectionControl();
                driveControl.DiskPercentage = 1.0 - ( ( double ) freeSpace / ( totalSize ) );
                driveControl.DiskLabel = string.Format(Properties.Resources.InstallDestination_DiskLabelFormat, driveName, letter);
                driveControl.FreeSpaceLabel = string.Format(Properties.Resources.InstallDestination_StorageFormatFree, Util.SizeSuffix(freeSpace));

                driveControl.Tag = drives[i];
                driveControl.MouseLeftButtonUp += driveSelected_Click;

                driveControl.Margin = new Thickness(4, 8, 4, 2);
                if ( drives[i].RootDirectory.ToString() == systemDriveLetter ) {
                    // If this is the boot drive, select it
                    driveControl.Selected = true;
                    currentlySelectedDriveControl = driveControl;
                    pathTextbox.Text = Path.GetFullPath(Path.Combine(drives[i].RootDirectory.ToString(), "Amethyst"));
                    // First and last drives have different padding
                } else if ( i == 0 ) {
                    driveControl.Margin = new Thickness(8, 8, 4, 2);
                } else if ( i == drives.Length - 1 )
                    driveControl.Margin = new Thickness(4, 8, 8, 2);

                drivesContainer.Children.Add(driveControl);
                driveControlDiskLetterPair.Add(drives[i].RootDirectory.ToString(), driveControl);
            }

            // If we couldn't find the boot drive, assume drive 0
            if ( currentlySelectedDriveControl == null ) {
                currentlySelectedDriveControl = driveControlDiskLetterPair[drives[0].RootDirectory.ToString()];
                currentlySelectedDriveControl.Selected = true;
                pathTextbox.Text = Path.GetFullPath(Path.Combine(drives[0].RootDirectory.ToString(), "Amethyst"));
            }
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

        private void openDirectory_Click(object sender, RoutedEventArgs e) {

            // Setup Vista Folder Dialog
            var dialog = new VistaFolderBrowserDialog();
            // 
            if ( Directory.Exists(pathTextbox.Text) || new DirectoryInfo(pathTextbox.Text).Root.Exists ) {
                // Go up a directory if the path ends with amethyst and doesn't exist
                // i.e. if the user has created an Amethyst directory at C:\Amethyst and clicks this button (assuming the textbox points to this path)
                DirectoryInfo dirInfo = new DirectoryInfo(pathTextbox.Text);
                if ( dirInfo.Name.ToLowerInvariant() == "amethyst" && !dirInfo.Exists ) {
                    dialog.SelectedPath = Path.Combine(pathTextbox.Text, "..");
                } else {
                    dialog.SelectedPath = pathTextbox.Text; // if the ame directory exists and contains ame just open it
                }
            } else {
                var drive = (DriveInfo) currentlySelectedDriveControl.Tag;
                dialog.SelectedPath = Path.Combine(drive.RootDirectory.ToString(), "Amethyst");
            }
            // Ensure the path is valid and expanded properly
            dialog.SelectedPath = Path.GetFullPath(dialog.SelectedPath);
            dialog.ShowNewFolderButton = true;

            // Dialog closed by clicking OK
            if ( dialog.ShowDialog() == true ) {
                pathTextbox.Text = dialog.SelectedPath;
                DirectoryInfo dirInfo = new DirectoryInfo(dialog.SelectedPath);
                // Check if the path includes amethyst
                if ( dirInfo.Name.ToLowerInvariant() != "amethyst" ) {
                    pathTextbox.Text = Path.Combine(dialog.SelectedPath, "Amethyst");
                }
                pathTextbox.Text = Path.GetFullPath(pathTextbox.Text);
                if ( driveControlDiskLetterPair.ContainsKey(dirInfo.Root.ToString()) ) {
                    var thisDrive = driveControlDiskLetterPair[dirInfo.Root.ToString()];
                    // Invoke left click handler because im lazy
                    thisDrive.RaiseEvent(
                        new MouseButtonEventArgs(Mouse.PrimaryDevice, Environment.TickCount, MouseButton.Left) {
                            RoutedEvent = MouseLeftButtonUpEvent,
                            Source = thisDrive
                        });
                }
            }
        }

        private void driveSelected_Click(object sender, MouseButtonEventArgs e) {
            var control = e.Source as DriveSelectionControl;
            if ( control != null && control.Selected )
                return;
            var drive = (DriveInfo) control.Tag;
            pathTextbox.Text = Path.GetFullPath(Path.Combine(drive.RootDirectory.ToString(), "Amethyst"));
            control.Selected = true;
            currentlySelectedDriveControl.Selected = false;
            currentlySelectedDriveControl = control;

            // Scroll the control into view
            currentlySelectedDriveControl.BringIntoView();
        }
    }
}
