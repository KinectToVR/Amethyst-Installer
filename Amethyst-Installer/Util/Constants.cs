using amethyst_installer_gui.PInvoke;
using System;
using System.IO;
using System.Windows.Media;

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
                if ( m_ameTmpDir == null || m_ameTmpDir.Length == 0 ) {
#if DEBUG
                    m_ameTmpDir = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "temp"));
#else
                    m_ameTmpDir = Path.GetFullPath(Path.Combine(Userprofile, "AppData", "Local", "Temp", $"amethyst-installer-{Path.GetRandomFileName().Replace(".", "")}"));
#endif
                    if ( Directory.Exists(m_ameTmpDir) ) {
                        Directory.Delete(m_ameTmpDir, true); // Clear previous temp dir if it exists
                    }

                    Directory.CreateDirectory(m_ameTmpDir);
                    Logger.Info($"Created temp directory at \"{m_ameTmpDir}\"...");
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

        /// <summary>
        /// A mapping from <see cref="ConsoleColor"/> to <see cref="SolidColorBrush"/>
        /// </summary>
        public static SolidColorBrush[] ConsoleBrushColors = new[]
        {
            new SolidColorBrush(Colors.Black),
            new SolidColorBrush(Colors.DarkBlue),
            new SolidColorBrush(Colors.DarkGreen),
            new SolidColorBrush(Colors.DarkCyan),
            new SolidColorBrush(Color.FromArgb(255,255,132,132)), // Fatal
            new SolidColorBrush(Colors.DarkMagenta),
            new SolidColorBrush(Colors.DarkOliveGreen),
            new SolidColorBrush(Colors.Gray),
            new SolidColorBrush(Colors.DarkGray),
            new SolidColorBrush(Colors.Blue),
            new SolidColorBrush(Colors.Green),
            new SolidColorBrush(Colors.Cyan),
            new SolidColorBrush(Color.FromArgb(255,255,120,0)),    // Error
            new SolidColorBrush(Colors.Magenta),
            new SolidColorBrush(Color.FromArgb(255, 255, 255, 86)), // Warn
            new SolidColorBrush(Colors.White)   // Info
        };
    }
}
