using System;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {
    public static class Kernel {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);
    }
}
