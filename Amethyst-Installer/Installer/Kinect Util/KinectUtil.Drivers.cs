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

        #region Drivers

        // P/Invoke fuckery to check Device Nodes

        // Checking if the current Kinect Drivers setup is fucked::
        // 1. SetupDiGetClassDevs => fuckery to get up to => Some Konct Device::Check Status
        //              OR
        // 2. CM_Get_DevNode_Registry_Property_Ex type shit for each device => if class == GUID {0} and description includes "Kinect" probably NOT_READY
        //        OR count number of devices under kinect device class GUID

        // Removing drivers::
        //     (effectively "Uninstall Device" without deleting the software
        //     CM_Uninstall_DevNode
        //
        // Removing corrupt drivers::
        //     (effectively "Uninstall Device" AND deleting the software
        //     DiUninstallDevice followed by SetupUninstallOEMInf
        //
        // need to talk with Ella about this bit

        #endregion
    }
}
