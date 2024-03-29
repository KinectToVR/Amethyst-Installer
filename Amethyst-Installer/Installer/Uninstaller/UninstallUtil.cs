﻿using amethyst_installer_gui.Installer;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace amethyst_installer_gui {
    public static class UninstallUtil {
        internal const string UninstallSubKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

        private static List<UninstallEntry> uninstallEntries;

        static UninstallUtil() {
            uninstallEntries = new List<UninstallEntry>();

            var HKLM = Registry.LocalMachine.OpenSubKey(UninstallSubKey, false);
            var HKLM_UninstallNodes = HKLM.GetSubKeyNames();
            for ( int i = 0; i < HKLM_UninstallNodes.Length; i++ ) {

                var currentNodeKey = HKLM.OpenSubKey(HKLM_UninstallNodes[i]);
                var displayName         = (string)currentNodeKey.GetValue("DisplayName",        string.Empty);
                var installLocation     = (string)currentNodeKey.GetValue("InstallLocation",    string.Empty);
                var modifyPath          = (string)currentNodeKey.GetValue("ModifyPath",         string.Empty);
                var uninstallString     = (string)currentNodeKey.GetValue("UninstallString",    string.Empty);
                var applicationVersion  = (string)currentNodeKey.GetValue("ApplicationVersion", string.Empty);
                var displayVersion      = (string)currentNodeKey.GetValue("DisplayVersion",     string.Empty);
                var publisher           = (string)currentNodeKey.GetValue("Publisher",          string.Empty);
                var urlInfoAbout        = (string)currentNodeKey.GetValue("URLInfoAbout",       string.Empty);
                var helpLink            = (string)currentNodeKey.GetValue("HelpLink",           string.Empty);
                var displayIcon         = (string)currentNodeKey.GetValue("DisplayIcon",        string.Empty);

                uninstallEntries.Add(new UninstallEntry() {

                    DisplayName = displayName,
                    InstallLocation = installLocation,
                    ModifyPath = modifyPath,
                    UninstallString = uninstallString,
                    ApplicationVersion = applicationVersion,
                    DisplayVersion = displayVersion,
                    Publisher = publisher,
                    URLInfoAbout = urlInfoAbout,
                    HelpLink = helpLink,
                    DisplayIcon = displayIcon,
                });

                currentNodeKey.Close();
            }

            var HKCU = Registry.CurrentUser.OpenSubKey(UninstallSubKey, false);
            var HKCU_UninstallNodes = HKCU.GetSubKeyNames();
            for ( int i = 0; i < HKCU_UninstallNodes.Length; i++ ) {

                var currentNodeKey      = HKCU.OpenSubKey(HKCU_UninstallNodes[i]);
                var displayName         = (string)currentNodeKey.GetValue("DisplayName",        string.Empty);
                var installLocation     = (string)currentNodeKey.GetValue("InstallLocation",    string.Empty);
                var modifyPath          = (string)currentNodeKey.GetValue("ModifyPath",         string.Empty);
                var uninstallString     = (string)currentNodeKey.GetValue("UninstallString",    string.Empty);
                var applicationVersion  = (string)currentNodeKey.GetValue("ApplicationVersion", string.Empty);
                var displayVersion      = (string)currentNodeKey.GetValue("DisplayVersion",     string.Empty);
                var publisher           = (string)currentNodeKey.GetValue("Publisher",          string.Empty);
                var urlInfoAbout        = (string)currentNodeKey.GetValue("URLInfoAbout",       string.Empty);
                var helpLink            = (string)currentNodeKey.GetValue("HelpLink",           string.Empty);
                var displayIcon         = (string)currentNodeKey.GetValue("DisplayIcon",        string.Empty);

                uninstallEntries.Add(new UninstallEntry() {

                    DisplayName         = displayName,
                    InstallLocation     = installLocation,
                    ModifyPath          = modifyPath,
                    UninstallString     = uninstallString,
                    ApplicationVersion  = applicationVersion,
                    DisplayVersion      = displayVersion,
                    Publisher           = publisher,
                    URLInfoAbout        = urlInfoAbout,
                    HelpLink            = helpLink,
                    DisplayIcon         = displayIcon,
                });

                currentNodeKey.Close();
            }

            HKLM.Close();
            HKCU.Close();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static UninstallEntry? GetUninstallEntry(string amogus) {
            for (int i = 0; i < uninstallEntries.Count; i++ )
                if ( uninstallEntries[i].DisplayName == amogus )
                    return uninstallEntries[i];
            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterUninstallEntry(UninstallEntry uninstallEntryInfo) {
            RegisterUninstallEntry(uninstallEntryInfo, Guid.NewGuid().ToString());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RegisterUninstallEntry(UninstallEntry uninstallEntryInfo, string keyName) {

            RegistryKey subkeyEntry = null;

            try {

                var HKLM = Registry.LocalMachine.OpenSubKey(UninstallSubKey, true);
                subkeyEntry = HKLM.OpenSubKey(keyName, true) ?? HKLM.CreateSubKey(keyName);

                subkeyEntry.SetValue("DisplayName",         uninstallEntryInfo.DisplayName);
                subkeyEntry.SetValue("ApplicationVersion",  uninstallEntryInfo.ApplicationVersion);
                subkeyEntry.SetValue("Publisher",           uninstallEntryInfo.Publisher);
                subkeyEntry.SetValue("DisplayIcon",         uninstallEntryInfo.DisplayIcon);
                subkeyEntry.SetValue("DisplayVersion",      uninstallEntryInfo.DisplayVersion);
                subkeyEntry.SetValue("URLInfoAbout",        uninstallEntryInfo.URLInfoAbout);
                subkeyEntry.SetValue("HelpLink",            uninstallEntryInfo.HelpLink);
                subkeyEntry.SetValue("InstallDate",         DateTime.Now.ToString("yyyyMMdd"));
                subkeyEntry.SetValue("UninstallString",     uninstallEntryInfo.UninstallString);
                // @TODO: Add modify back once we have a modify workflow implemented
                // subkeyEntry.SetValue("ModifyPath",          uninstallEntryInfo.ModifyPath);
                subkeyEntry.SetValue("InstallLocation",     uninstallEntryInfo.InstallLocation);
                // subkeyEntry.SetValue("HelpTelephone",       "441173257425");
                subkeyEntry.SetValue("EstimatedSize",       new DirectoryInfo(uninstallEntryInfo.InstallLocation).EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length) / 1024, RegistryValueKind.DWord); // MB

                subkeyEntry.Close();
                HKLM.Close();
            } catch {
                if ( subkeyEntry != null ) {
                    subkeyEntry.Close();
                }
                throw;
            }
        }

        /// <summary>
        /// Uninstalls Amethyst
        /// </summary>
        /// <param name="removeConfig">Whether to keep configs or not</param>
        /// <param name="ameDirectory">The directory of the Amethyst install we're trying to remove</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UninstallAmethyst(bool removeConfig = true, string ameDirectory = "") {
            // Removes Amethyst according to a list of files

            // 1. Locate the current Amethyst install
            // We first check if the directory containing the installer is an Amethyst install. If so we continue to stage 2.
            // If this isn't the case, we check the registry key for the path variable (I have no clue how fucked one's setup could be so fallbacks!!)
            // If this still isn't the case check in C:\\Amethyst, C:\\Program Files\\Amethyst, C:\\Program Files (x86)\\Amethyst and C:\\K2EX for installs

            string ameInstall;
            if ( ameDirectory.Length == 0) {
                ameInstall = InstallUtil.LocateAmethystInstall();
            } else {
                ameInstall = ameDirectory;
            }

            if ( !Directory.Exists(ameInstall) ) {
                Logger.Fatal("Couldn't locate Amethyst install! Aborting...");
                return;
            }

            InstallUtil.TryKillingConflictingProcesses();

            // 2. Now that we have the install directory, check in %APPDATA% for an uninstall list. If we find one, load it and use it
            // Now based on said uninstall list, clean the directory
            UninstallListJSON uninstallList = FetchUninstallList();

            // Remove files
            /*
            for ( int i = 0; i < uninstallList.Files.Length; i++ ) {
                string file = Path.Combine(ameInstall, uninstallList.Files[i]);
                if (File.Exists(file) )
                    File.Delete(file);
            }
            // Remove directories
            for ( int i = 0; i < uninstallList.Directories.Length; i++ ) {
                string dir = Path.Combine(ameInstall, uninstallList.Directories[i]);
                if ( Directory.Exists(dir) ) {
                    // Check to make sure the directory isn't empty
                    if ( Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length == 0 &&
                         Directory.GetDirectories(dir, "*", SearchOption.AllDirectories).Length == 0 ) {
                        Directory.Delete(dir);
                    }
                }
            }

            // 3. If the directory is empty, remove it
            if ( Directory.Exists(ameInstall) ) {
                // If there are 0 files or directories left
                if ( Directory.GetFiles(ameInstall, "*", SearchOption.AllDirectories).Length == 0 &&
                     Directory.GetDirectories(ameInstall, "*", SearchOption.AllDirectories).Length == 0 ) {
                    Directory.Delete(ameInstall);
                }
            }
            */

            DeleteDirectoryUsingUninstallList(ameInstall, uninstallList);

            // 4. Locate the uninstall key, and remove it
            try {
                var HKLM = Registry.LocalMachine.OpenSubKey(UninstallSubKey, true);
                HKLM.DeleteSubKey("Amethyst");
                HKLM.Close();
            } catch (Exception e) {
                Logger.Fatal("Failed to remove uninstall key!");
                Logger.Fatal(Util.FormatException(e));
            }

            // 5. Locate %APPDATA%\Amethyst, and clear configs
            if ( removeConfig ) {

                if ( Directory.Exists(Constants.AmethystConfigDirectory) ) {
                    
                    // Settings
                    string[] settings = Directory.GetFiles(Constants.AmethystConfigDirectory, "*_settings.xml", SearchOption.TopDirectoryOnly);
                    for ( int i = 0; i < settings.Length; i++ ) {
                        if ( File.Exists(settings[i]) ) {
                            File.Delete(settings[i]);
                        }
                    }

                    // Installer configs
                    if ( File.Exists(Path.Combine(Constants.AmethystConfigDirectory, "UninstallList.json")) ) {
                        File.Delete(Path.Combine(Constants.AmethystConfigDirectory, "UninstallList.json"));
                    }

                    // Logs
                    string[] logs = Directory.GetFiles(Path.Combine(Constants.AmethystConfigDirectory, "logs"), "*.log", SearchOption.TopDirectoryOnly);

                    for ( int i = 0; i < logs.Length; i++ ) {
                        if ( File.Exists(logs[i]) && logs[i] != Logger.LogFilePath ) {
                            File.Delete(logs[i]);
                        }
                    }

                    // If the directory is empty, remove it
                    if ( Directory.Exists(Constants.AmethystConfigDirectory) ) {
                        if ( Directory.GetFiles(Constants.AmethystConfigDirectory, "*", SearchOption.AllDirectories).Length == 0 &&
                             Directory.GetDirectories(Constants.AmethystConfigDirectory, "*", SearchOption.AllDirectories).Length == 0 ) {
                            Directory.Delete(Constants.AmethystConfigDirectory);
                        }
                    }
                }

            }

            // 6. Remove registry key; the user has completely uninstalled Amethyst
            try {

                var HKLMSoftware = Registry.LocalMachine.OpenSubKey(@"SOFTWARE", true);
                var K2VRSoftware = HKLMSoftware.OpenSubKey("K2VR Team", true);
                if ( K2VRSoftware != null ) {
                    var Ame = K2VRSoftware.OpenSubKey("Amethyst", true);
                    if ( Ame != null ) {
                        Ame.Close();
                        K2VRSoftware.DeleteSubKey("Amethyst");
                    }

                    if ( K2VRSoftware.GetSubKeyNames().Length == 0 ) {
                        K2VRSoftware.Close();
                        HKLMSoftware.DeleteSubKey("K2VR Team");
                    } else {
                        K2VRSoftware.Close();
                    }
                }
                HKLMSoftware.Close();
            } catch (Exception e) {
                Logger.Fatal("Failed to remove registry entry!");
                Logger.Fatal(Util.FormatException(e));
            }

            // 7. Remove Amethyst Tracker Roles
            {
                Logger.Info(LogStrings.RemovingTrackerRoles);

                try {
                    // Remove all Amethyst tracker roles
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-00WAIST0");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-00WAIST00");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-CHEST");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-L0ELBOW0");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-L0FOOT00");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-L0KNEE00");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-LELBOW");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-LFOOT");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-LKNEE");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-R0ELBOW0");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-R0FOOT00");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-R0KNEE00");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-RELBOW");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-RFOOT");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-RKNEE");
                    OpenVRUtil.RemoveTrackerRole("/devices/amethyst/vr_tracker/AME-WAIST");

                    // Just in case, K2EX tracker roles too

                    // Ancient versions of KinectToVR
                    OpenVRUtil.RemoveTrackerRole("/devices/00vrinputemulator/0");
                    OpenVRUtil.RemoveTrackerRole("/devices/00vrinputemulator/1");
                    OpenVRUtil.RemoveTrackerRole("/devices/00vrinputemulator/2");
                    OpenVRUtil.RemoveTrackerRole("/devices/00vrinputemulator/3");

                    // For most of K2EX's lifespan we tried mimicking Vive Trackers, sorry cnlohr (pls don't look at the OpenVrUtils files k thx)
                    OpenVRUtil.RemoveTrackerRole("/devices/htc/vive_trackerLHR-CB11ABEC");
                    OpenVRUtil.RemoveTrackerRole("/devices/htc/vive_trackerLHR-CB1441A7");
                    OpenVRUtil.RemoveTrackerRole("/devices/htc/vive_trackerLHR-CB9AD1T0");
                    OpenVRUtil.RemoveTrackerRole("/devices/htc/vive_trackerLHR-CB9AD1T1");
                    OpenVRUtil.RemoveTrackerRole("/devices/htc/vive_trackerLHR-CB9AD1T2");

                    // In K2EX 0.9.1 we use custom serial ids
                    OpenVRUtil.RemoveTrackerRole("/devices/KinectToVR/Puck_HIP");
                    OpenVRUtil.RemoveTrackerRole("/devices/KinectToVR/Puck_LFOOT");
                    OpenVRUtil.RemoveTrackerRole("/devices/KinectToVR/Puck_RFOOT");
                } catch ( Exception e ) {
                    Logger.Fatal($"{LogStrings.FailRemoveTrackerRoles}:\n{Util.FormatException(e)})");
                }
            }
        }

        public static void DeleteDirectoryUsingUninstallList(string directory, UninstallListJSON uninstallList) {

            // Remove files
            for ( int i = 0; i < uninstallList.Files.Length; i++ ) {
                string file = Path.Combine(directory, uninstallList.Files[i]);
                if ( File.Exists(file) )
                    File.Delete(file);
            }
            // Remove directories
            for ( int i = 0; i < uninstallList.Directories.Length; i++ ) {
                string dir = Path.Combine(directory, uninstallList.Directories[i]);
                if ( Directory.Exists(dir) ) {
                    // Check to make sure the directory isn't empty
                    if ( Directory.GetFiles(dir, "*", SearchOption.AllDirectories).Length == 0 &&
                         Directory.GetDirectories(dir, "*", SearchOption.AllDirectories).Length == 0 ) {
                        Directory.Delete(dir);
                    }
                }
            }

            // If the directory is empty, remove it
            if ( Directory.Exists(directory) ) {
                // If there are 0 files or directories left
                if ( Directory.GetFiles(directory, "*", SearchOption.AllDirectories).Length == 0 &&
                     Directory.GetDirectories(directory, "*", SearchOption.AllDirectories).Length == 0 ) {
                    Directory.Delete(directory);
                }
            }
        }

        private static UninstallListJSON FetchUninstallList() {
            UninstallListJSON uninstallList = null;
            string uninstallListText = string.Empty;
            string uninstallListPath = Path.GetFullPath(Path.Combine(Constants.AmethystConfigDirectory, "UninstallList.json"));

            if ( File.Exists(uninstallListPath) ) {
                try {
                    uninstallListText = File.ReadAllText(uninstallListPath);
                    uninstallList = JsonConvert.DeserializeObject<UninstallListJSON>(uninstallListText);
                } catch {
                    uninstallListText = Util.ExtractResourceAsString("UninstallList.json");
                }
            }

            if ( uninstallListText.Trim().Replace("\0", "").Length == 0 ) {
                uninstallListText = Util.ExtractResourceAsString("UninstallList.json");
            }
            if ( uninstallList == null ) {
                uninstallList = JsonConvert.DeserializeObject<UninstallListJSON>(uninstallListText);
            }

            return uninstallList;
        }
    }

    public struct UninstallEntry {
        public string DisplayName;
        public string InstallLocation;
        public string ModifyPath;
        public string UninstallString;
        public string ApplicationVersion;
        public string DisplayVersion;
        public string Publisher;
        public string URLInfoAbout;
        public string HelpLink;
        public string DisplayIcon;
    }
}