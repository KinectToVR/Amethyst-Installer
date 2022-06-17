using Microsoft.Win32;
using System;
using System.Collections.Generic;

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
            }

            var HKCU = Registry.CurrentUser.OpenSubKey(UninstallSubKey, false);
            var HKCU_UninstallNodes = HKCU.GetSubKeyNames();
            for ( int i = 0; i < HKCU_UninstallNodes.Length; i++ ) {

                var currentNodeKey = HKCU.OpenSubKey(HKCU_UninstallNodes[i]);
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
            }
        }

        public static UninstallEntry? GetUninstallEntry(string amogus) {
            for (int i = 0; i < uninstallEntries.Count; i++ )
                if ( uninstallEntries[i].DisplayName == amogus )
                    return uninstallEntries[i];
            return null;
        }

        public static void RegisterUninstallEntry(UninstallEntry uninstallEntryInfo) {
            Guid guid = Guid.NewGuid();
            var HKLM = Registry.LocalMachine.OpenSubKey(UninstallSubKey, true);
            var subkeyEntry = HKLM.CreateSubKey(guid.ToString());
        }
    }

    public struct UninstallEntry {
        public string DisplayName;
        public string InstallLocation;
        public string ModifyPath;
        public string UninstallString;
    }
}
