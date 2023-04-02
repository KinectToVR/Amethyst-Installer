using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace amethyst_installer_gui.PInvoke {

    public static class DeviceClasses {
        public static readonly Guid AudioEndpoint       = new Guid("{c166523c-fe0c-4a94-a586-f1a80cfbbf3e}");
        public static readonly Guid Bluetooth           = new Guid("{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}");
        public static readonly Guid BTW                 = new Guid("{95c7a0a0-3094-11d7-a202-00508b9d7d5a}");
        public static readonly Guid Camera              = new Guid("{ca3e7ab9-b4c3-4ae6-8251-579ef933890f}");
        public static readonly Guid Computer            = new Guid("{4d36e966-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid DiskDrive           = new Guid("{4d36e967-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid Display             = new Guid("{4d36e968-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid Keyboards           = new Guid("{4d36e96b-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid KinectForWindows    = new Guid("{3a0339cd-b5f0-421c-8661-f243eef1528c}");
        public static readonly Guid Mouse               = new Guid("{4d36e96f-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid USB                 = new Guid("{36fc9e60-c465-11cf-8056-444553540000}");
        public static readonly Guid Media               = new Guid("{4d36e96c-e325-11ce-bfc1-08002be10318}");
        public static readonly Guid USBDevice           = new Guid("{88bae032-5a81-49f0-bc3d-a4ff138216d6}");
        public static readonly Guid Unknown             = new Guid("{00000000-0000-0000-0000-000000000000}");
    }

    public class DeviceTree : IDisposable {
        private IntPtr _machineHandle = IntPtr.Zero;
        private IntPtr _rootDeviceHandle = IntPtr.Zero;
        private DeviceNode _rootNode;

        // flat collection of all devices found
        private List<DeviceNode> _deviceNodes;

        public DeviceNode RootNode {
            get {
                return _rootNode;
            }
        }

        public List<DeviceNode> DeviceNodes {
            get {
                return _deviceNodes;
            }
        }

        public DeviceTree() {
            EnumerateDevices();
        }

        ~DeviceTree() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing )
                _deviceNodes.Clear();

            DisconnectFromMachine();
        }

        private void EnumerateDevices() {
            DisconnectFromMachine();

            // local machine assumed
            if ( CM_Connect_Machine(null, ref _machineHandle) != 0 ) {
                return;
            }

            try {
                CM_Locate_DevNode_Ex(ref _rootDeviceHandle, 0, 0, _machineHandle);

                // recursive enumeration
                _rootNode = new DeviceNode(_rootDeviceHandle, null, _machineHandle);
            } finally {
                DisconnectFromMachine();

                if ( _rootNode != null )
                    _deviceNodes = _rootNode.Flatten(node => node.Children).ToList();
            }
        }

        private void DisconnectFromMachine() {
            if ( _machineHandle != IntPtr.Zero ) {
                CM_Disconnect_Machine(_machineHandle);
                _machineHandle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Equivalent functionality to "Scan for Hardware Changes" in device manager
        /// </summary>
        public bool RescanDevices() {
            _rootDeviceHandle = IntPtr.Zero;
            if ( CM_Locate_DevNodeA(ref _rootDeviceHandle, null, CM_LOCATE_DEVNODE_NORMAL) != 0 )
                return false;
            if ( CM_Reenumerate_DevNode(_rootDeviceHandle, CM_REENUMERATE_NORMAL) != 0 )
                return false;

            return true;
        }

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Connect_Machine([MarshalAs(UnmanagedType.LPStr)] string uncServerName, ref IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Disconnect_Machine(IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Locate_DevNode_Ex(ref IntPtr deviceHandle, int deviceId, uint flags, IntPtr machineHandle);

        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Locate_DevNodeA(ref IntPtr pdnDevInst, string pDeviceID, uint ulFlags);

        [DllImport("setupapi.dll")]
        private static extern UInt32 CM_Reenumerate_DevNode_Ex(ref IntPtr deviceHandle, UInt32 flags, IntPtr machineHandle);
        [DllImport("CfgMgr32.dll", SetLastError = true)]
        private static extern int CM_Reenumerate_DevNode(IntPtr dnDevInst, uint ulFlags);

        const uint CM_LOCATE_DEVNODE_NORMAL             = 0x00000000;
        const uint CM_REENUMERATE_NORMAL                = 0x00000000;
        const uint CM_REENUMERATE_SYNCHRONOUS           = 0x00000001;
        const uint CM_REENUMERATE_RETRY_INSTALLATION    = 0x00000002;
        const uint CM_REENUMERATE_ASYNCHRONOUS          = 0x00000004;
    }

    public class DeviceNode : IDisposable {
        private readonly DeviceNode _parentDevice;
        private readonly List<DeviceNode> _children;
        private readonly IntPtr _deviceHandle;
        private readonly IntPtr _machineHandle;
        private readonly Dictionary<int, string> _deviceProperties;

        public DeviceNode ParentDevice {
            get {
                return _parentDevice;
            }
        }

        // public Dictionary<int, string> DeviceProperties {
        //     get {
        //         return _deviceProperties;
        //     }
        // }

        public List<DeviceNode> Children {
            get {
                return _children;
            }
        }

        public Guid ClassGuid {
            get {
                string buffer = GetProperty(DevRegProperty.ClassGuid);
                var guid = new Guid();

                if ( buffer.Length >= 32 ) {
                    try {
                        guid = new Guid(buffer);
                    } catch {
                        guid = new Guid();
                    }
                }

                return guid;
            }
        }

        public string Description {
            get {
                return GetProperty(DevRegProperty.DeviceDescription);
            }
        }

        public string FriendlyName {
            get {
                return GetProperty(DevRegProperty.FriendlyName);
            }
        }

        public string EnumeratorName {
            get {
                return GetProperty(DevRegProperty.EnumeratorName);
            }
        }

        public string LocationInfo {
            get {
                return GetProperty(DevRegProperty.LocationInfo);
            }
        }

        public DeviceNode(IntPtr deviceHandle, DeviceNode parentDevice) : this(deviceHandle, parentDevice, parentDevice._machineHandle) {
        }

        public DeviceNode(IntPtr deviceHandle, DeviceNode parentDevice, IntPtr machineHandle) {
            _deviceProperties = new Dictionary<int, string>();
            _children = new List<DeviceNode>();

            _deviceHandle = deviceHandle;
            _machineHandle = machineHandle;
            _parentDevice = parentDevice;

            // The function CM_Get_DevNode_Registry_Property_Ex is slow, and while not super slowon it's own, due to the sheer amount of devices
            // we have to poll and the number of unique properties (64) a device could own, the time executing this adds up fast.
            
            // We *could* enumerate all properties, but we aren't even going to use most of them, so instead we poll on demand.
            // This brings polling the entire device tree from ~470ms down to ~17ms

            // EnumerateDeviceProperties();
            EnumerateChildren();
        }

        public string GetProperty(DevRegProperty devRegProperty) {
            return GetProperty(( int ) devRegProperty);
        }

        private string GetProperty(int index) {
            string buffer;
            var result = _deviceProperties.TryGetValue(index, out buffer);
            if (!result) {
                EnumerateDeviceProperty(index);
                return _deviceProperties[index];
            }
            return buffer;
            // return result ? buffer : EnumerateDeviceProperty(index);
            // return result ? buffer : string.Empty;
        }

        public string GetInstanceId() {
            StringBuilder instanceIdBuffer = new StringBuilder(256);
            var wasSuccessful = CM_Get_Device_ID(_deviceHandle, instanceIdBuffer, 256);
            if (wasSuccessful != 0) {
                Logger.Fatal("Failed to fetch device instance ID!");
                return string.Empty;
            }
            return instanceIdBuffer.ToString();
        }

        public bool UninstallDevice() {
            var wasRemoved = CM_Query_And_Remove_SubTree_Ex(_deviceHandle, out _, null, 0, 0, _machineHandle);
            if ( wasRemoved != 0 )
                Logger.Fatal("Failed to remove device! Cannot uninstall!");

            var result = CM_Uninstall_DevNode(_deviceHandle, 0);
            if ( result != 0 )
                Logger.Fatal("Failed to uninstall device!");

            return wasRemoved == 0 && result == 0;
        }

        private void EnumerateDeviceProperties() {
            for ( var index = 0; index < 64; index++ ) {
                uint bufsize = 2048;
                IntPtr buffer = Marshal.AllocHGlobal((int)bufsize);

                var result = CM_Get_DevNode_Registry_Property_Ex(_deviceHandle, index, IntPtr.Zero, buffer, ref bufsize, 0, _machineHandle);
                var propertyString = result == 0 ? Marshal.PtrToStringAnsi(buffer) : string.Empty;

                _deviceProperties.Add(index, propertyString);

                Marshal.FreeHGlobal(buffer);
            }
        }

        private void EnumerateDeviceProperty(int index) {
            uint bufsize = 2048;
            IntPtr buffer = Marshal.AllocHGlobal((int)bufsize);

            var result = CM_Get_DevNode_Registry_Property_Ex(_deviceHandle, index, IntPtr.Zero, buffer, ref bufsize, 0, _machineHandle);
            var propertyString = result == 0 ? Marshal.PtrToStringAnsi(buffer) : string.Empty;

            _deviceProperties.Add(index, propertyString);

            Marshal.FreeHGlobal(buffer);
        }

        public DeviceNodeStatus GetStatus(out DeviceNodeProblemCode problemCode) {
            uint status;
            uint code;
            if (CM_Get_DevNode_Status(out status, out code, _deviceHandle, 0) == 0) {
                problemCode = ( DeviceNodeProblemCode) code;
                return ( DeviceNodeStatus ) status;
            }
            problemCode = ( DeviceNodeProblemCode ) uint.MaxValue;
            return DeviceNodeStatus.PrivateProblem;
        }

        private void EnumerateChildren() {
            IntPtr ptrFirstChild = IntPtr.Zero;

            if ( CM_Get_Child_Ex(ref ptrFirstChild, _deviceHandle, 0, _machineHandle) != 0 )
                return;

            var ptrChild = ptrFirstChild;
            var ptrSibling = IntPtr.Zero;

            do {
                var childDevice = new DeviceNode(ptrChild, this);
                _children.Add(childDevice);

                if ( CM_Get_Sibling_Ex(ref ptrSibling, ptrChild, 0, _machineHandle) != 0 )
                    break;

                ptrChild = ptrSibling;
            }
            while ( true );
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if ( disposing )
                _deviceProperties.Clear();
        }

        public override string ToString() {
            return $"{{ {( FriendlyName.Length > 0 ? FriendlyName : Description )} : {EnumeratorName} }}";
        }

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_Child_Ex(ref IntPtr childDeviceHandle, IntPtr parentDeviceHandle, uint flags, IntPtr machineHandle);
        [DllImport("setupapi.dll", SetLastError = true)]
        private static extern int CM_Get_Device_ID(IntPtr deviceHandle, StringBuilder buffer, int bufferLen, int flags = 0);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_Sibling_Ex(ref IntPtr siblingDeviceHandle, IntPtr deviceHandle, uint flags, IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_DevNode_Registry_Property_Ex(IntPtr deviceHandle, int property, IntPtr regDataType, IntPtr outBuffer, ref uint size, int flags, IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Uninstall_DevNode(IntPtr deviceHandle, int flags);
        [DllImport("cfgmgr32.dll", SetLastError = true)]
        private static extern int CM_Get_DevNode_Status(out uint status, out uint probNum, IntPtr deviceHandle, int flags);

        [DllImport("Cfgmgr32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern int CM_Query_And_Remove_SubTree_Ex(IntPtr deviceInstance, out int vetoType, StringBuilder vetoName, uint vetoNameLength, int flags, IntPtr machineHandle);
    }

    public enum DevRegProperty : uint {
        DeviceDescription = 1,
        HardwareId = 2,
        CompatibleIds = 3,
        Unused0 = 4,
        Service = 5,
        Unused1 = 6,
        Unused2 = 7,
        Class = 8,
        ClassGuid = 9,
        Driver = 0x0a,
        ConfigFlags = 0x0b,
        Mfg = 0x0c,
        FriendlyName = 0x0d,
        LocationInfo = 0x0e,
        PhysicalDeviceObjectName = 0x0f,
        Capabilities = 0x10,
        UiNumber = 0x11,
        UpperFilters = 0x12,
        LowerFilters = 0x13,
        BusTypeGuid = 0x014,
        LegacyBusType = 0x15,
        BusNumber = 0x16,
        EnumeratorName = 0x17,
    }

    [Flags]
    public enum DeviceNodeStatus : UInt64 {
        RootEnumerated          = (0x00000001), // Was enumerated by ROOT
        DriverLoaded            = (0x00000002), // Has Register_Device_Driver
        EnumLoaded              = (0x00000004), // Has Register_Enumerator
        Started                 = (0x00000008), // Is currently configured
        Manual                  = (0x00000010), // Manually installed
        NeedToEnum              = (0x00000020), // May need reenumeration
        NotFirstTime            = (0x00000040), // Has received a config
        HardwareEnum            = (0x00000080), // Enum generates hardware ID
        Liar                    = (0x00000100), // Lied about can reconfig once
        HasMark                 = (0x00000200), // Not CM_Create_DevInst lately
        HasProblem              = (0x00000400), // Need device installer
        Filtered                = (0x00000800), // Is filtered
        Moved                   = (0x00001000), // Has been moved
        Disableable             = (0x00002000), // Can be disabled
        Removable               = (0x00004000), // Can be removed
        PrivateProblem          = (0x00008000), // Has a private problem
        MultiFunctionalParent   = (0x00010000), // Multi function parent
        MultiFunctionalChild    = (0x00020000), // Multi function child
        WillBeRemoved           = (0x00040000), // DevInst is being removed

        //
        // Windows 4 OPK2 Flags
        //
        NotFirstTimeEnumerating = 0x00080000,  // S: Has received a config enumerate
        StoppedFreeResources    = 0x00100000,  // S: When child is stopped, free resources
        RebalanceCandidate      = 0x00200000,  // S: Don't skip during rebalance
        BadPartial              = 0x00400000,  // S: This devnode's log_confs do not have same resources
        NTEnumerator            = 0x00800000,  // S: This devnode's is an NT enumerator
        NTDriver                = 0x01000000,  // S: This devnode's is an NT driver
        //
        // Windows 4.1 Flags
        //
        NeedsLocking            = 0x02000000,  // S: Devnode need lock resume processing
        ArmWakeup               = 0x04000000,  // S: Devnode can be the wakeup device
        APMAwareEnumerator      = 0x08000000,  // S: APM aware enumerator
        APMAwareDriver          = 0x10000000,  // S: APM aware driver
        SilentInstall           = 0x20000000,  // S: Silent install
        HideInDeviceManager     = 0x40000000,  // S: No show in device manager
        BootLogProblem          = 0x80000000,  // S: Had a problem during preassignment of boot log conf

        //
        // Windows NT Flags
        //
        // These are overloaded on top of unused Win 9X flags
        //

        NeedRestart                     = Liar,                     // System needs to be restarted for this Devnode to work properly

        DriverBlocked                   = NotFirstTime,             // One or more drivers are blocked from loading for this Devnode
        LegacyDriver                    = Moved,                    // This device is using a legacy driver
        ChildWithInvalidID              = HasMark,                  // One or more children have invalid ID(s)

        DeviceDisconnected              = NeedsLocking,             // The function driver for a device reported that the device is not connected.  Typically this means a wireless device is out of range.

        QueryRemovePending              = MultiFunctionalParent,    // Device is part of a set of related devices collectively pending query-removal
        QueryRemoveActive               = MultiFunctionalChild,     // Device is actively engaged in a query-remove IRP

    }

    //
    // DevInst problem values, returned by call to CM_Get_DevInst_Status
    //
    public enum DeviceNodeProblemCode : uint {
        NotConfigured               = (0x00000001),   // no config for device
        DevloaderFailed             = (0x00000002),   // service load failed
        OutOfMemory                 = (0x00000003),   // out of memory
        EntryIsWrongType            = (0x00000004),   //
        LackedArbitrator            = (0x00000005),   //
        BootConfigConflict          = (0x00000006),   // boot config conflict
        FailedFilter                = (0x00000007),   //
        DevloaderNotFound           = (0x00000008),   // Devloader not found
        InvalidData                 = (0x00000009),   // Invalid ID
        FailedStart                 = (0x0000000A),   //
        Liar                        = (0x0000000B),   //
        NormalConflict              = (0x0000000C),   // config conflict
        NotVerified                 = (0x0000000D),   //
        NeedRestart                 = (0x0000000E),   // requires restart
        Reenumeration               = (0x0000000F),   //
        PartialLogConf              = (0x00000010),   //
        UnknownResource             = (0x00000011),   // unknown res type
        Reinstall                   = (0x00000012),   //
        Registry                    = (0x00000013),   //
        VXDLDR                      = (0x00000014),   // WINDOWS 95 ONLY
        WillBeRemoved               = (0x00000015),   // devinst will remove
        Disabled                    = (0x00000016),   // devinst is disabled
        DevloaderNotReady           = (0x00000017),   // Devloader not ready
        DeviceNotThere              = (0x00000018),   // device doesn't exist
        Moved                       = (0x00000019),   //
        TooEarly                    = (0x0000001A),   //
        NoValidLogConf              = (0x0000001B),   // no valid log config
        FailedInstall               = (0x0000001C),   // install failed
        HardwareDisabled            = (0x0000001D),   // device disabled
        CantShareIrq                = (0x0000001E),   // can't share IRQ
        FailedAdd                   = (0x0000001F),   // driver failed add
        DisabledService             = (0x00000020),   // service's Start = 4
        TranslationFailed           = (0x00000021),   // resource translation failed
        NoSoftconfig                = (0x00000022),   // no soft config
        BiosTable                   = (0x00000023),   // device missing in BIOS table
        IrqTranslationFailed        = (0x00000024),   // IRQ translator failed
        FailedDriverEntry           = (0x00000025),   // DriverEntry() failed.
        DriverFailedPriorUnload     = (0x00000026),   // Driver should have unloaded.
        DriverFailedLoad            = (0x00000027),   // Driver load unsuccessful.
        DriverServiceKeyInvalid     = (0x00000028),   // Error accessing driver's service key
        LegacyServiceNoDevices      = (0x00000029),   // Loaded legacy service created no devices
        DuplicateDevice             = (0x0000002A),   // Two devices were discovered with the same name
        FailedPostStart             = (0x0000002B),   // The drivers set the device state to failed
        Halted                      = (0x0000002C),   // This device was failed post start via usermode
        Phantom                     = (0x0000002D),   // The devinst currently exists only in the registry
        SystemShutdown              = (0x0000002E),   // The system is shutting down
        HeldForEject                = (0x0000002F),   // The device is offline awaiting removal
        DriverBlocked               = (0x00000030),   // One or more drivers is blocked from loading
        RegistryTooLarge            = (0x00000031),   // System hive has grown too large
        SetPropertiesFailed         = (0x00000032),   // Failed to apply one or more registry properties  
        WaitingOnDependency         = (0x00000033),   // Device is stalled waiting on a dependency to start
        UnsignedDriver              = (0x00000034),   // Failed load driver due to unsigned image.
        UsedByDebugger              = (0x00000035),   // Device is being used by kernel debugger
        DeviceReset                 = (0x00000036),   // Device is being reset
        ConsoleLocked               = (0x00000037),   // Device is blocked while console is locked
        NeedClassConfig             = (0x00000038),   // Device needs extended class configuration to start
    }
}