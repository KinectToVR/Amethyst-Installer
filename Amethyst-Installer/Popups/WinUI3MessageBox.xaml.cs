using amethyst_installer_gui.PInvoke;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace amethyst_installer_gui.Popups {
    /// <summary>
    /// Interaction logic for WinUI3MessageBox.xaml
    /// </summary>
    public partial class WinUI3MessageBox : Window {

        public ResultState Result = ResultState.None;

        public WinUI3MessageBox(string title, string caption, string buttonPrimaryText, string buttonSecondaryText, string buttonTertiaryText) {
            InitializeComponent();
            ContentRendered += Window_ContentRendered;

            // Apply props
            titleContainer.Text = title;
            Title               = title;

            message.Inlines.Clear();
            message.Text = caption;

            dialogButton_Primary.Content    = buttonPrimaryText;
            dialogButton_Secondary.Content  = buttonSecondaryText;
            dialogButton_Tertiary.Content   = buttonTertiaryText;

            // Handle visibility
            dialogButton_Primary.Visibility     = buttonPrimaryText.Length      == 0 ? Visibility.Hidden : Visibility.Visible;
            dialogButton_Secondary.Visibility   = buttonSecondaryText.Length    == 0 ? Visibility.Hidden : Visibility.Visible;
            dialogButton_Tertiary.Visibility    = buttonTertiaryText.Length     == 0 ? Visibility.Hidden : Visibility.Visible;
        }

        // Setup theme related stuff using DWM
        private void Window_ContentRendered(object sender, EventArgs e) {

            // Fix corners on Win11
            DWM.SetWindowCorners(this, CornerPreference.Round);

            // @TODO: Theming!!
            // Dark / Light mode
            DWM.SetDarkMode(this, true);

            // Mica some bitches
            if ( DWM.EnableBackdropBlur(this) ) {

                DWM.ExtendWindowChrome(this);
            } else {
                var color = ( ( SolidColorBrush ) Background ).Color;
                color.A = 255;
                Background = new SolidColorBrush(color);
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e) {
            Keyboard.ClearFocus();
        }

        private void TitlebarDragArea_MouseDown(object sender, MouseButtonEventArgs e) {
            if ( e.ChangedButton == MouseButton.Left )
                this.DragMove();
        }

        private void TitlebarDragArea_MouseRightButtonUp(object sender, MouseButtonEventArgs e) {
            SystemCommands.ShowSystemMenu(this, PointToScreen(Mouse.GetPosition(this)));
        }

        private void Close_Click(object sender, RoutedEventArgs e) {
            Util.HandleKeyboardFocus(e);
            Close();
        }

        private void dialogButton_Primary_Click(object sender, RoutedEventArgs e) {
            Result = ResultState.Primary;
            Close();
        }

        private void dialogButton_Secondary_Click(object sender, RoutedEventArgs e) {
            Result = ResultState.Secondary;
            Close();
        }

        private void dialogButton_Tertiary_Click(object sender, RoutedEventArgs e) {
            Result = ResultState.Tertiary;
            Close();
        }
    }

    public enum ResultState {
        None = 0,
        Primary = 1,
        Secondary = 2,
        Tertiary = 3,
    }
}
