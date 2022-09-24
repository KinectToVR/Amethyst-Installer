using amethyst_installer_gui.Commands;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using amethyst_installer_gui.PInvoke;
using amethyst_installer_gui.Protocol;
using System;
using System.IO;
using System.Media;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Windows;

using AppWindow = amethyst_installer_gui.MainWindow;
using LocaleStrings = amethyst_installer_gui.Localisation;

namespace amethyst_installer_gui {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static InstallerState InitialPage = InstallerState.Welcome;
        private static bool s_initialized = false;

        private void Application_Startup(object sender, StartupEventArgs e) {

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            
            // Init console ; enables ANSI, unicode, and enables logging in WPF
            Kernel.AttachConsole(-1);
            Console.OutputEncoding = Encoding.Unicode;
            Kernel.EnableAnsiCmd();

            // Ensure that we are running as admin, in the event that the user somehow started the app without admin
            if ( !Util.IsCurrentProcessElevated() ) {
                Console.WriteLine("Currently executing as a standard user...");
                Console.WriteLine("Restarting installer as admin!");
                Util.ElevateSelf();
                Util.Quit(ExitCodes.RequiredAdmin);
                return;
            }

            DeviceManaged.RegisterDeviceNotificationHandler();

            CommandParser parser = new CommandParser();
            if ( !ProtocolParser.ParseCommands(e.Args) && !parser.ParseCommands(e.Args) ) {

                // @NOTE: Yes, we technically do support light theme, but right now it's an unfinished mess, so we'll keep on forcing
                // dark mode for the time being
                // SetTheme(WindowsColorHelpers.IsDarkTheme());
                SetTheme(true);
                Init();

                // MainWindow
                MainWindow mainWindow = new MainWindow();
                mainWindow.ShowDialog();
            } else {
                Util.Quit(ExitCodes.Command);
            }
        }

        private static void CheckCanInstall() {
            if ( !InstallerStateManager.CanInstall ) {
                SystemSounds.Exclamation.Play();
                if ( InstallerStateManager.IsCloudPC ) {
                    // If Cloud PC
                    Util.ShowMessageBox(LocaleStrings.InstallProhibited_CloudPC, LocaleStrings.InstallProhibited_Title, MessageBoxButton.OK);
                    Util.Quit(ExitCodes.IncompatibleSetup);
                } else if ( !InstallerStateManager.SteamVRInstalled ) {
                    // If no SteamVR
                    Util.ShowMessageBox(LocaleStrings.InstallProhibited_NoSteamVR, LocaleStrings.InstallProhibited_Title, MessageBoxButton.OK);
                    Util.Quit(ExitCodes.IncompatibleSetup);
                } else if ( InstallerStateManager.IsWindowsAncient ) {
                    // If Windows version is not supported
                    Util.ShowMessageBox(LocaleStrings.InstallProhibited_WindowsAncient, LocaleStrings.InstallProhibited_Title, MessageBoxButton.OK);
                    Util.Quit(ExitCodes.IncompatibleSetup);
                }
            }
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.PrivateDoNotUseLogExecption(Util.FormatException(e.Exception), $"Unhandled Exception: {e.Exception.GetType().Name} in {e.Exception.Source}: {e.Exception.Message}");
            e.Handled = true;

            if ( AppWindow.Instance == null ) {
                // uhhhhhhhhhh how the fuck did you get here
                Util.ShowMessageBox(LocaleStrings.Dialog_Description_CritError.Replace("[server]", Constants.DiscordInvite), LocaleStrings.Dialog_Title_CritError);
                Util.Quit(ExitCodes.ExceptionPreInit);
                return;
            }
            // AFAIK the app should always be initialized, so this scenario should be impossible
            ( AppWindow.Instance.Pages[InstallerState.Exception] as PageException ).currentException = e.Exception;
            AppWindow.Instance.OverridePage(InstallerState.Exception);
            SoundPlayer.PlaySound(SoundEffect.Error);
        }

        public static void Init() {

            if ( s_initialized )
                return;

            // Initialize logger
            string logFileDate = DateTime.Now.ToString("yyyyMMdd-hhmmss.ffffff");
            Logger.Init(Path.GetFullPath(Path.Combine(Constants.AmethystLogsDirectory, $"Amethyst_Installer_{logFileDate}.log")));
            Logger.Info(Util.InstallerVersionString);

            // Init OpenVR
            OpenVRUtil.InitOpenVR();

            // Init K2Archive native
            K2Archive.Init();

            // Fetch installer API response from server
            InstallerStateManager.Initialize();

            // Check if we can even install Amethyst
            CheckCanInstall();

            // Check if we're running on a multi-account setup.
            // If the user is a standard user but we're running the installer as admin, we have to break vrpathreg, as we can't de-elevate it.
            // Well... we can de-elevate it (see SystemUtilities.cs), but we won't be able to get any return codes.
            if ( InstallerStateManager.MustDelevateProcesses ) {

                // Equivalent of:
                // OpenVRUtil.s_failedToInit = false;

                BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
                FieldInfo handleField = typeof(OpenVRUtil).GetField("s_failedToInit", bindFlags);
                handleField.SetValue(null, true);
            }

            s_initialized = true;
        }


        public ResourceDictionary ThemeDictionary {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }

        private void SetTheme(bool darkMode) {
            ThemeDictionary.MergedDictionaries.Clear();
            if (!darkMode) {
                ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/ColoursLight.xaml", UriKind.Relative) });
            } else {
                ThemeDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("Styles/ColoursDark.xaml", UriKind.Relative) });
            }
        }
    }
}
