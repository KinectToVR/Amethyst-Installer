using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using amethyst_installer_gui.Popups;

namespace amethyst_installer_gui {
    public static class Util {

        public static Random Rng = new Random();

        /// <summary>
        /// Returns the version number of Amethyst Installer
        /// </summary>
        public static string InstallerVersionString {
            get {
                string verison = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return "Amethyst Installer v" + verison.Remove(verison.Length - 2);
            }
        }

        public static MessageBoxResult ShowMessageBox(string title, string caption = "", MessageBoxButton button = MessageBoxButton.OK) {

            string btnPrimary = string.Empty;
            string btnSecondary = string.Empty;
            string btnTertiary = string.Empty;

            switch ( button ) {
                case MessageBoxButton.OK:
                    btnPrimary = string.Empty;
                    btnSecondary = string.Empty;
                    btnTertiary = Properties.Resources.Modal_OK;
                    break;
                case MessageBoxButton.OKCancel:
                    btnPrimary = string.Empty;
                    btnSecondary = Properties.Resources.Modal_OK;
                    btnTertiary = Properties.Resources.Modal_Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    btnPrimary = string.Empty;
                    btnSecondary = Properties.Resources.Modal_Yes;
                    btnTertiary = Properties.Resources.Modal_No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnPrimary = Properties.Resources.Modal_Yes;
                    btnSecondary = Properties.Resources.Modal_No;
                    btnTertiary = Properties.Resources.Modal_Cancel;
                    break;
            }

            var modalWindow = new WinUI3MessageBox(caption, title, btnPrimary, btnSecondary, btnTertiary);

            // If the window type is a WinUI3MessageBox we'll get an exception
            if ( Application.Current.MainWindow.GetType() == typeof(MainWindow) )
                modalWindow.Owner = Application.Current.MainWindow;

            modalWindow.ShowDialog();


            switch ( button ) {
                case MessageBoxButton.OK:
                    switch ( modalWindow.Result ) {
                        case ResultState.Tertiary:
                            return MessageBoxResult.OK;
                    }
                    break;
                case MessageBoxButton.OKCancel:
                    switch ( modalWindow.Result ) {
                        case ResultState.Secondary:
                            return MessageBoxResult.OK;
                        case ResultState.Tertiary:
                            return MessageBoxResult.Cancel;
                    }
                    break;
                case MessageBoxButton.YesNo:
                    switch ( modalWindow.Result ) {
                        case ResultState.Secondary:
                            return MessageBoxResult.Yes;
                        case ResultState.Tertiary:
                            return MessageBoxResult.No;
                    }
                    break;
                case MessageBoxButton.YesNoCancel:
                    switch ( modalWindow.Result ) {
                        case ResultState.Primary:
                            return MessageBoxResult.Yes;
                        case ResultState.Secondary:
                            return MessageBoxResult.No;
                        case ResultState.Tertiary:
                            return MessageBoxResult.Cancel;
                    }
                    break;
            }
            return MessageBoxResult.None;
        }

        /// <summary>
        /// A shorthand for clearing the keyboard focus style of a button if the user used their mouse to click it
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void HandleKeyboardFocus(RoutedEventArgs e) {
            if ( ( ( Control ) e.Source ).IsMouseOver && ( ( Control ) e.Source ).IsKeyboardFocused )
                Keyboard.ClearFocus();
        }

        private static readonly string[] SizeSuffixes =
                   { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        /// <summary>
        /// Converts bytes to the largest format which makes sense
        /// </summary>
        public static string SizeSuffix(long value, int decimalPlaces = 3) {
            if ( decimalPlaces < 0 ) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if ( value < 0 ) { return "-" + SizeSuffix(-value, decimalPlaces); }
            if ( value == 0 ) { return string.Format("{0:n" + decimalPlaces + "} bytes", 0); }

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << (mag * 10));

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if ( Math.Round(adjustedSize, decimalPlaces) >= 1000 ) {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0} {1}",
                Truncate(adjustedSize, decimalPlaces),
                SizeSuffixes[mag]);
        }

        /// <summary>
        /// Rounds a number to N digits and returns it as a string
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Truncate(decimal value, int N) {
            int integerPart = (int)value;
            int size = integerPart.Digits();
            if ( size < N )
                return value.ToString($"n{N - size}");
            return value.ToString($"n{0}");
        }

        /// <summary>
        /// Returns how many digits there are in a given 32-bit integer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Digits(this int n) {
            if ( n >= 0 ) {
                if ( n < 10 )
                    return 1;
                if ( n < 100 )
                    return 2;
                if ( n < 1000 )
                    return 3;
                if ( n < 10000 )
                    return 4;
                if ( n < 100000 )
                    return 5;
                if ( n < 1000000 )
                    return 6;
                if ( n < 10000000 )
                    return 7;
                if ( n < 100000000 )
                    return 8;
                if ( n < 1000000000 )
                    return 9;
                return 10;
            } else {
                if ( n > -10 )
                    return 2;
                if ( n > -100 )
                    return 3;
                if ( n > -1000 )
                    return 4;
                if ( n > -10000 )
                    return 5;
                if ( n > -100000 )
                    return 6;
                if ( n > -1000000 )
                    return 7;
                if ( n > -10000000 )
                    return 8;
                if ( n > -100000000 )
                    return 9;
                if ( n > -1000000000 )
                    return 10;
                return 11;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string FormatException(Exception ex) {
            return $"Unhandled Exception: {ex.GetType().Name} in {ex.Source}: {ex.Message}\n{ex.StackTrace}";
        }
    }
}