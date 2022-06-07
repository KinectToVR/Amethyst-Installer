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

namespace amethyst_installer_gui.Pages
{
    /// <summary>
    /// Interaction logic for PageDownloading.xaml
    /// </summary>
    public partial class PageDownloading : UserControl, IInstallerPage
    {
        public PageDownloading()
        {
            InitializeComponent();
        }

        public InstallerState GetInstallerState()
        {
            return InstallerState.Downloading;
        }

        public string GetTitle()
        {
            return Properties.Resources.Page_Download_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e)
        {
            // Advance to next page
            MainWindow.Instance.SetPage(InstallerState.Installation);
        }

        public void OnSelected()
        {

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
