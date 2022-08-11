namespace amethyst_installer_gui.Installer {

    /// <summary>
    /// Used to identify supported HMDs.
    /// </summary>
    public enum VRHmdType {
        /// <summary>
        /// Oculus Rift (CV1).
        /// </summary>
        Rift,
        /// <summary>
        /// Oculus Rift S
        /// </summary>
        RiftS,
        /// <summary>
        /// Oculus Quest.
        /// </summary>
        Quest,
        /// <summary>
        /// Oculus Quest 2.
        /// </summary>
        Quest2,
        /// <summary>
        /// HTC Vive.
        /// </summary>
        Vive,
        /// <summary>
        /// HTC Vive Pro.
        /// </summary>
        VivePro,
        /// <summary>
        /// HTC Vive Cosmos.
        /// </summary>
        ViveCosmos,
        /// <summary>
        /// The ThrillSeeker headset.
        /// </summary>
        Index,
        /// <summary>
        /// Hi Brad.
        /// </summary>
        Deckard,
        /// <summary>
        /// Pimax VR Headsets.
        /// </summary>
        Pimax,
        /// <summary>
        /// Windows Mixed Reality.
        /// </summary>
        WMR,
        /// <summary>
        /// RGB VR!
        /// </summary>
        PSVR,
        /// <summary>
        /// Pico Neo
        /// </summary>
        PicoNeo,
        /// <summary>
        /// Pico Neo 2
        /// </summary>
        PicoNeo2,
        /// <summary>
        /// Pico Neo 3
        /// </summary>
        PicoNeo3,
        /// <summary>
        /// My condolences.
        /// </summary>
        Phone,
        /// <summary>
        /// What are you using?!
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// The tracking system employed by a VR Headset
    /// </summary>
    public enum VRTrackingType {
        /// <summary>
        /// Enable stage tracking I'm begging you it's for your own sanity.
        /// </summary>
        Quest,
        /// <summary>
        /// Tracking system used by Oculus devices other than the Quest family of headsets (Rift S and CV1 basically).
        /// </summary>
        Oculus,
        /// <summary>
        /// Tracking system used by devices that support the Lighthouse ecosystem.
        /// </summary>
        Lighthouse,
        /// <summary>
        /// It works, but you gotta deal with that runtime, and I feel bad for you.
        /// </summary>
        MixedReality,
        /// <summary>
        /// Basically idk how you got here. Buy a proper headset pls.
        /// </summary>
        Unknown,
    }

    /// <summary>
    /// Connection method used by a VR Headset (primarily focused on Quest)
    /// </summary>
    public enum VRConnectionType {
        /// <summary>
        /// Most headsets use this. Even wireless headsets like the HTC Vive with the wireless kit will use this.
        /// </summary>
        Tethered,
        /// <summary>
        /// ALVR.
        /// </summary>
        ALVR,
        /// <summary>
        /// Guy Godium.
        /// </summary>
        VirtualDesktop,
        /// <summary>
        /// You probably bought the official Link cable.
        /// </summary>
        OculusLink,
        /// <summary>
        /// Stop using Riftcat.
        /// </summary>
        Unknown,
    }
}
