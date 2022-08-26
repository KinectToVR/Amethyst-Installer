using amethyst_installer_gui.Installer;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace amethyst_installer_gui {
    public static class UninstallUtil {
        private const string UninstallSubKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";

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


            } catch {
                if ( subkeyEntry != null ) {
                    subkeyEntry.Close();
                }
                throw;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UninstallAmethyst(bool removeConfig = true) {
            // Removes Amethyst according to a list of files

            // 1. Locate the current Amethyst install
            // We first check if the directory containing the installer is an Amethyst install. If so we continue to stage 2.
            // If this isn't the case, we check the registry key for the path variable (I have no clue how fucked one's setup could be so fallbacks!!)
            // If this still isn't the case check in C:\\Amethyst, C:\\Program Files\\Amethyst, C:\\Program Files (x86)\\Amethyst and C:\\K2EX for installs

            string ameInstall = InstallUtil.LocateAmethystInstall();

            // 2. Now that we have the install directory, check in %APPDATA% for an uninstall list. If we find one, load it and use it
            // Now based on said uninstall list, clean the directory
            UninstallListJSON uninstallList = FetchUninstallList();
            // Remove files
            for ( int i = 0; i < uninstallList.Files.Length; i++ ) {
                string file = Path.Combine(ameInstall, uninstallList.Files[i]);
                if (File.Exists(file) )
                    File.Delete(file);
            }
            // Remove directories
            for ( int i = 0; i < uninstallList.Directories.Length; i++ ) {
                string dir = Path.Combine(ameInstall, uninstallList.Directories[i]);
                if (Directory.Exists(dir))
                    Directory.Delete(dir);
            }

            // 3. If the directory is empty, remove it
            if ( Directory.Exists(ameInstall) ) {

            }

            // 4. Locate the uninstall key, and remove it

            // 5. Locate %APPDATA%\Amethyst, and clear configs

            // 6. Remove registry key; the user has completely uninstalled Amethyst


        }

        private static UninstallListJSON FetchUninstallList() {
            UninstallListJSON uninstallList = null;
            string uninstallListText = string.Empty;
            string uninstallListPath = Path.GetFullPath(Path.Combine(Constants.AmethystConfigDirectory, "UninstallList.json"));

            if ( File.Exists(uninstallListPath) ) {
                try {
                    uninstallListText = File.ReadAllText(uninstallListPath);
                    uninstallList = JsonConvert.DeserializeObject<UninstallListJSON>(uninstallListPath);
                } catch {
                    uninstallListText = Util.ExtractResourceAsString("UninstallList.json");
                }
            }

            if ( uninstallListText.Length == 0 ) {
                uninstallListText = Util.ExtractResourceAsString("UninstallList.json");
            }
            if ( uninstallList == null ) {
                uninstallList = JsonConvert.DeserializeObject<UninstallListJSON>(uninstallListPath);
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