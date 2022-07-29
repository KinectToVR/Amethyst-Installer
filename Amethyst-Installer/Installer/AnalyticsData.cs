using System;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// A container for analytics data, this will then get converted to JSON and sent to our API servers
    /// </summary>
    public struct AnalyticsData {
        /// <summary>
        /// The VR Headset model of the current user
        /// </summary>
        public VRHmdType HeadsetModel               { get; set; }
        /// <summary>
        /// The tracking universe of the current user
        /// </summary>
        public VRTrackingType TrackingUniverse      { get; set; }
        /// <summary>
        /// The tracking universe of the current user
        /// </summary>
        public VRConnectionType ConnectionType      { get; set; }
        /// <summary>
        /// The Windows build of the system
        /// </summary>
        public string WindowsBuild                  { get; set; }
        /// <summary>
        /// The Amethyst version we're attempting to install
        /// </summary>
        public string TargetAmethystVersion         { get; set; }
        /// <summary>
        /// The version of the installer
        /// </summary>
        public string InstallerVersion              { get; set; }
        /// <summary>
        /// Whether we are running on a Cloud machine (unsupported!)
        /// </summary>
        public bool IsCloudMachine                  { get; set; }
        /// <summary>
        /// Whether we found a prior install of K2EX or not
        /// </summary>
        public bool K2EXFound                       { get; set; }
        /// <summary>
        /// Time at which the installer was opened in unix time
        /// </summary>
        public ulong InstallerStartupTime           { get; set; }
        /// <summary>
        /// Devices found on the current system
        /// </summary>
        public DeviceFlags Devices                  { get; set; }
        /// <summary>
        /// Status code for the installer
        /// </summary>
        public InstallerStatusCode InstallerState   { get; set; }
    }

    /// <summary>
    /// An error code which represents the installer's state, be it a success or a failure, so that we can track where the installer falls short
    /// </summary>
    public enum InstallerStatusCode : uint {
        /// <summary>
        /// A successful install of Amethyst
        /// </summary>
        Success                     = 0,
        /// <summary>
        /// The application threw an unknown exception
        /// </summary>
        UnknownException               ,
        /// <summary>
        /// Downloading Amethyst's files failed
        /// </summary>
        DownloadFailed                 ,
        /// <summary>
        /// Generic failure, not really helpful for debugging
        /// </summary>
        InstallFailed                  ,
        /// <summary>
        /// Access was denied to some critical folder
        /// </summary>
        AccessDenied                   ,
        /// <summary>
        /// Insufficient permissions exception while trying to install Amethyst and modules
        /// </summary>
        InsufficientPrivileges         ,
        /// <summary>
        /// Failed to find a valid SteamVR install
        /// </summary>
        SteamVRNotFound                ,
        /// <summary>
        /// Failed to connect to the K2VR web API
        /// </summary>
        K2VR_Web_APINotFound           ,
    }

    /// <summary>
    /// Supported devices which were found on the system
    /// </summary>
    [Flags]
    public enum DeviceFlags : uint {
        /// <summary>
        /// No supported device was found
        /// </summary>
        None        = 0,
        /// <summary>
        /// An Xbox 360 Kinect was found
        /// </summary>
        KinectV1    = 2,
        /// <summary>
        /// An Xbox One Kinect was found
        /// </summary>
        KinectV2    = 4,
        /// <summary>
        /// PlayStation move drivers were found
        /// </summary>
        PSMove      = 8,
    }
}
