using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for handling Kinect related tasks, such as ensuring presence of the drivers, checking software things, etc.
    /// </summary>
    public static partial class KinectUtil {

        #region Not Powered Fix

        // An automagic fix for E_NUI_NOTPOWERED
        // This fix involves practically uninstalling all Kinect Drivers, using P/Invoke, then forcing a scan for hardware changes to trigger the drivers to re-scan
        // The method involved with fixing this is subject to change at this point



        #endregion
    }
}
