using amethyst_installer_gui.Commands;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using amethyst_installer_gui.PInvoke;
using System;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;

using AppWindow = amethyst_installer_gui.MainWindow;
using LocaleStrings = amethyst_installer_gui.Localisation;

namespace amethyst_installer_gui {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {

        public static InstallerState InitialPage = InstallerState.Welcome;

        private void Application_Startup(object sender, StartupEventArgs e) {

            Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            Kernel.AttachConsole(-1);

            // Initialize logger
            string logFileDate = DateTime.Now.ToString("yyyyMMdd-hhmmss.ffffff");
            Logger.Init(Path.GetFullPath(Path.Combine(Constants.AmethystLogsDirectory, $"Amethyst_Installer_{logFileDate}.log")));
            Logger.Info(Util.InstallerVersionString);

            CommandParser parser = new CommandParser();
            parser.ParseCommands(e.Args);

            // Init OpenVR
            OpenVRUtil.InitOpenVR();

            // Fetch installer API response from server
            InstallerStateManager.Initialize();

            // Check if we can even install Amethyst
            CheckCanInstall();

            // MainWindow
            MainWindow mainWindow = new MainWindow();
            mainWindow.ShowDialog();
        }

        private void CheckCanInstall() {
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

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.Fatal(Util.FormatException(e.Exception));
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
            AppWindow.Instance.privacyPolicyContainer.Visibility = Visibility.Hidden;
            SoundPlayer.PlaySound(SoundEffect.Error);
        }
    }
}
