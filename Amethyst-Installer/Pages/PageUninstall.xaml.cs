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
    /// Interaction logic for PageUninstall.xaml
    /// </summary>
    public partial class PageUninstall : UserControl, IInstallerPage {

        /*
         
        1) Check if Amethyst is fucking installed in case someone decided to be "special"
        2) Change sidebar to uninstall flow
        3) YEET!

        // MVP

        "Removing Ame"
        ☑ Remove calibration and setting (yeet appdata/ame)

        */

        public PageUninstall() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Uninstall;
        }

        public string GetTitle() {
            return Localisation.Manager.Page_Uninstall_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {}
        public void OnButtonSecondary(object sender, RoutedEventArgs e) {}
        public void OnButtonTertiary(object sender, RoutedEventArgs e) {}

        public void OnFocus() {

        }

        public void OnSelected() {

        }
    }
}
