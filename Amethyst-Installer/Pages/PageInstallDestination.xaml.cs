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

namespace amethyst_installer_gui.Pages
{
    /// <summary>
    /// Interaction logic for PageInstallDestination.xaml
    /// </summary>
    public partial class PageInstallDestination : UserControl, IInstallerPage
    {
        public PageInstallDestination()
        {
            InitializeComponent();
        }

        public InstallerState GetInstallerState()
        {
            return InstallerState.InstallDestination;
        }

        public string GetTitle()
        {
            return Properties.Resources.Page_Location_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e)
        {
            // Advance to next page
            MainWindow.Instance.SetPage(InstallerState.SystemRequirements);
        }

        public void OnSelected()
        {
            // Fetch drive info
            var drives = DriveInfo.GetDrives();
            for ( int i = 0; i < drives.Length; i++ ) {
                var letter = drives[i].RootDirectory.ToString().Replace(Path.DirectorySeparatorChar.ToString(), string.Empty).Replace(Path.AltDirectorySeparatorChar.ToString(), string.Empty);
                var driveName = Shell.GetDriveLabel(drives[i].RootDirectory.ToString());
                var freeSpace = drives[i].AvailableFreeSpace;
                var totalSize = drives[i].TotalSize;

                Logger.Info($"Drive [{i}] :: Letter {letter} :: Free {Util.SizeSuffix(freeSpace)} :: Total {Util.SizeSuffix(totalSize)} :: Name {driveName}");
            }
        }

        // Force only the first button to have focus
        public void OnFocus()
        {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) {}
        public void OnButtonTertiary(object sender, RoutedEventArgs e) {}
    }
}
