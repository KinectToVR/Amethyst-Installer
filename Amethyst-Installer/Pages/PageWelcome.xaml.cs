using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            return string.Empty;
        }

        // Force only the first button to have focus
        public void OnFocus() {
            MainWindow.Instance.ActionButtonPrimary.Visibility = Visibility.Visible;
            MainWindow.Instance.ActionButtonPrimary.Content = Localisation.Manager.Installer_Action_Next;
            MainWindow.Instance.ActionButtonSecondary.Visibility = Visibility.Hidden;
            MainWindow.Instance.ActionButtonTertiary.Visibility = Visibility.Hidden;

            MainWindow.Instance.SetSidebarHidden(false);
            MainWindow.Instance.SetButtonsHidden(true);
        }

        public void OnButtonPrimary(object sender, RoutedEventArgs e) { }
        public void OnButtonSecondary(object sender, RoutedEventArgs e) { }
        public void OnButtonTertiary(object sender, RoutedEventArgs e) { }

        public void OnSelected() {
            // Localize the installer footnote
            string techPreviewRaw = Localisation.Manager.Welcome_Footnote;
            string t_firstPart = techPreviewRaw.Substring(0, techPreviewRaw.IndexOf("%s%"));
            string t_secondPart = techPreviewRaw.Substring(techPreviewRaw.IndexOf("%s%") + 3);

            previewWarning.Inlines.Clear();
            previewWarning.Inlines.Add("Amethyst was moved to the Microsoft Store! ");
            previewWarning.Inlines.Add("The current one will need to be uninstalled. ");

            Hyperlink discordLink = new Hyperlink {
                NavigateUri = new Uri(Constants.DiscordInvite),
                Foreground = WindowsColorHelpers.AccentLight,
                BaselineAlignment = BaselineAlignment.Center
            };
            discordLink.Inlines.Add("You can find more info on that, here.");
            discordLink.RequestNavigate += OpenUriUrl;
            previewWarning.Inlines.Add(discordLink);

            // Splash screen
            GenerateSplashText();
        }

        private void OpenUriUrl(object sender, RequestNavigateEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(( sender as Hyperlink ).NavigateUri.ToString());
        }

        private void splash_MouseUp(object sender, MouseButtonEventArgs e) {
            if ( e.ChangedButton != MouseButton.Left ) {
                return;
            }

            SoundPlayer.PlaySound(SoundEffect.Focus);
            GenerateSplashText();
        }

        private void splash_MouseWheel(object sender, MouseWheelEventArgs e) {
#if DEBUG
            // UP
            if ( e.Delta > 0 ) {
                splashId += 2;
            }

            // DOWN
            if ( e.Delta < 0 ) {
                splashId -= 2;
            }

            GenerateSplashText();
#endif
        }

#if DEBUG
        private int splashId = -1;
#endif

        private void GenerateSplashText() {
#if DEBUG
            splashId++;
            splashId %= InstallerStateManager.API_Response.Splashes.Count;
            if ( splashId < 0 ) {
                splashId = ( splashId + InstallerStateManager.API_Response.Splashes.Count ) %
                           InstallerStateManager.API_Response.Splashes.Count;
            }
#else
            Random rng = new Random();
            int splashId = rng.Next(0, InstallerStateManager.API_Response.Splashes.Count);
#endif

            string splashString = InstallerStateManager.API_Response.Splashes[splashId];
            if ( splashString[0] == '"' ) {
                splashText.Text = splashString;
            } else {
                splashText.Text = $"\"{splashString}\"";
            }
        }

        private void proceedButton_Click(object sender, RoutedEventArgs e) {
            Process.Start(
                new ProcessStartInfo("ms-windows-store://pdp/?productid=9P7R8FGDDGDH") { UseShellExecute = true });

            ProcessStartInfo procInfo = new ProcessStartInfo() {
                // Pass same args
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                Arguments = " --uninstall",
                WorkingDirectory = Directory.GetCurrentDirectory()
            };

            Process.Start(procInfo);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e) {
            if ( ActualWidth == 0 || ActualHeight == 0 ) {
                return;
            }

            SoundPlayer.PlaySound(SoundEffect.Invoke);
        }
    }
}