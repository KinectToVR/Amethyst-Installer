using amethyst_installer_gui.Installer;
using System;
using System.Windows;
using System.Windows.Controls;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDone.xaml
    /// </summary>
    public partial class PageDone : UserControl, IInstallerPage {
        public PageDone() {
            InitializeComponent();
        }

        public InstallerState GetInstallerState() {
            return InstallerState.Done;
        }

        public string GetTitle() {
            return Localisation.Page_Done_Title;
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Application.Current.Shutdown(( int ) ExitCodes.OK);
        }

        public void OnSelected() {
            MainWindow.Instance.StopSpeedrunTimer();
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(false);
        }

        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }
    }
}
