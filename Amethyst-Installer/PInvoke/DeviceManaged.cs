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
}