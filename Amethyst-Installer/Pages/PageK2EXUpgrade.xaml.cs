using amethyst_installer_gui.Controls;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageK2EXUpgrade.xaml
    /// </summary>
    public partial class PageK2EXUpgrade : UserControl, IInstallerPage {
        public PageK2EXUpgrade() {
            InitializeComponent();
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            Util.HandleKeyboardFocus(e);

#if DEBUG
            // Capture frame with RenderDoc
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            dxHost.doCapture = true;
#else
            if ( MainWindow.HandleSpeedrun() ) {
                // Advance to next page
                SoundPlayer.PlaySound(SoundEffect.MoveNext);
                MainWindow.Instance.SetPage(InstallerState.Welcome);
            }
#endif
        }

        private void UpdateHandler() {

        }

        private void AnimationHandler() {
            Storyboard sb = (Resources["IntroAnimationSequence"] as Storyboard);
            sb.Begin();
        }

        private void learnMoreLink_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(Util.GenerateDocsURL("k2ex-upgrade"));
        }

        public InstallerState GetInstallerState() {
            return InstallerState.K2EXUpgrading;
        }

        public string GetTitle() {
            return string.Empty;
        }

        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = false;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;

            AnimationHandler();

#if DEBUG
            ActionButtonPrimary.Content = "RenderDoc Capture";
#endif
        }

        public void OnSelected() {
            MainWindow.Instance.SetSidebarHidden(true);
            MainWindow.Instance.SetButtonsHidden(true);
            UpdateHandler();
        }
        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
