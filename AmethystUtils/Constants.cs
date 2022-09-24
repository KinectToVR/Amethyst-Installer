using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AmethystUtils {
    /// <summary>
    /// A static class containing constants such as directories
    /// </summary>
    public static class Constants {
        /// <summary>
        /// Returns the equivalent of %USERPROFILE%, except smarter as it behaves properly with Admin. Since this is an unelevated process
        /// we don't bother with that, as anything which requires that will de-elevate anyway require us to call Amethyst Installer
        /// </summary>
        public static string Userprofile {
            get {
                return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }
        }

        /// <summary>
        /// Returns the config directory for Amethyst
        /// </summary>
        public static string AmethystConfigDirectory {
            get {
                return Path.GetFullPath(Path.Combine(Userprofile, "AppData", "Roaming", "Amethyst"));
            }
        }

        /// <summary>
        /// Returns the logging directory for Amethyst
        /// </summary>
        public static string AmethystLogsDirectory {
            get {
                return Path.GetFullPath(Path.Combine(Userprofile, "AppData", "Roaming", "Amethyst", "logs"));
            }
        }

        /// <summary>
        /// Returns the path to the Amethyst Installer Executable
        /// </summary>
        public static string AmethystInstallerExecutablePath {
            get {
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Amethyst-Installer.exe"));
            }
        }

        /// <summary>
        /// Returns the path to the Amethyst Executable
        /// </summary>
        public static string AmethystExecutablePath {
            get {
                return Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Amethyst.exe"));
            }
        }

    }
}
