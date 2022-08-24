using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static partial class KinectUtil {

        // Check whether memory integrity is enabled or not in Windows Defender / Windows Security via the registry
        // https://docs.microsoft.com/en-us/windows/security/threat-protection/device-guard/enable-virtualization-based-protection-of-code-integrity

        public static void FixNotReady() {

        }

        public static void MustFixNotReady() {

        }

        private static bool IsMemoryIntegrityEnabled () {



            return false;
        }
    }
}
