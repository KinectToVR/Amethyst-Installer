using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.PInvoke {
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

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Connect_Machine([MarshalAs(UnmanagedType.LPStr)] string uncServerName, ref IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Disconnect_Machine(IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Locate_DevNode_Ex(ref IntPtr deviceHandle, int deviceId, uint flags, IntPtr machineHandle);
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

        public Dictionary<int, string> DeviceProperties {
            get {
                return _deviceProperties;
            }
        }

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

            EnumerateDeviceProperties();
            EnumerateChildren();
        }

        private string GetProperty(DevRegProperty devRegProperty) {
            return GetProperty(( int ) devRegProperty);
        }

        private string GetProperty(int index) {
            string buffer;
            var result = _deviceProperties.TryGetValue(index, out buffer);
            return result ? buffer : string.Empty;
        }

        public void UninstallDevice() {
            var result = CM_Uninstall_DevNode(_deviceHandle, 0);
            if ( result != 0 )
                Console.Error.WriteLine("Failed to uninstall device!");
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

        private enum DevRegProperty : uint {
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

        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_Child_Ex(ref IntPtr childDeviceHandle, IntPtr parentDeviceHandle, uint flags, IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_Sibling_Ex(ref IntPtr siblingDeviceHandle, IntPtr deviceHandle, uint flags, IntPtr machineHandle);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Get_DevNode_Registry_Property_Ex(IntPtr deviceHandle, int property, IntPtr regDataType, IntPtr outBuffer, ref uint size, int flags, IntPtr machineHandler);
        [DllImport("cfgmgr32.dll")]
        private static extern int CM_Uninstall_DevNode(IntPtr deviceHandle, int flags);
    }
}