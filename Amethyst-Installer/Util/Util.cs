using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using amethyst_installer_gui.Popups;
using Microsoft.Win32;

namespace amethyst_installer_gui {
    public static class Util {

        public static Random Rng = new Random();

        /// <summary>
        /// Returns the version number of Amethyst Installer
        /// </summary>
        public static string InstallerVersionString {
            get {
                string version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                return "Amethyst Installer v" + version.Remove(version.Length - 2);
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
            else {
                Application.Current.MainWindow = null;
            }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte[] ExtractResourceAsBytes(string resourcePath) {
            using ( var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream($"amethyst_installer_gui.Resources.{resourcePath}") ) {
                byte[] buffer = new byte[resource.Length];
                using ( MemoryStream memStream = new MemoryStream() ) {
                    int read;
                    while ( ( read = resource.Read(buffer, 0, buffer.Length) ) > 0 ) {
                        memStream.Write(buffer, 0, read);
                    }
                    return memStream.ToArray();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsLaptop() {
            return PowerProvider.SystemPowerCapabilites.LidPresent;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        public static void Quit(ExitCodes exitCode, bool cleanTemp = true) {

#if !DEBUG && !DOWNLOAD_CACHE
            if ( cleanTemp ) {
                // Clear the temp directory
                // Directory.Delete(Constants.AmethystTempDirectory, true);

                // rmdir /Q /S nonemptydir

                // yes we use taskkill, I don't want to deal with all the bullshit of "bad PID" using the P/Invoke approach
                // besides taskkill is garuanteed to be on the current install anyway

                string tempDir = Constants.AmethystTempDirectory;

#if !DEBUG
                string tmpDirRoot = Path.GetFullPath($"{Constants.AmethystTempDirectory}/..");
                if (Directory.GetDirectories(tmpDirRoot).Length == 1 && Directory.GetFiles(tmpDirRoot).Length == 0) {
                    // Only Ame installer, therefore nuke the upper temp folder too
                    tempDir = tmpDirRoot;
                }
#endif

                var clearDirProc = Process.Start(new ProcessStartInfo() {
                    FileName = "cmd.exe",
                    Arguments = $"/C timeout 10 && rmdir /Q /S \"{tempDir}\"",
                    WindowStyle = ProcessWindowStyle.Hidden,
                    CreateNoWindow = true
                });
            }
#endif
            DeviceManaged.UnregisterDeviceNotifications();

            // Graceful close

            // Hack to hide the window, then actually close it
            // This is so that we make it seem like the app closes instantly, when in reality we have lots of threads in the background which I
            // can't be bothered to cleanup (Windows will clean them up anyway so I don't really care). Due to all these threads, the app takes
            // a few seconds until it fully closes
            if ( Application.Current.MainWindow != null ) {
                Application.Current.MainWindow.Visibility = Visibility.Hidden;
                Application.Current.MainWindow.ShowInTaskbar = false;
            }

            Task.Run(() => {

                Application.Current.Dispatcher.InvokeShutdown();
                // Wait a few 250ms first, otherwise the window will hang
                Thread.Sleep(250);

                Application.Current.Dispatcher.InvokeAsync(new Action(() => {
                    Application.Current.Shutdown(( int ) exitCode);

                    // @HACK: We should figure out *why* some other threads are keeping the process alive in some scenarios, and fix that behaviour.
                    Environment.Exit(( int ) exitCode); // Sometimes we would have a background thread resulting in a zombie process


                    // ForceKillProcess("Amethyst-Installer");
                }), DispatcherPriority.ContextIdle);
            });
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

        public static string GetChecksum(string filePath) {
            // 1MB read buffer, seems *200ms faster*
            using ( var stream = new BufferedStream(File.OpenRead(filePath), 1024 * 1024 * 1) ) {
                using ( var md5 = MD5.Create() ) {
                    byte[] checksum = md5.ComputeHash(stream);
                    return BitConverter.ToString(checksum).Replace("-", String.Empty);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetProcessorName() {
            var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0\");
            return key?.GetValue("ProcessorNameString").ToString() ?? "Unknown CPU";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ForceKillProcess(string processName) {
            var processes = Process.GetProcessesByName(processName);
            if ( processes.Length > 0 ) {
                // kill each process via taskkill.exe
                foreach ( var process in processes ) {

                    // yes we use taskkill, I don't want to deal with all the bullshit of "bad PID" using the P/Invoke approach
                    // besides taskkill is garuanteed to be on the current install anyway
                    var taskkillProc = Process.Start(new ProcessStartInfo() {
                        FileName = "taskkill.exe",
                        Arguments = $"/F /T /PID {process.Id}",
                        WindowStyle = ProcessWindowStyle.Hidden,
                        CreateNoWindow = true
                    });
                    taskkillProc.WaitForExit(10000);
                }
            }
        }

        /// <summary>
        /// Returns whether the current process is elevated or not
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCurrentProcessElevated() {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            WindowsPrincipal currentGroup = new WindowsPrincipal(currentIdentity);
            return currentGroup.IsInRole(WindowsBuiltInRole.Administrator);
        }

        /// <summary>
        /// Elevates the current process by re-launching it, passing all parameters to the new process, with a run as verb, just in case because
        /// people somehow got around this on k2vr-installer-gui
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ElevateSelf() {

            ProcessStartInfo procInfo = new ProcessStartInfo(){
                
                // Pass same args
                FileName            = Process.GetCurrentProcess().MainModule.FileName,
                Arguments           = string.Join("\" \"", Environment.GetCommandLineArgs()),
                WorkingDirectory    = Directory.GetCurrentDirectory(),

                Verb                = "runas" // Force UAC prompt
            };

            Process.Start(procInfo);
        }

        /// <summary>
        /// Gets a path relative to some directory
        /// </summary>
        /// <param name="path">The base path</param>
        /// <param name="relativePathStart">The length of the base path</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ReadOnlyMemory<char> RelativePath(this ReadOnlyMemory<char> path, int relativePathStart) {
            return path.Slice(relativePathStart, path.Length - relativePathStart);
        }

        public static bool ActivateFirewallRule(string title, NetworkProtocol protocol, ushort port) {
            string protocolString;
            switch ( protocol ) {
                case NetworkProtocol.TCP:
                    protocolString = "TCP";
                    break;
                case NetworkProtocol.UDP:
                    protocolString = "UDP";
                    break;
                default:
                    protocolString = "Unknown network protocol";
                    break;
            }

            // Network in
            var netProcIn = new ProcessStartInfo() {
                FileName = "netsh",
                Arguments = $"advfirewall firewall add rule name=\"{title} IN\" dir=in action=allow protocol={protocolString} localport={port}",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            var winProcNetIn = Process.Start(netProcIn);
            winProcNetIn.WaitForExit(10000);
            

            // Network in
            var netProcOut = new ProcessStartInfo() {
                FileName = "netsh",
                Arguments = $"advfirewall firewall add rule name=\"{title} OUT\" dir=out action=allow protocol={protocolString} localport={port}",
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden,
            };
            var winProcNetOut = Process.Start(netProcOut);
            winProcNetOut.WaitForExit(10000);

            // Process.Start($"netsh advfirewall firewall add rule name=\"{title} IN\" dir=in action=allow protocol={protocolString} localport={port}").WaitForExit(10000);
            // Process.Start($"netsh advfirewall firewall add rule name=\"{title} OUT\" dir=out action=allow protocol={protocolString} localport={port}").WaitForExit(10000);
            return winProcNetIn.ExitCode == 0 && winProcNetOut.ExitCode == 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        // https://stackoverflow.com/a/9294382
        public static bool IsDiskFull(Exception ex) {
            const int HR_ERROR_HANDLE_DISK_FULL = unchecked((int)0x80070027);
            const int HR_ERROR_DISK_FULL = unchecked((int)0x80070070);

            return ex.HResult == HR_ERROR_HANDLE_DISK_FULL
                || ex.HResult == HR_ERROR_DISK_FULL;
        }
    }

    public enum NetworkProtocol {
        UDP,
        TCP
    }
}