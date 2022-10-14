using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {
    public static class SetupApi {

        #region Private API

        private const int ERROR_NO_MORE_ITEMS = 259;

        [UnmanagedFunctionPointer(CallingConvention.Winapi, CharSet = CharSet.Unicode)]
        private delegate uint PSP_FILE_CALLBACK(IntPtr Context, uint Notification, IntPtr Param1, IntPtr Param2);

        private enum DI_FUNCTION : uint {
            DIF_SELECTDEVICE = 0x00000001,
            DIF_INSTALLDEVICE = 0x00000002,
            DIF_ASSIGNRESOURCES = 0x00000003,
            DIF_PROPERTIES = 0x00000004,
            DIF_REMOVE = 0x00000005,
            DIF_FIRSTTIMESETUP = 0x00000006,
            DIF_FOUNDDEVICE = 0x00000007,
            DIF_SELECTCLASSDRIVERS = 0x00000008,
            DIF_VALIDATECLASSDRIVERS = 0x00000009,
            DIF_INSTALLCLASSDRIVERS = 0x0000000A,
            DIF_CALCDISKSPACE = 0x0000000B,
            DIF_DESTROYPRIVATEDATA = 0x0000000C,
            DIF_VALIDATEDRIVER = 0x0000000D,
            DIF_DETECT = 0x0000000F,
            DIF_INSTALLWIZARD = 0x00000010,
            DIF_DESTROYWIZARDDATA = 0x00000011,
            DIF_PROPERTYCHANGE = 0x00000012,
            DIF_ENABLECLASS = 0x00000013,
            DIF_DETECTVERIFY = 0x00000014,
            DIF_INSTALLDEVICEFILES = 0x00000015,
            DIF_UNREMOVE = 0x00000016,
            DIF_SELECTBESTCOMPATDRV = 0x00000017,
            DIF_ALLOW_INSTALL = 0x00000018,
            DIF_REGISTERDEVICE = 0x00000019,
            DIF_NEWDEVICEWIZARD_PRESELECT = 0x0000001A,
            DIF_NEWDEVICEWIZARD_SELECT = 0x0000001B,
            DIF_NEWDEVICEWIZARD_PREANALYZE = 0x0000001C,
            DIF_NEWDEVICEWIZARD_POSTANALYZE = 0x0000001D,
            DIF_NEWDEVICEWIZARD_FINISHINSTALL = 0x0000001E,
            DIF_UNUSED1 = 0x0000001F,
            DIF_INSTALLINTERFACES = 0x00000020,
            DIF_DETECTCANCEL = 0x00000021,
            DIF_REGISTER_COINSTALLERS = 0x00000022,
            DIF_ADDPROPERTYPAGE_ADVANCED = 0x00000023,
            DIF_ADDPROPERTYPAGE_BASIC = 0x00000024,
            DIF_RESERVED1 = 0x00000025,
            DIF_TROUBLESHOOTER = 0x00000026,
            DIF_POWERMESSAGEWAKE = 0x00000027,
            DIF_ADDREMOTEPROPERTYPAGE_ADVANCED = 0x00000028,
            DIF_UPDATEDRIVER_UI = 0x00000029,
            DIF_FINISHINSTALL_ACTION = 0x0000002A,
            DIF_RESERVED2 = 0x00000030,
            [Obsolete]
            DIF_MOVEDEVICE = 0x0000000E,
        }

        [Flags]
        private enum DI_FLAGS : uint {
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
        private enum DI_FLAGSEX : uint {
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

        private enum SPDIT : uint {
            SPDIT_NODRIVER      = 0x00,
            SPDIT_CLASSDRIVER   = 0x01,
            SPDIT_COMPATDRIVER  = 0x02,
        }

        [Flags]
        private enum DIOD : uint {
            DIOD_INHERIT_CLASSDRVS = 0x00000002,
            DIOD_CANCEL_REMOVE = 0x00000004
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SP_DEVINFO_DATA {
            public int cbSize;
            public Guid ClassGuid;
            public uint DevInst;
            public IntPtr Reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = 4)]
        private struct SP_DRVINFO_DATA_V2 {
            public int cbSize;
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DRVINFO_DATA {
            public int cbSize;
            public SPDIT DriverType;
            public IntPtr Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string Description;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string MfgName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string ProviderName;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
        private struct SP_DEVINSTALL_PARAMS {
            public int cbSize;
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

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SP_DRVINFO_DETAIL_DATA {
            public int cbSize;
            public System.Runtime.InteropServices.ComTypes.FILETIME InfDate;
            public uint CompatIDsOffset;
            public uint CompatIDsLength;
            public IntPtr Reserved;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string SectionName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string InfFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
            public string DrvDescription;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1)]
            public string HardwareID;
        }

        private static readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr SetupDiCreateDeviceInfoList([In, Optional] IntPtr ClassGuid, [In, Optional] IntPtr hwndParent);


        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiDestroyDeviceInfoList(IntPtr DeviceInfoSet);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiOpenDeviceInfo(IntPtr DeviceInfoSet, [Optional, MarshalAs(UnmanagedType.LPTStr)] string DeviceInstanceId, [In, Optional] IntPtr hwndParent, DIOD OpenFlags, ref SP_DEVINFO_DATA DeviceInfoData);


        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiSetSelectedDevice(IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetDeviceInstallParams(IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData, ref SP_DEVINSTALL_PARAMS DeviceInstallParams);


        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiSetDeviceInstallParams(IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData, in SP_DEVINSTALL_PARAMS DeviceInstallParams);

        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiBuildDriverInfoList(IntPtr DeviceInfoSet, [In, Optional] SP_DEVINFO_DATA DeviceInfoData, SPDIT DriverType);

        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiBuildDriverInfoList(IntPtr DeviceInfoSet, [In, Optional] IntPtr DeviceInfoData, SPDIT DriverType);

        [DllImport("SetupAPI.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiCallClassInstaller(DI_FUNCTION InstallFunction, IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetSelectedDriver(IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData, ref SP_DRVINFO_DATA_V2 DriverInfoData);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiSetSelectedDriver(IntPtr DeviceInfoSet, ref SP_DEVINFO_DATA DeviceInfoData, ref SP_DRVINFO_DATA_V2 DriverInfoData);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiEnumDriverInfo(IntPtr DeviceInfoSet, [In, Optional] IntPtr DeviceInfoData, SPDIT DriverType, int MemberIndex, ref SP_DRVINFO_DATA_V2 DriverInfoData);
        
        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiEnumDriverInfo(IntPtr DeviceInfoSet, [In, Optional] SP_DEVINFO_DATA DeviceInfoData, SPDIT DriverType, int MemberIndex, ref SP_DRVINFO_DATA_V2 DriverInfoData);


        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetDriverInfoDetail(IntPtr DeviceInfoSet, in SP_DEVINFO_DATA DeviceInfoData, in SP_DRVINFO_DATA_V2 DriverInfoData, ref SP_DRVINFO_DETAIL_DATA DriverInfoDetailData, int DriverInfoDetailDataSize, out int RequiredSize);

        [DllImport("SetupAPI.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetupDiGetDriverInfoDetail(IntPtr DeviceInfoSet, in IntPtr DeviceInfoData, in IntPtr DriverInfoData, ref SP_DRVINFO_DETAIL_DATA DriverInfoDetailData, int DriverInfoDetailDataSize, out int RequiredSize);


        [DllImport("newdev.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InstallSelectedDriver(IntPtr HwndParent, IntPtr DeviceInfoSet, IntPtr Reserved, [MarshalAs(UnmanagedType.Bool)] bool Backup, out uint Reboot);

        [DllImport("Setupapi.dll", SetLastError = true, EntryPoint = "InstallHinfSection", CharSet = CharSet.Auto)]
        private static extern void InstallHinfSection([In] IntPtr hwnd, [In] IntPtr ModuleHandle, [In, MarshalAs(UnmanagedType.LPWStr)] string CmdLineBuffer, int nCmdShow);

        #endregion

        #region Public API

        public static void InstallDriverFromInf(string infPath) {
            if ( !File.Exists(infPath) )
                throw new ArgumentException($"Invalid file {infPath}!");
            InstallHinfSection(IntPtr.Zero, IntPtr.Zero, $"DefaultInstall 128 {infPath}", 0);
        }


        // Assigns a driver to a device
        public static bool AssignDriverToDeviceId(string deviceInstanceId, string infPath) {

            bool success = false;

            // Based on sample at https://web.archive.org/web/20221013072029/https://www.betaarchive.com/wiki/index.php/Microsoft_KB_Archive/889763

            IntPtr DeviceInfoSet = INVALID_HANDLE_VALUE;
            SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);
            SP_DRVINFO_DATA_V2 DriverInfoData = new SP_DRVINFO_DATA_V2();
            DriverInfoData.cbSize = Marshal.SizeOf(DriverInfoData);
            SP_DEVINSTALL_PARAMS DeviceInstallParams = new SP_DEVINSTALL_PARAMS();
            DeviceInstallParams.cbSize = Marshal.SizeOf(DeviceInstallParams);

            try {

                // Load devices into memory
                DeviceInfoSet = SetupDiCreateDeviceInfoList(IntPtr.Zero, IntPtr.Zero);
                if ( DeviceInfoSet == INVALID_HANDLE_VALUE ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Register the device we want in the driver list
                if ( !SetupDiOpenDeviceInfo(DeviceInfoSet, deviceInstanceId, IntPtr.Zero, 0, ref DeviceInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Select our device
                if ( !SetupDiSetSelectedDevice(DeviceInfoSet, DeviceInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Fetch a SP_DRVINFO_DATA structure to install on the device
                if ( !SetupDiGetDeviceInstallParams(DeviceInfoSet, DeviceInfoData, ref DeviceInstallParams) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Only construct the available driver list from the inf file we provided
                DeviceInstallParams.Flags |= DI_FLAGS.DI_ENUMSINGLEINF;
                DeviceInstallParams.DriverPath = infPath;

                // Set this flag so that we can select the driver even if it's marked as ExcludeFromSelect
                DeviceInstallParams.FlagsEx |= DI_FLAGSEX.DI_FLAGSEX_ALLOWEXCLUDEDDRVS;

                if ( !SetupDiSetDeviceInstallParams(DeviceInfoSet, DeviceInfoData, DeviceInstallParams) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Build a driver information list from the specified INF, and a compatible driver list
                if ( !SetupDiBuildDriverInfoList(DeviceInfoSet, DeviceInfoData, SPDIT.SPDIT_COMPATDRIVER) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Pick the best driver in the list of drivers that was built.
                if ( !SetupDiCallClassInstaller(DI_FUNCTION.DIF_SELECTBESTCOMPATDRV, DeviceInfoSet, DeviceInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Get the selected driver node
                if ( !SetupDiGetSelectedDriver(DeviceInfoSet, DeviceInfoData, ref DriverInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Install the selected driver on the selected device
                success = InstallSelectedDriver(IntPtr.Zero, DeviceInfoSet, IntPtr.Zero, true, out uint Reboot);

                if ( ( Reboot & ( uint ) ( DI_FLAGS.DI_NEEDREBOOT | DI_FLAGS.DI_NEEDRESTART ) ) == ( uint ) ( DI_FLAGS.DI_NEEDREBOOT | DI_FLAGS.DI_NEEDRESTART ) ) {
                    // A reboot is required.
                    Logger.Error("A reboot is required!");
                }

            } catch ( Win32Exception e ) {
                Logger.Fatal(Util.FormatException(e));
                success = false;
            } finally {
                // Cleanup
                if ( DeviceInfoSet != INVALID_HANDLE_VALUE ) {
                    SetupDiDestroyDeviceInfoList(DeviceInfoSet);
                }
            }

            return success;
        }
        
        // Assigns an existing driver to a device using an inf file
        public static bool AssignExistingDriverViaInfToDeviceId(string deviceInstanceId, string infFile, string manufacturer, string description) {

            bool success = false;

            // Based on sample at https://web.archive.org/web/20221013072029/https://www.betaarchive.com/wiki/index.php/Microsoft_KB_Archive/889763

            IntPtr DeviceInfoSet = INVALID_HANDLE_VALUE;
            SP_DEVINFO_DATA DeviceInfoData = new SP_DEVINFO_DATA();
            DeviceInfoData.cbSize = Marshal.SizeOf(DeviceInfoData);
            SP_DRVINFO_DATA_V2 DriverInfoData = new SP_DRVINFO_DATA_V2();
            DriverInfoData.cbSize = Marshal.SizeOf(DriverInfoData);
            SP_DEVINSTALL_PARAMS DeviceInstallParams = new SP_DEVINSTALL_PARAMS();
            DeviceInstallParams.cbSize = Marshal.SizeOf(DeviceInstallParams);
            SP_DRVINFO_DETAIL_DATA DriverInfoDetail = new SP_DRVINFO_DETAIL_DATA();
            DriverInfoDetail.cbSize = Marshal.SizeOf(DriverInfoDetail);

            try {

                // Load devices into memory
                DeviceInfoSet = SetupDiCreateDeviceInfoList(IntPtr.Zero, IntPtr.Zero);
                if ( DeviceInfoSet == INVALID_HANDLE_VALUE ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Register the device we want in the driver list
                if ( !SetupDiOpenDeviceInfo(DeviceInfoSet, deviceInstanceId, IntPtr.Zero, 0, ref DeviceInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Select our device
                if ( !SetupDiSetSelectedDevice(DeviceInfoSet, DeviceInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Fetch a SP_DRVINFO_DATA structure to install on the device
                if ( !SetupDiGetDeviceInstallParams(DeviceInfoSet, DeviceInfoData, ref DeviceInstallParams) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Allow excluded drivers
                DeviceInstallParams.FlagsEx |= DI_FLAGSEX.DI_FLAGSEX_ALLOWEXCLUDEDDRVS;

                if ( !SetupDiSetDeviceInstallParams(DeviceInfoSet, DeviceInfoData, DeviceInstallParams) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Build a driver information list from the specified INF, and a compatible driver list
                if ( !SetupDiBuildDriverInfoList(DeviceInfoSet, DeviceInfoData, SPDIT.SPDIT_CLASSDRIVER) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                int index = 0;
                int requiredSize = Marshal.SizeOf(DriverInfoDetail);
                bool state;
                while ( state = SetupDiEnumDriverInfo(DeviceInfoSet, DeviceInfoData, SPDIT.SPDIT_CLASSDRIVER, index, ref DriverInfoData) ) {

                    success = true;

                    // get useful driver information
                    if ( SetupDiGetDriverInfoDetail(DeviceInfoSet, DeviceInfoData, DriverInfoData, ref DriverInfoDetail, Marshal.SizeOf(DriverInfoDetail), out requiredSize) || Marshal.GetLastWin32Error() == 122 ) {

                        if ( DriverInfoData.MfgName == manufacturer &&          // If the Manufacturer is (Generic USB Audio)
                            DriverInfoDetail.InfFileName.Contains(infFile) &&   // If the driver INF file is wdma_usb.inf
                            DriverInfoData.Description == description ) {       // If the driver display name is USB Audio Device
                            break; // Exit loop ; we found the driver
                        }
                    }
                    index++;
                }
                if ( !state ) {
                    // shit.
                    int err = Marshal.GetLastWin32Error();
                    if ( err != ERROR_NO_MORE_ITEMS ) {
                        throw new Win32Exception(err);
                    } else {
                        Logger.Warn("Got WIN32_ERROR_259:: ERROR_NO_MORE_ITEMS");
                    }
                }

                // Select the driver
                if ( !SetupDiSetSelectedDriver(DeviceInfoSet, ref DeviceInfoData, ref DriverInfoData) ) {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                // Install the selected driver on the selected device
                success = InstallSelectedDriver(IntPtr.Zero, DeviceInfoSet, IntPtr.Zero, true, out uint Reboot);

                if ( ( Reboot & ( uint ) ( DI_FLAGS.DI_NEEDREBOOT | DI_FLAGS.DI_NEEDRESTART ) ) == ( uint ) ( DI_FLAGS.DI_NEEDREBOOT | DI_FLAGS.DI_NEEDRESTART ) ) {
                    // A reboot is required.
                    Logger.Error("A reboot is required!");
                }

            } catch ( Win32Exception e ) {
                Logger.Fatal(Util.FormatException(e));
                success = false;
            } finally {
                // Cleanup
                if ( DeviceInfoSet != INVALID_HANDLE_VALUE ) {
                    SetupDiDestroyDeviceInfoList(DeviceInfoSet);
                }
            }

            return success;
        }

        #endregion
    }
}
