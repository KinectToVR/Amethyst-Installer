using amethyst_installer_gui.PInvoke;
using System;
using System.IO;

namespace amethyst_installer_gui {
    /// <summary>
    /// A static class containing constants such as directories
    /// </summary>
    public static class Constants {
        /// <summary>
        /// Returns the equivalent of %USERPROFILE%, except smarter as it behaves properly with Admin
        /// </summary>
        public static string Userprofile {
            get {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "..", CurrentUser.GetCurrentlyLoggedInUsername()));
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
        /// Returns a temporary directory, in %TEMP% in Release mode, and in ./temp in Debug mode (to make troubleshooting easier)
        /// </summary>
        public static string AmethystTempDirectory {
            get {
                if ( m_ameTmpDir == null || m_ameTmpDir == "" ) {
#if DEBUG
                    m_ameTmpDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "temp"));
#else
                    m_ameTmpDir = Path.GetFullPath(Path.Combine(Userprofile, "AppData", "Local", "Temp", "amethyst-installer"));
#endif
                    if ( !Directory.Exists(m_ameTmpDir) ) {
                        Directory.CreateDirectory(m_ameTmpDir);
                        Logger.Info($"Created temp directory at \"{m_ameTmpDir}\"...");
                    }
                }
                return m_ameTmpDir;
            }
        }
        // use a static variable to not pollute the tmp dir
        private static string m_ameTmpDir = null;

        /// <summary>
        /// An invite to the K2VR Community Discord server
        /// </summary>
        public static readonly string DiscordInvite = "https://discord.gg/YBQCRDG";
    }
}
