using Newtonsoft.Json;
using System.Collections.Generic;

namespace amethyst_installer_gui.Installer.OpenVR {
    public class OculusDeviceCache {
        [JsonProperty("devices")]
        public List<OculusDevice> Devices { get; set; }
    }

    public class OculusDevice {
        [JsonProperty("batteryState")]
        public object BatteryState { get; set; }

        [JsonProperty("configurationState")]
        public string ConfigurationState { get; set; }

        [JsonProperty("connectionState")]
        public string ConnectionState { get; set; }

        [JsonProperty("firmware")]
        public OculusFirmware Firmware { get; set; }

        [JsonProperty("guardianSupportMode")]
        public string GuardianSupportMode { get; set; }

        [JsonProperty("headsetOSBuild")]
        public int HeadsetOSBuild { get; set; }

        [JsonProperty("headsetOSUpdateRecommended")]
        public bool HeadsetOSUpdateRecommended { get; set; }

        [JsonProperty("headsetRequiresNewerService")]
        public bool HeadsetRequiresNewerService { get; set; }

        [JsonProperty("headsetSupportsFirmwareUpgrade")]
        public bool HeadsetSupportsFirmwareUpgrade { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("isUsingAirLink")]
        public bool IsUsingAirLink { get; set; }

        [JsonProperty("lastSeenAt")]
        public int LastSeenAt { get; set; }

        [JsonProperty("operationalState")]
        public string OperationalState { get; set; }

        [JsonProperty("passthroughSupportType")]
        public string PassthroughSupportType { get; set; }

        [JsonProperty("powerState")]
        public string PowerState { get; set; }

        [JsonProperty("primaryState")]
        public string PrimaryState { get; set; }

        [JsonProperty("problems")]
        public List<object> Problems { get; set; }

        [JsonProperty("serialNumber")]
        public string SerialNumber { get; set; }

        [JsonProperty("subtype")]
        public string Subtype { get; set; }

        [JsonProperty("supportsOculusLink")]
        public bool SupportsOculusLink { get; set; }

        [JsonProperty("trackingType")]
        public string TrackingType { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("usbConnection")]
        public OculusUsbConnection UsbConnection { get; set; }

        [JsonProperty("visibility")]
        public OculusDeviceVisibility Visibility { get; set; }

        [JsonProperty("xrsClientBuild")]
        public int XrsClientBuild { get; set; }

        [JsonProperty("iad")]
        public double? Iad { get; set; }

        [JsonProperty("lastPrimaryAt")]
        public int? LastPrimaryAt { get; set; }

        [JsonProperty("mixedRealityCaptureSupport")]
        public string MixedRealityCaptureSupport { get; set; }

        [JsonProperty("pairedTo")]
        public string PairedTo { get; set; }
    }

    public class OculusFirmware {
        [JsonProperty("latestVersion")]
        public string LatestVersion { get; set; }

        [JsonProperty("updateProgress")]
        public double UpdateProgress { get; set; }

        [JsonProperty("updateState")]
        public string UpdateState { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("versionState")]
        public string VersionState { get; set; }
    }

    public class OculusUsbConnection {
        [JsonProperty("driverProvider")]
        public string DriverProvider { get; set; }

        [JsonProperty("driverVersion")]
        public string DriverVersion { get; set; }

        [JsonProperty("expectedDriverProvider")]
        public string ExpectedDriverProvider { get; set; }

        [JsonProperty("expectedDriverVersion")]
        public string ExpectedDriverVersion { get; set; }

        [JsonProperty("isBlacklisted")]
        public bool IsBlacklisted { get; set; }

        [JsonProperty("isDriverOutdated")]
        public bool IsDriverOutdated { get; set; }

        [JsonProperty("isWrongDriverProvider")]
        public bool IsWrongDriverProvider { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("recommendedMode")]
        public string RecommendedMode { get; set; }

        [JsonProperty("requiredMode")]
        public string RequiredMode { get; set; }

        [JsonProperty("usbProductId")]
        public int UsbProductId { get; set; }

        [JsonProperty("usbVendorId")]
        public int UsbVendorId { get; set; }

        [JsonProperty("usbVendorName")]
        public string UsbVendorName { get; set; }
    }

    public class OculusDeviceVisibility {
        [JsonProperty("headset")]
        public bool Headset { get; set; }

        [JsonProperty("ltouch")]
        public bool Ltouch { get; set; }

        [JsonProperty("rtouch")]
        public bool Rtouch { get; set; }
    }
}
