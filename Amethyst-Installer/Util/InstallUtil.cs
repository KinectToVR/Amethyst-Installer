using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui {
    public static class InstallUtil {

        /// <summary>
        /// Returns whether a copy of Amethyst is installed at a given path
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsAmethystInstalledInDirectory(string path) {

            if ( Directory.Exists(path) ) {
                if ( File.Exists(Path.Combine(path, "Amethyst.exe")) ) {
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
            // If this still isn't the case check in C:\\Amethyst, C:\\Program Files\\Amethyst, C:\\Program Files (x86)\\Amethyst and C:\\K2EX for installs

            return "";
        }
    }
}
