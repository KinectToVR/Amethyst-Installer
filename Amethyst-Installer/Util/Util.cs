using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using amethyst_installer_gui.Popups;

namespace amethyst_installer_gui
{
    public static class Util
    {
        /// <summary>
        /// Returns the version number of Amethyst Installer
        /// </summary>
        public static string InstallerVersionString
        {
            get
            {
                string verison = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return "Amethyst Installer v" + verison.Remove(verison.Length - 2);
            }
        }

        public static void ShowMessageBox(string title, string caption = "", MessageBoxButton button = MessageBoxButton.OK)
        {
            var modalWindow = new WinUI3MessageBox(caption, title, "Save", "Don't Save", "Cancel");

            // If the window type is a WinUI3MessageBox we'll get an exception
            if (Application.Current.MainWindow.GetType() == typeof(MainWindow))
                modalWindow.Owner = Application.Current.MainWindow;

            modalWindow.ShowDialog();

            // TODO: Return
        }

        /// <summary>
        /// A shorthand for clearing the keyboard focus style of a button if the user used their mouse to click it
        /// </summary>
        public static void HandleKeyboardFocus(RoutedEventArgs e)
        {
            if (((Control)e.Source).IsMouseOver && ((Control)e.Source).IsKeyboardFocused) Keyboard.ClearFocus();
        }
    }
}
