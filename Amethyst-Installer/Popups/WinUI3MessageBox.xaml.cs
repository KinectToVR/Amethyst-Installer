using amethyst_installer_gui.PInvoke;
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
using System.Windows.Shapes;

namespace amethyst_installer_gui.Popups {
    /// <summary>
    /// Interaction logic for WinUI3MessageBox.xaml
    /// </summary>
    public partial class WinUI3MessageBox : Window {

        public WinUI3MessageBox(string title, string caption, string buttonPrimaryText, string buttonSecondaryText, string buttonTertiaryText) {
            InitializeComponent();

            // Apply props
            titleContainer.Text = title;
            Title = title;

            message.Content = caption;

            dialogButton_Primary.Content = buttonPrimaryText;
            dialogButton_Secondary.Content = buttonSecondaryText;
            dialogButton_Tertiary.Content = buttonTertiaryText;

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

        }

        private void dialogButton_Secondary_Click(object sender, RoutedEventArgs e) {

        }

        private void dialogButton_Tertiary_Click(object sender, RoutedEventArgs e) {

        }
    }
}
