using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            // TODO: Implement K2EX detection



            return string.Empty;
        }
    }
}
