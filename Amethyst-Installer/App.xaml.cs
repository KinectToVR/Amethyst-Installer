using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using System;
using System.IO;
using System.Windows;

using AppWindow = amethyst_installer_gui.MainWindow;

namespace amethyst_installer_gui {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application {
        private void Application_Startup(object sender, StartupEventArgs e) {

            /*
             
            COMMAND LINE ARGUMENTS:

            --help, -h :: Shows help
            --update -u :: Attempts to update Amethyst
            --uninstall -x :: Attempts to uninstall Amethyst
            --silent -s :: Executes the installer silently
            --install-dir :: Sets the install directory, 
             
             */

            // Initialize logger
            string logFileDate = DateTime.Now.ToString("yyyyMMdd-hhmmss.ffffff");
            Logger.Init(Path.GetFullPath(Path.Combine(Constants.AmethystLogsDirectory, $"Amethyst_Installer_{logFileDate}.log")));

            // Init OpenVR
            OpenVRUtil.InitOpenVR();

            // Fetch installer API response from server
            InstallerStateManager.Initialize();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.Fatal(Util.FormatException(e.Exception));
            e.Handled = true;

            if ( AppWindow.Instance == null ) {
                // uhhhhhhhhhh how the fuck did you get here
                MessageBox.Show($"An unknown error has occured, please join the Discord server for help at {Constants.DiscordInvite}", "Critical Failure");
                Current.Shutdown();
                return;
            }
            // AFAIK the app should always be initialized, so this scenario should be impossible
            ( AppWindow.Instance.Pages[InstallerState.Exception] as PageException ).currentException = e.Exception;
            AppWindow.Instance.OverridePage(InstallerState.Exception);
        }
    }
}
