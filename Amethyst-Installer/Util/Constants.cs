using System;
using System.IO;

namespace amethyst_installer_gui
{
    /// <summary>
    /// A static class containing constants such as directories
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Returns the logging directory for Amethyst
        /// </summary>
        public static string AmethystLogsDirectory
        {
            get
            {
                return Path.GetFullPath(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Amethyst", "logs"));
            }
        }

        /// <summary>
        /// Returns a temporary directory which is 
        /// </summary>
        public static string AmethystTempDirectory
        {
            get
            {
                if (m_ameTmpDir == null || m_ameTmpDir == "")
                    m_ameTmpDir = Path.GetTempPath();
                return m_ameTmpDir;
            }
        }
        private static string m_ameTmpDir = Path.GetTempPath();
    }
}
