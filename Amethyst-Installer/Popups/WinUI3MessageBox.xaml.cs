using amethyst_installer_gui.PInvoke;
using System.Windows;
using System.Windows.Input;

namespace amethyst_installer_gui.Popups {
    /// <summary>
    /// Interaction logic for WinUI3MessageBox.xaml
    /// </summary>
    public partial class WinUI3MessageBox : Window {

        public ResultState Result = ResultState.None;

        public WinUI3MessageBox(string title, string caption, string buttonPrimaryText, string buttonSecondaryText, string buttonTertiaryText) {
            InitializeComponent();

            // Apply props
            titleContainer.Text = title;
            Title = title;

            message.Content = caption;

            dialogButton_Primary.Content = buttonPrimaryText;
            dialogButton_Secondary.Content = buttonSecondaryText;
            dialogButton_Tertiary.Content = buttonTertiaryText;

            dialogButton_Primary.Visibility = buttonPrimaryText.Length == 0 ? Visibility.Hidden : Visibility.Visible;
            dialogButton_Secondary.Visibility = buttonSecondaryText.Length == 0 ? Visibility.Hidden : Visibility.Visible;
            dialogButton_Tertiary.Visibility = buttonTertiaryText.Length == 0 ? Visibility.Hidden : Visibility.Visible;

            // Handle visibility

            // Fix corners on Win11
            DWM.SetWindowCorners(this, CornerPreference.Round);
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
