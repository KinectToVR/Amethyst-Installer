using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui {
    public static class InstallUtil {

        /// <summary>
        /// Extracts a zip file to a specified directory. This is different from <see cref="System.IO.Compression.ZipFile.ExtractToDirectory(string, string)"/>
        /// </summary>
        public static bool ExtractZipToDirectory(string sourceArchive, string destinationDirectory) {
            using ( var zip = ZipFile.OpenRead(sourceArchive) ) {
                foreach ( var archiveEntry in zip.Entries ) {

                    // Check if we're handling a directory or a file
                    if ( archiveEntry.Name.Length == 0 ) {
                        // Directories' Name is an empty string
                        string finalDir = Path.Combine(destinationDirectory, archiveEntry.FullName);
                        if ( !Directory.Exists(finalDir) ) {
                            Directory.CreateDirectory(finalDir);
                        }
                    } else {
                        try {
                            // If the entry Name is not empty, it's a file
                            string fullPath = Path.Combine(destinationDirectory, archiveEntry.FullName);

                            // Ensure the directory exists, in case
                            string dirName = Path.GetDirectoryName(fullPath);
                            if ( !Directory.Exists(dirName) )
                                Directory.CreateDirectory(dirName);

                            archiveEntry.ExtractToFile(fullPath, true);

                            // Unblock all executable files
                            if ( Path.GetExtension(fullPath) == ".exe" ) {
                                Shell.Unblock(fullPath);
                            }
                        } catch ( Exception ex ) {
                            if (Util.IsDiskFull(ex)) {
                                Util.ShowMessageBox(Localisation.InstallFailure_DiskFull_Description, Localisation.InstallFailure_DiskFull_Title);
                                return false;
                            }
                            throw ex;
                        }
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Returns whether a copy of Amethyst is installed at a given path
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAmethystInstalledInDirectory(string path) {

            if ( Directory.Exists(path) ) {
                if (
                    File.Exists(Path.Combine(path, "Amethyst.exe")) &&                          // If we located the main Amethyst executable
                    File.Exists(Path.Combine(path, "K2CrashHandler", "K2CrashHandler.exe")) &&  // If we located the K2 Crash Handler executable
                    Directory.Exists(Path.Combine(path, "Amethyst"))                            // If we located the Amethyst SteamVR driver
                    ) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Locates an Amethyst install
        /// </summary>
        public static string LocateAmethystInstall() {
            // We first check if the directory containing the installer is an Amethyst install. If so we continue to stage 2.
            // If this isn't the case, we check the registry key for the path variable (I have no clue how fucked one's setup could be so fallbacks!!)
            // If this isn't the case, try locating the Amethyst SteamVR driver
            // If this still isn't the case check in C:\\Amethyst, C:\\Program Files\\Amethyst, C:\\Program Files (x86)\\Amethyst and C:\\K2EX for installs

            // Check if the current install is an Amethyst install
            string executingDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if ( IsAmethystInstalledInDirectory(executingDirectory) ) {
                return executingDirectory;
            }

            // That failed! Let's check the registry next
            var AmeReg = Registry.LocalMachine.OpenSubKey(Constants.AmethystRegsitryKey, false)?.GetValue("Path");
            if ( AmeReg != null ) {
                string pathFull = Path.GetFullPath(AmeReg as string);
                if ( IsAmethystInstalledInDirectory(pathFull) ) {
                    return pathFull;
                }
            }

            // The user most likely either *doesn't* have Amethyst installed, or has manually installed it.
            // As a result, let's attempt locating the driver via SteamVR driver paths
            var ameDriver = OpenVRUtil.GetDriverPath("Amethyst");
            if ( ameDriver.Length > 0) {
                ameDriver = Path.GetFullPath(ameDriver);
                if (IsAmethystInstalledInDirectory(ameDriver)) {
                    return ameDriver;
                }
            }

            // As a last resort, manually check some common directories for an Amethyst install
            if (IsAmethystInstalledInDirectory("C:\\Amethyst") ) {
                return "C:\\Amethyst";
            }
            if ( IsAmethystInstalledInDirectory(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Amethyst"))) ) {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Amethyst"));
            }
            if ( IsAmethystInstalledInDirectory(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Amethyst"))) ) {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Amethyst"));
            }

            // OK, now let's check for the edge case of a user having manually upgraded from K2EX to Amethyst, replacing their
            // existing K2EX install with Amethyst. This means that Amethyst is installed in C:\\K2EX for instance...

            // Let's check the registry next
            var K2EXReg = Registry.LocalMachine.OpenSubKey(@"Software\KinectToVR", false)?.GetValue("InstallPath");
            if ( K2EXReg != null ) {
                string pathFull = Path.GetFullPath(K2EXReg as string);
                if ( IsAmethystInstalledInDirectory(pathFull) ) {
                    return pathFull;
                }
            }

            // Check for people who replaced a K2EX install with an Amethyst install manually
            if ( IsAmethystInstalledInDirectory("C:\\K2EX") ) {
                return "C:\\K2EX";
            }
            if ( IsAmethystInstalledInDirectory(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "K2EX"))) ) {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "K2EX"));
            }
            if ( IsAmethystInstalledInDirectory(Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "K2EX"))) ) {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "K2EX"));
            }

            return string.Empty;
        }

        /// <summary>
        /// Kills a long list of processes which are known to conflict with the installation process
        /// due to them having a high chance of opening SteamVR during the install process.
        /// </summary>
        public static void TryKillingConflictingProcesses() {

            // K2EX
            Util.ForceKillProcess("kinectv1process");
            Util.ForceKillProcess("kinectv2process");
            Util.ForceKillProcess("psmsprocess");
            Util.ForceKillProcess("kinecttovr");

            // Amethyst
            Util.ForceKillProcess("amethyst");
            Util.ForceKillProcess("k2crashhandler");

            // ALVR
            Util.ForceKillProcess("alvr launcher");

            // PiTool
            Util.ForceKillProcess("pitool");
            Util.ForceKillProcess("pimaxclient");

            // WMR
            Util.ForceKillProcess("MixedRealityPortal.Brokered");
            Util.ForceKillProcess("MixedRealityPortal");
            Util.ForceKillProcess("WUDFHost");

            // Vive Wireless Software
            Util.ForceKillProcess("htcconnectionutility");

            // Pico Neo Software
            // @TODO: Pico Neo Software
            // Util.ForceKillProcess("htcconnectionutility");

            // Revive
            Util.ForceKillProcess("reviveinjector");
            Util.ForceKillProcess("reviveoverlay");

            // SteamVR
            Util.ForceKillProcess("vrmonitor");
            Util.ForceKillProcess("vrdashboard");
            Util.ForceKillProcess("vrserver");
            Util.ForceKillProcess("vrservice");
            Util.ForceKillProcess("vrserverhelper");
            Util.ForceKillProcess("vrcompositor");
            Util.ForceKillProcess("vrstartup");
            Util.ForceKillProcess("vrwebhelper");
            Util.ForceKillProcess("overlay_viewer");
            Util.ForceKillProcess("removeusbhelper");
            Util.ForceKillProcess("restarthelper");
            Util.ForceKillProcess("vrcmd");
            Util.ForceKillProcess("vrpathreg");
            Util.ForceKillProcess("vrprismhost");
            Util.ForceKillProcess("vrurlhandler");

            // SteamVR Lighthouse devices
            Util.ForceKillProcess("vivelink");
            Util.ForceKillProcess("vivetools");
            Util.ForceKillProcess("vivebtdriver");
            Util.ForceKillProcess("vivebtdriver_win10");
            Util.ForceKillProcess("lighthouse_console");
            Util.ForceKillProcess("lighthouse_watchman_update");
            Util.ForceKillProcess("nrfutil");

            // VirtualDesktop
            Util.ForceKillProcess("virtualdesktop.streamer");

            // Oculus processes
            Util.ForceKillProcess("oculusclient");
            Util.ForceKillProcess("oculusdash");

            // SteamVR (again, just for good measure)
            Util.ForceKillProcess("vrmonitor");
            Util.ForceKillProcess("vrdashboard");
            Util.ForceKillProcess("vrserver");
            Util.ForceKillProcess("vrservice");
            Util.ForceKillProcess("vrserverhelper");
            Util.ForceKillProcess("vrcompositor");
            Util.ForceKillProcess("vrstartup");
            Util.ForceKillProcess("vrwebhelper");
            Util.ForceKillProcess("overlay_viewer");
            Util.ForceKillProcess("removeusbhelper");
            Util.ForceKillProcess("restarthelper");
            Util.ForceKillProcess("vrcmd");
            Util.ForceKillProcess("vrpathreg");
            Util.ForceKillProcess("vrprismhost");
            Util.ForceKillProcess("vrurlhandler");

            // SteamVR Lighthouse devices (for good measure)
            Util.ForceKillProcess("vivelink");
            Util.ForceKillProcess("vivetools");
            Util.ForceKillProcess("vivebtdriver");
            Util.ForceKillProcess("vivebtdriver_win10");
            Util.ForceKillProcess("lighthouse_console");
            Util.ForceKillProcess("lighthouse_watchman_update");
            Util.ForceKillProcess("nrfutil");

        }
    }
}
