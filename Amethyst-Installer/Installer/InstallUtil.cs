using amethyst_installer_gui.Installer;
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
        public static void ExtractZipToDirectory(string sourceArchive, string destinationDirectory) {
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
                        // If the entry Name is not empty, it's a file
                        archiveEntry.ExtractToFile(Path.Combine(destinationDirectory, archiveEntry.FullName));
                    }
                }
            }
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
    }
}
