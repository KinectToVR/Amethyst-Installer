using amethyst_installer_gui.Installer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace amethyst_installer_gui.Pages {
    /// <summary>
    /// Interaction logic for PageDoom.xaml
    /// </summary>
    public partial class PageDoom : UserControl, IInstallerPage  {
        public PageDoom() {
            InitializeComponent();
        }

        private void InitDoom() {

            // Init DOOM
            doomHost.doomGame = new ManagedDoom.SFML.SfmlDoom(new ManagedDoom.CommandLineArgs(new string[] { }), ( int ) doomHost.ActualWidth, ( int ) doomHost.ActualHeight, doomHost);
            doomHost.Focus();
        }

        private void ActionButtonPrimary_Click(object sender, RoutedEventArgs e) {
            MainWindow.Instance.GoToLastPage();
        }

        private void UpdateHandler() {

            Dispatcher.BeginInvoke(new Action(() => InitDoom()), DispatcherPriority.ContextIdle, null);
        }

        private void AnimationHandler() {
            for ( int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++ ) {
                DependencyObject ithChild = VisualTreeHelper.GetChild(this, i);
                if ( ithChild == null )
                    continue;
                if ( ithChild is UIElement )
                    ( ( UIElement ) ithChild ).Focusable = false;
            }

            doomHost.Focusable = true;
        }

        private void learnMoreLink_Click(object sender, RoutedEventArgs e) {
            SoundPlayer.PlaySound(SoundEffect.Invoke);
            Process.Start(Util.GenerateDocsURL("k2ex-upgrade"));
        }

        public InstallerState GetInstallerState() {
            return InstallerState.DooM;
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
