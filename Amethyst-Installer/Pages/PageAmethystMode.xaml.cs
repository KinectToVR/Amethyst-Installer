using amethyst_installer_gui.Controls;
using amethyst_installer_gui.Installer;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageAmethystMode.xaml
    /// </summary>
    public partial class PageAmethystMode : UserControl, IInstallerPage {
        public PageAmethystMode() {
            InitializeComponent();

            radioOpenvr.OnToggled += OnToggledItem;
            radioOsc.OnToggled += OnToggledItem;
        }

        private void OnToggledItem(object sender, RoutedEventArgs e) {
            if (((RadioOptionDescriptive)sender).IsChecked) {
                SoundPlayer.PlaySound(SoundEffect.Invoke);
            }
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            Util.HandleKeyboardFocus(e);

            if ( MainWindow.HandleSpeedrun() ) {
                // Advance to next page
                SoundPlayer.PlaySound(SoundEffect.MoveNext);

                InstallerStateManager.DefaultToOSC = radioOsc.IsChecked;

                MainWindow.Instance.SetPage(InstallerState.InstallOptions);
            }
        }

        public InstallerState GetInstallerState() {
            return InstallerState.AmethystModeSelection;
        }

        public string GetTitle() {
            return Localisation.Page_SelectAmethystMode_Title;
        }

        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.IsEnabled = false;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Visible;

        }

        public void OnSelected() {
            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);

            // If SteamVR is installed and managed to locate vrpaths and the install, default to SteamVR
            // otherwise default to OSC
            if ( OpenVRUtil.IsSteamVrInstalled() ) {
                radioOpenvr.IsChecked   = true;
                radioOsc.IsChecked      = false;
            } else {
                radioOpenvr.IsChecked   = false;
                radioOsc.IsChecked      = true;
            }
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
