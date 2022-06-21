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
    /// Interaction logic for PageSystemRequirements.xaml
    /// </summary>
    public partial class PageSystemRequirements : UserControl, IInstallerPage {
        public PageSystemRequirements() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.SystemRequirements;
        }

        public string GetTitle() {
            return Properties.Resources.Page_Sysreq_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            MainWindow.Instance.SetPage(InstallerState.Downloading);
        }

        public void OnSelected() {
            // Compute install requirements

            // TODO: Check storage
            diskSpaceDescription.Text = Properties.Resources.SystemRequirement_Description_Storage; // TODO: String format

            // TODO: Check USB controllers
            usbControllersDescription.Text = Properties.Resources.SystemRequirement_Description_UsbControllers; // TODO: String format

            // TODO: Check VR headset
            vrSystemDescription.Text = $"{OpenVRUtil.HmdType} ({OpenVRUtil.ConnectionType}) [{OpenVRUtil.TrackingType}]"; // TODO: ToFriendlyString method

            vrSystemFootnote.Inlines.Clear();
            vrSystemFootnote.Inlines.Add("Some text ");
            Hyperlink hyperLink3 = new Hyperlink()
            {
                NavigateUri = new Uri("http://somesite.com"),
                // Foreground = WindowsColorHelpers.Accent
            };
            hyperLink3.Inlines.Add("some site");
            hyperLink3.RequestNavigate += Hyperlink_RequestNavigate;
            vrSystemFootnote.Inlines.Add(hyperLink3);
            vrSystemFootnote.Inlines.Add(" Some more text");

            // TODO: Check target device
            compatDevicesDescription.Text = "among us";
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

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e) {
            // TODO: link click handler
        }
    }
}
