using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                var displayName = (string)currentNodeKey.GetValue("DisplayName", string.Empty);
                var installLocation = (string)currentNodeKey.GetValue("InstallLocation", string.Empty);
                var modifyPath = (string)currentNodeKey.GetValue("ModifyPath", string.Empty);
                var uninstallString = (string)currentNodeKey.GetValue("UninstallString", string.Empty);

                uninstallEntries.Add(new UninstallEntry() {
                    DisplayName = displayName,
                    InstallLocation = installLocation,
                    ModifyPath = modifyPath,
                    UninstallString = uninstallString,
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

                uninstallEntries.Add(new UninstallEntry() {
                    DisplayName = displayName,
                    InstallLocation = installLocation,
                    ModifyPath = modifyPath,
                    UninstallString = uninstallString,
                });

                currentNodeKey.Close();
            }

            HKLM.Close();
            HKCU.Close();
        }

        public static UninstallEntry? GetUninstallEntry(string amogus) {
            for (int i = 0; i < uninstallEntries.Count; i++ )
                if ( uninstallEntries[i].DisplayName == amogus )
                    return uninstallEntries[i];
            return null;
        }

        public static void RegisterUninstallEntry(UninstallEntry uninstallEntryInfo) {
            RegisterUninstallEntry(uninstallEntryInfo, Guid.NewGuid().ToString());
        }

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
                subkeyEntry.SetValue("Contact",             uninstallEntryInfo.Contact);
                subkeyEntry.SetValue("InstallDate",         DateTime.Now.ToString("yyyyMMdd"));
                subkeyEntry.SetValue("UninstallString",     uninstallEntryInfo.UninstallString);
                subkeyEntry.SetValue("ModifyPath",          uninstallEntryInfo.ModifyPath);
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
        public string Contact;
        public string DisplayIcon;
    }
}
