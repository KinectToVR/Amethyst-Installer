using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.PInvoke {
    public static class SetupApi {
        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        public delegate uint PSP_FILE_CALLBACK(IntPtr Context, uint Notification, IntPtr Param1, IntPtr Param2);

        [Flags]
        public enum DI_FLAGS : uint {
            DI_SHOWOEM = 0x00000001,
            DI_SHOWCOMPAT = 0x00000002,
            DI_SHOWCLASS = 0x00000004,
            DI_SHOWALL = 0x00000007,
            DI_NOVCP = 0x00000008,
            DI_DIDCOMPAT = 0x00000010,
            DI_DIDCLASS = 0x00000020,
            DI_AUTOASSIGNRES = 0x00000040,
            DI_NEEDRESTART = 0x00000080,
            DI_NEEDREBOOT = 0x00000100,
            DI_NOBROWSE = 0x00000200,
            DI_MULTMFGS = 0x00000400,
            DI_DISABLED = 0x00000800,
            DI_GENERALPAGE_ADDED = 0x00001000,
            DI_RESOURCEPAGE_ADDED = 0x00002000,
            DI_PROPERTIES_CHANGE = 0x00004000,
            DI_INF_IS_SORTED = 0x00008000,
            DI_ENUMSINGLEINF = 0x00010000,
            DI_DONOTCALLCONFIGMG = 0x00020000,
            DI_INSTALLDISABLED = 0x00040000,
            DI_COMPAT_FROM_CLASS = 0x00080000,
            DI_CLASSINSTALLPARAMS = 0x00100000,
            DI_NODI_DEFAULTACTION = 0x00200000,
            DI_QUIETINSTALL = 0x00800000,
            DI_NOFILECOPY = 0x01000000,
            DI_FORCECOPY = 0x02000000,
            DI_DRIVERPAGE_ADDED = 0x04000000,
            DI_USECI_SELECTSTRINGS = 0x08000000,
            DI_OVERRIDE_INFFLAGS = 0x10000000,
            [Obsolete]
            DI_PROPS_NOCHANGEUSAGE = 0x20000000,
            [Obsolete]
            DI_NOSELECTICONS = 0x40000000,
            DI_NOWRITE_IDS = 0x80000000,
        }

        [Flags]
        public enum DI_FLAGSEX : uint {
            DI_FLAGSEX_ALLOWEXCLUDEDDRVS = 0x00000800,
            DI_FLAGSEX_ALTPLATFORM_DRVSEARCH = 0x10000000,
            DI_FLAGSEX_ALWAYSWRITEIDS = 0x00000200,
            DI_FLAGSEX_APPENDDRIVERLIST = 0x00040000,
            DI_FLAGSEX_BACKUPONREPLACE = 0x00100000,
            DI_FLAGSEX_CI_FAILED = 0x00000004,
            DI_FLAGSEX_DEVICECHANGE = 0x00000100,
            DI_FLAGSEX_DIDCOMPATINFO = 0x00000020,
            DI_FLAGSEX_DIDINFOLIST = 0x00000010,
            DI_FLAGSEX_DRIVERLIST_FROM_URL = 0x00200000,
            DI_FLAGSEX_EXCLUDE_OLD_INET_DRIVERS = 0x00800000,
            DI_FLAGSEX_FILTERCLASSES = 0x00000040,
            DI_FLAGSEX_FILTERSIMILARDRIVERS = 0x02000000,
            DI_FLAGSEX_FINISHINSTALL_ACTION = 0x00000008,
            DI_FLAGSEX_IN_SYSTEM_SETUP = 0x00010000,
            DI_FLAGSEX_INET_DRIVER = 0x00020000,
            DI_FLAGSEX_INSTALLEDDRIVER = 0x04000000,
            DI_FLAGSEX_NO_CLASSLIST_NODE_MERGE = 0x08000000,
            DI_FLAGSEX_NO_DRVREG_MODIFY = 0x00008000,
            [Obsolete]
            DI_FLAGSEX_NOUIONQUERYREMOVE = 0x00001000,
            DI_FLAGSEX_POWERPAGE_ADDED = 0x01000000,
            DI_FLAGSEX_PREINSTALLBACKUP = 0x00080000,
            DI_FLAGSEX_PROPCHANGE_PENDING = 0x00000400,
            DI_FLAGSEX_RECURSIVESEARCH = 0x40000000,
            DI_FLAGSEX_RESERVED1 = 0x00400000,
            DI_FLAGSEX_RESERVED2 = 0x00000001,
            DI_FLAGSEX_RESERVED3 = 0x00000002,
            DI_FLAGSEX_RESERVED4 = 0x00004000,
            DI_FLAGSEX_RESTART_DEVICE_ONLY = 0x20000000,
            DI_FLAGSEX_SEARCH_PUBLISHED_INFS = 0x80000000,
            DI_FLAGSEX_SETFAILEDINSTALL = 0x00000080,
            DI_FLAGSEX_USECLASSFORCOMPAT = 0x00002000,
        }

        public enum SPDIT : uint {
            SPDIT_NODRIVER      = 0x00,
            SPDIT_CLASSDRIVER   = 0x01,
            SPDIT_COMPATDRIVER  = 0x02,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SP_DEVINFO_DATA {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct SP_DRVINFO_DATA_V2 {
            public uint cbSize;
            public SPDIT DriverType;
            public IntPtr Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string MfgName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ProviderName;
            public System.Runtime.InteropServices.ComTypes.FILETIME DriverDate;
            public ulong DriverVersion;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
        public struct SP_DEVINSTALL_PARAMS {
            public uint cbSize;
            public DI_FLAGS Flags;
            public DI_FLAGSEX FlagsEx;
            public IntPtr hwndParent;
            public PSP_FILE_CALLBACK InstallMsgHandler;
            public IntPtr InstallMsgHandlerContext;
            public IntPtr FileQueue;
            public IntPtr ClassInstallReserved;
            public uint Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string DriverPath;
        }

        public static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        public static extern IntPtr SetupDiCreateDeviceInfoList([In, Optional] IntPtr ClassGuid, [In, Optional] IntPtr hwndParent);

        [DllImport("newdev.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool InstallSelectedDriver(IntPtr HwndParent, IntPtr DeviceInfoSet, string Reserved, [MarshalAs(UnmanagedType.Bool)] bool Backup, out uint Reboot);

        [DllImport("Setupapi.dll", SetLastError = true, EntryPoint = "InstallHinfSection", CharSet = CharSet.Auto)]
        private static extern void InstallHinfSection([In] IntPtr hwnd, [In] IntPtr ModuleHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string CmdLineBuffer, int nCmdShow);

        public static void InstallDriverFromInf(string infPath) {
            if ( !File.Exists(infPath) )
                throw new ArgumentException($"Invalid file {infPath}!");
            InstallHinfSection(IntPtr.Zero, IntPtr.Zero, $"DefaultInstall 128 {infPath}", 0);
        }
    }
}
