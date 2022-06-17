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

            InstallerStateManager.Initialize();
        }

        private void App_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            Logger.Fatal($"Unhandled Exception: {e.Exception.GetType().Name} in {e.Exception.Source}: {e.Exception.Message}\n{e.Exception.StackTrace}");
            e.Handled = true;

            // TODO: Check if the main window is initialized?
            // AFAIK the app should always be initialized, so this scenario should be impossible
            ( AppWindow.Instance.Pages[InstallerState.Exception] as PageException ).currentException = e.Exception;
            AppWindow.Instance.OverridePage(InstallerState.Exception);
        }
    }
}
