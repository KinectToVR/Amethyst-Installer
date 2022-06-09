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
    /// Interaction logic for PageEULA.xaml
    /// </summary>
    public partial class PageEULA : UserControl, IInstallerPage
    {
        public PageEULA()
        {
            InitializeComponent();
        }

        public InstallerState GetInstallerState()
        {
            return InstallerState.EULA;
        }

        public string GetTitle()
        {
            // TODO: EULA title
            return "EULA TODO: replace based on params or something";
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e)
        {

        }

        private void eulaRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = eulaAgree != null && eulaAgree.IsChecked.HasValue && eulaAgree.IsChecked.Value == true;
        }

        // Force only the first button to have focus
        public void OnFocus()
        {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = false;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnSelected() {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) {}
        public void OnButtonTertiary(object sender, RoutedEventArgs e) {}
    }
}
