using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
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
                    btnTertiary = Localisation.Modal_OK;
                    break;
                case MessageBoxButton.OKCancel:
                    btnPrimary = string.Empty;
                    btnSecondary = Localisation.Modal_OK;
                    btnTertiary = Localisation.Modal_Cancel;
                    break;
                case MessageBoxButton.YesNo:
                    btnPrimary = string.Empty;
                    btnSecondary = Localisation.Modal_Yes;
                    btnTertiary = Localisation.Modal_No;
                    break;
                case MessageBoxButton.YesNoCancel:
                    btnPrimary = Localisation.Modal_Yes;
                    btnSecondary = Localisation.Modal_No;
                    btnTertiary = Localisation.Modal_Cancel;
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

        public static bool UseShitpostSizes = false;

        private static readonly string[] SizeSuffixes =
            { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

        private static readonly string[] ShitpostSizeSuffixes =
            { "bites", "K-Marts", "MegaFarts", "GigaShits", "TeraShits", "PeanutButters", "ExtraBoats", "ZigoBoats", "YellowBees" };

        /// <summary>
        /// Converts bytes to the largest format which makes sense
        /// </summary>
        public static string SizeSuffix(long value, int decimalPlaces = 3) {
            return SizeSuffix(value, UseShitpostSizes ? ShitpostSizeSuffixes : SizeSuffixes, decimalPlaces);
        }
        public static string SizeSuffix(long value, string[] sizeSuffixes, int decimalPlaces = 3) {
            if ( decimalPlaces < 0 ) { throw new ArgumentOutOfRangeException("decimalPlaces"); }
            if ( value < 0 || value == 0 ) { return $"0 {sizeSuffixes[0]}"; }

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
                sizeSuffixes[mag]);
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

            // While this amount of nested if statements might seem excessive, it's faster than
            // alternative methods, hence why I've implemented it (in the rare case of someone
            // having like 26 drives for instance)

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ExtractResourceToFile(string resourcePath, string filePath) {
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.{resourcePath}") ) {
                using ( var file = new FileStream(filePath, FileMode.Create, FileAccess.Write) ) {
                    resource.CopyTo(file);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ExtractResourceAsString(string resourcePath) {
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.{resourcePath}") ) {
                using ( StreamReader reader = new StreamReader(resource) ) {
                    return reader.ReadToEnd();
                }
            }
        }

        public static bool IsLaptop() {
            return PowerProvider.SystemPowerCapabilites.LidPresent;
        }

        public static string GenerateDocsURL(string relative) {

            string docsLocaleCode = "en";
            for ( int i = 0; i < InstallerStateManager.AmeDocsLocaleList.Length; i++ ) {
                if ( InstallerStateManager.AmeDocsLocaleList[i] == LocaleManager.CurrentLocale ) {
                    docsLocaleCode = InstallerStateManager.AmeDocsLocaleList[i];
                    break;
                }
            }

            return Constants.DocsDomain + $"/{docsLocaleCode}/" + relative;
        }

        /// <summary>
        /// Terminates the installer using an exit code. Use this method instead of the standard WPF method due to extra
        /// code that has to be run on all shutdown events.
        /// </summary>
        /// <param name="exitCode">The exitcode for this shutdown</param>
        public static void Quit(ExitCodes exitCode) {

            // TODO: Launch Ame on shutdown if necessary
            // This is required in the event of an upgrade for example

            Application.Current.Shutdown(( int ) exitCode);

            
        }

        // https://floating-point-gui.de/errors/comparison/
        public static bool ApproximatelyEqualTo(this double a, double b, double epsilon = Constants.Epsilon) {
            double absA = Math.Abs(a);
            double absB = Math.Abs(b);
            double diff = Math.Abs(a - b);

            if ( a == b ) {
                // handle infinities
                return true;
            } else if ( a == 0 || b == 0 || ( absA + absB < Constants.DoubleMinNormal ) ) {
                // a or b are extremely close to 0, hence relative error is less meaningful
                return diff < epsilon * Constants.DoubleMinNormal;
            } else {
                // use relative error
                return diff / Math.Min(absA + absB, float.MaxValue) < epsilon;
            }
        }


        public static void CreateExecutableShortcut(string path, string directory, string file, string description) {
            var shell = new IWshRuntimeLibrary.WshShell();
            var shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(path);
            shortcut.Description = description;
            shortcut.TargetPath = file;
            shortcut.IconLocation = $"{file},0";
            shortcut.WorkingDirectory = directory;
            shortcut.Save();
        }
    }
}