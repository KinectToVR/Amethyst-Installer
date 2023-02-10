using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;

namespace amethyst_installer_gui.Installer {
    public static class K2EXUtil {

        /// <summary>
        /// Returns the path to a K2EX install, or an empty string if an install wasn't found
        /// </summary>
        public static string LocateK2EX () {

            // First, we check HKLM\SOFTWARE\KinectToVR\InstallPath
            // Then we *naiively* verify that this is a valid K2EX install
            // If this fails, we attempt to locate the KinectToVR driver, and go up a directory and return that
            // If the above fails as well, we assume that K2EX is not present on the current system

            // 1. Get registry entry
            var HKLMSoftware = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
            var K2VRSoftware = HKLMSoftware.OpenSubKey("KinectToVR", true);
            if ( K2VRSoftware != null ) {
                string K2EXPath = (string) K2VRSoftware.GetValue("InstallPath");
                if ( IsValidK2EXInstall(K2EXPath) ) {
                    return K2EXPath;
                }

                if ( K2VRSoftware.GetSubKeyNames().Length == 0 ) {
                    K2VRSoftware.Close();
                    HKLMSoftware.DeleteSubKey("KinectToVR");
                } else {
                    K2VRSoftware.Close();
                }
            }
            HKLMSoftware.Close();

            // 2. Try getting the K2EX driver via SteamVR
            string k2exDriver = Path.GetFullPath(Path.Combine(OpenVRUtil.GetDriverPath("KinectToVR"), ".."));
            if (Directory.Exists(k2exDriver) ) {
                if (IsValidK2EXInstall(k2exDriver)) {
                    return k2exDriver;
                }
            }

            // 3. Assume it's at C:\K2EX
            if (IsValidK2EXInstall("C:\\K2EX")) {
                return "C:\\K2EX";
            }

            // 4. Assume it's at C:\KinectToVR
            if (IsValidK2EXInstall("C:\\KinectToVR") ) {
                return "C:\\KinectToVR";
            }

            return string.Empty;
        }

        private static bool IsValidK2EXInstall(string K2EXPath) {

            // Check if the KinectV1 and V2 binaries exist lol
            if (Directory.Exists(K2EXPath)) {
                if (File.Exists(Path.Combine(K2EXPath, "KinectV1Process.exe")) ||
                    File.Exists(Path.Combine(K2EXPath, "KinectV2Process.exe"))) {
                    return true;
                }
            }

            return false;
        }

        public static bool NukeK2EX(string k2exDir) {

            // The impostor was ejected!
            try {
                // 1. Get hardcoded K2EX uninstall list lol
                string k2exList = Util.ExtractResourceAsString("K2EXList.json");
                UninstallListJSON uninstallList = JsonConvert.DeserializeObject<UninstallListJSON>(k2exList);
                UninstallUtil.DeleteDirectoryUsingUninstallList(k2exDir, uninstallList);

                const string globalStartMenuDir = @"C:\ProgramData\Microsoft\Windows\Start Menu";
                string k2exPath = Path.Combine(globalStartMenuDir, "K2EX");
                string k2exShortcut = Path.Combine(globalStartMenuDir, "K2EX", "KinectToVR.lnk");

                // 2. Try removing the K2EX start menu shortcut
                try {
                    if ( Directory.Exists(k2exPath) ) {
                        if ( File.Exists(k2exShortcut) ) {
                            File.Delete(k2exShortcut);
                        }
                        if ( Directory.GetFiles(k2exPath).Length == 0 && Directory.GetDirectories(k2exPath).Length == 0 ) {
                            Directory.Delete(Path.Combine(globalStartMenuDir, "K2EX"));
                        }
                    }
                } catch ( Exception e ) {
                    Logger.Fatal("Failed to remove K2EX start menu shortcuts!");
                    Logger.Fatal(Util.FormatException(e));
                }

                // 3. Try removing it from the registry
                try {
                    var HKLMSoftware = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
                    var K2VRSoftware = HKLMSoftware.OpenSubKey("KinectToVR", true);
                    if ( K2VRSoftware != null ) {
                        K2VRSoftware.Close();
                        K2VRSoftware.DeleteSubKeyTree("KinectToVR");
                    }
                } catch ( Exception e ) {
                    Logger.Fatal("Failed to remove registry keys!");
                    Logger.Fatal(Util.FormatException(e));
                }

                // 4. Locate the uninstall key, and remove it
                try {
                    var HKLM = Registry.LocalMachine.OpenSubKey(UninstallUtil.UninstallSubKey, true);
                    HKLM.DeleteSubKey("{BA21A8D1-E588-48AB-BF4C-B37E8FB3708E}");
                    HKLM.Close();
                } catch ( Exception e ) {
                    Logger.Fatal("Failed to remove uninstall key!");
                    Logger.Fatal(Util.FormatException(e));
                }

            } catch ( Exception ex ) {
                Logger.Fatal("Failed to nuke K2EX!");
                Logger.Fatal(Util.FormatException(ex));
                return false;
            }
            return true;
        }
    }
}