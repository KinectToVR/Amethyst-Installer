/*
  LICENSE
  -------
  Copyright (C) 2007 Ray Molenkamp
  This source code is provided 'as-is', without any express or implied
  warranty.  In no event will the authors be held liable for any damages
  arising from the use of this source code or the software it produces.
  Permission is granted to anyone to use this source code for any purpose,
  including commercial applications, and to alter it and redistribute it
  freely, subject to the following restrictions:
  1. The origin of this source code must not be misrepresented; you must not
     claim that you wrote the original source code.  If you use this source code
     in a product, an acknowledgment in the product documentation would be
     appreciated but is not required.
  2. Altered source versions must be plainly marked as such, and must not be
     misrepresented as being the original source code.
  3. This notice may not be removed or altered from any source distribution.
*/
// modified for NAudio
// milligan22963 - updated to include audio session manager

using System;
using NAudio.CoreAudioApi.Interfaces;
using System.Runtime.InteropServices;

namespace NAudio.CoreAudioApi {
    /// <summary>
    /// MM Device
    /// </summary>
    public class MMDevice : IDisposable {
        #region Variables
        private readonly IMMDevice deviceInterface;
        private PropertyStore propertyStore;
        #endregion

        #region Guids
        // ReSharper disable InconsistentNaming
        private static Guid IID_IAudioMeterInformation = new Guid("C02216F6-8C67-4B5B-9D00-D008E73E0064");
        private static Guid IID_IAudioEndpointVolume = new Guid("5CDF2C82-841E-4546-9722-0CF74078229A");
        private static Guid IID_IAudioClient = new Guid("1CB9AD4C-DBFA-4c32-B178-C2F568A703B2");
        private static Guid IDD_IAudioSessionManager = new Guid("BFA971F1-4D5E-40BB-935E-967039BFBEE4");
        private static Guid IDD_IDeviceTopology = new Guid("2A07407E-6497-4A18-9787-32F79BD0D98F");
        // ReSharper restore InconsistentNaming
        #endregion

        #region Init
        /// <summary>
        /// Initializes the device's property store.
        /// </summary>
        /// <param name="stgmAccess">The storage-access mode to open store for.</param>
        /// <remarks>Administrative client is required for Write and ReadWrite modes.</remarks>
        public void GetPropertyInformation(StorageAccessMode stgmAccess = StorageAccessMode.Read) {
            Marshal.ThrowExceptionForHR(deviceInterface.OpenPropertyStore(stgmAccess, out var propstore));
            propertyStore = new PropertyStore(propstore);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Properties
        /// </summary>
        public PropertyStore Properties {
            get {
                if ( propertyStore == null )
                    GetPropertyInformation();
                return propertyStore;
            }
        }

        /// <summary>
        /// Friendly name for the endpoint
        /// </summary>
        public string FriendlyName {
            get {
                if ( propertyStore == null ) {
                    GetPropertyInformation();
                }
                if ( propertyStore.Contains(PropertyKeys.PKEY_Device_FriendlyName) ) {
                    return ( string ) propertyStore[PropertyKeys.PKEY_Device_FriendlyName].Value;
                } else
                    return "Unknown";
            }
        }

        /// <summary>
        /// Friendly name of device
        /// </summary>
        public string DeviceFriendlyName {
            get {
                if ( propertyStore == null ) {
                    GetPropertyInformation();
                }
                if ( propertyStore.Contains(PropertyKeys.PKEY_DeviceInterface_FriendlyName) ) {
                    return ( string ) propertyStore[PropertyKeys.PKEY_DeviceInterface_FriendlyName].Value;
                } else {
                    return "Unknown";
                }
            }
        }

        /// <summary>
        /// Icon path of device
        /// </summary>
        public string IconPath {
            get {
                if ( propertyStore == null ) {
                    GetPropertyInformation();
                }
                if ( propertyStore.Contains(PropertyKeys.PKEY_Device_IconPath) ) {
                    return ( string ) propertyStore[PropertyKeys.PKEY_Device_IconPath].Value;
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Device Instance Id of Device
        /// </summary>
        public string InstanceId {
            get {
                if ( propertyStore == null ) {
                    GetPropertyInformation();
                }
                if ( propertyStore.Contains(PropertyKeys.PKEY_Device_InstanceId) ) {
                    return ( string ) propertyStore[PropertyKeys.PKEY_Device_InstanceId].Value;
                }

                return "Unknown";
            }
        }

        /// <summary>
        /// Device ID
        /// </summary>
        public string ID {
            get {
                Marshal.ThrowExceptionForHR(deviceInterface.GetId(out var result));
                return result;
            }
        }

        /// <summary>
        /// Data Flow
        /// </summary>
        public DataFlow DataFlow {
            get {
                var ep = deviceInterface as IMMEndpoint;
                ep.GetDataFlow(out var result);
                return result;
            }
        }

        /// <summary>
        /// Device State
        /// </summary>
        public DeviceState State {
            get {
                Marshal.ThrowExceptionForHR(deviceInterface.GetState(out var result));
                return result;
            }
        }

        #endregion

        #region Constructor
        internal MMDevice(IMMDevice realDevice) {
            deviceInterface = realDevice;
        }
        #endregion

        /// <summary>
        /// To string
        /// </summary>
        public override string ToString() {
            return FriendlyName;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose() {
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~MMDevice() {
            Dispose();
        }
    }
}
