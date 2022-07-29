using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Interaction logic for PageWelcome.xaml
    /// </summary>
    public partial class PageWelcome : UserControl, IInstallerPage {
        public PageWelcome() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Welcome;
        }

        public string GetTitle() {
            return "";
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            // Advance to next page
            MainWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Hidden;
            MainWindow.Instance.SetPage(InstallerState.InstallOptions);
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Properties.Resources.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        public void OnSelected() {

            // Localize the privacy policy thing
            var readPrivacyPolicyRaw = Properties.Resources.Welcome_ReadPrivacyPolicy;
            string firstPart = readPrivacyPolicyRaw.Substring(0, readPrivacyPolicyRaw.IndexOf("%s%"));
            string secondPart = readPrivacyPolicyRaw.Substring(readPrivacyPolicyRaw.IndexOf("%s%") + 3);

            MainWindow.Instance.readPrivacyPolicy.Inlines.Clear();
            MainWindow.Instance.readPrivacyPolicy.Inlines.Add(firstPart);
            Hyperlink privacyPolicyLink = new Hyperlink()
            {
                NavigateUri = new Uri($"http://k2vr.tech/{MainWindow.LocaleCode}/privacy"),
            };
            privacyPolicyLink.Inlines.Add(Properties.Resources.Welcome_PrivacyPolicy);
            privacyPolicyLink.RequestNavigate += OpenK2VRPrivacyPolicyURL;
            MainWindow.Instance.readPrivacyPolicy.Inlines.Add(privacyPolicyLink);
            if ( secondPart.Length > 0 )
                MainWindow.Instance.readPrivacyPolicy.Inlines.Add(secondPart);

            // TODO: Splash screen
        }

        private void OpenK2VRPrivacyPolicyURL(object sender, RequestNavigateEventArgs e) {
            Process.Start(( sender as Hyperlink ).NavigateUri.ToString());
        }
    }
}
