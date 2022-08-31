using amethyst_installer_gui.Installer;
using Microsoft.NodejsTools.SharedProject;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

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

        public void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {

            if ( launchAmeOnExit.IsChecked.Value ) {

                SystemUtility.ExecuteProcessUnElevated(
                    Path.GetFullPath(Path.Combine(InstallerStateManager.AmethystInstallDirectory, "Amethyst.exe")),
                    "",
                    InstallerStateManager.AmethystInstallDirectory,
                    ShowWindow.SW_NORMAL);
            }

            Util.Quit(ExitCodes.OK);
        }

        public void OnSelected() {
            MainWindow.Instance.StopSpeedrunTimer();
            linksContainer.Inlines.Clear();

            // Get platform string
            string os_string = "10";
            if ( WindowsUtils.GetVersion().Build >= (int) WindowsUtils.WindowsMajorReleases.Win11_21H2 ) {
                os_string = "11";
            }

            // Default to start menu, else desktop
            string launchImagePath = $"/Resources/Image/start_{os_string}.png";

            if ( InstallerStateManager.CreateDesktopShortcut && !InstallerStateManager.CreateStartMenuEntry ) {
                launchFromPlace.Text = Localisation.Done_LaunchDesktop;
                launchImagePath = $"/Resources/Image/desktop_{os_string}.png";
            }
            launchImage.Source = new BitmapImage(new Uri(launchImagePath, UriKind.Relative));

            AddLink(Localisation.Done_LinkDocumentation, Util.GenerateDocsURL(string.Empty));
            linksContainer.Inlines.Add(Environment.NewLine);
            AddLink(Localisation.Done_LinkDiscord, Constants.DiscordInvite);
            linksContainer.Inlines.Add(Environment.NewLine);
            AddLink(Localisation.Done_LinkGitHub, "https://github.com/KinectToVR");
            linksContainer.Inlines.Add(Environment.NewLine);
            AddLink(Localisation.Done_LinkDonations, "https://opencollective.com/k2vr");
        }

        private void AddLink(string displayString, string urlTarget) {

            Hyperlink link = new Hyperlink()
            {
                NavigateUri = new Uri(urlTarget),
                Foreground = WindowsColorHelpers.AccentLight,
                BaselineAlignment = BaselineAlignment.Center,
            };
            link.Inlines.Add(displayString);
            link.RequestNavigate += OpenURL;
            linksContainer.Inlines.Add(link);
        }
        private void OpenURL(object sender, RequestNavigateEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(( sender as Hyperlink ).NavigateUri.ToString());
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        private void launchAmeOnExit_Checked(object sender, RoutedEventArgs e) {
            if ( ActualHeight == 0 || ActualWidth == 0 )
                return;
            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }
    }
}
