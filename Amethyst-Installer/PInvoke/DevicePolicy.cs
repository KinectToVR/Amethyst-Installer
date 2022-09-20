using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.Wave;
using System;
using System.Runtime.InteropServices;

namespace amethyst_installer_gui.PInvoke {


    [ComImport]
    [Guid("f8679f50-850a-41cf-9c72-430f290290c8")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public partial interface IPolicyConfig {
        uint GetMixFormat([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, out WaveFormatExtensible ppFormat);
        uint GetDeviceFormat([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.Bool)] bool bDefault, out WaveFormatExtensible ppFormat);
        uint ResetDeviceFormat([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName);
        uint SetDeviceFormat([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.LPStruct)] WaveFormatExtensible pEndpointFormat, [In][MarshalAs(UnmanagedType.LPStruct)] WaveFormatExtensible pMixFormat);
        uint GetProcessingPeriod([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.Bool)] bool bDefault, out long pmftDefaultPeriod, out long pmftMinimumPeriod);
        uint SetProcessingPeriod([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, long pmftPeriod);
        uint GetShareMode([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, out DeviceShareMode pMode);
        uint SetShareMode([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In] DeviceShareMode mode);
        uint GetPropertyValue([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.Bool)] bool bFxStore, ref PropertyKey pKey, out PropVariant pv);
        uint SetPropertyValue([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.Bool)] bool bFxStore, [In] ref PropertyKey pKey, ref PropVariant pv);
        uint SetDefaultEndpoint([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.U4)] ERole role);
        uint SetEndpointVisibility([In][MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName, [In][MarshalAs(UnmanagedType.Bool)] bool bVisible);
    }
    public enum ERole {
        eConsole = 0,
        eMultimedia = eConsole + 1,
        eCommunications = eMultimedia + 1,
        ERole_enum_count = eCommunications + 1
    }

    public enum DeviceShareMode {
        Shared,
        Exclusive
    }

    public static class DevicePolicy {
        public static bool SetAudioEndpointState(string deviceId, bool state) {
            var hr = 0x80004005; // S_FAIL
            var CLSID_PolicyConfig = new Guid("{870af99c-171d-4f9e-af0d-e63df40c2bc9}");
            var PolicyConfigType = Type.GetTypeFromCLSID(CLSID_PolicyConfig, true);
            var PolicyConfig = Activator.CreateInstance(PolicyConfigType);
            IPolicyConfig pPolicyConfig = (IPolicyConfig)PolicyConfig;
            if ( pPolicyConfig is object ) {
                hr = pPolicyConfig.SetEndpointVisibility(deviceId, state);
                Marshal.ReleaseComObject(PolicyConfig);
            }
            return hr == 0;
        }
    }
}
