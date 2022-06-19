using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer {

    /// <summary>
    /// Used to identify supported HMDs.
    /// </summary>
    public enum VRHmdType {
        /// <summary>
        /// An Oculus PCVR Headset. Doesn't include the Quest family of headsets.
        /// </summary>
        Oculus,
        /// <summary>
        /// Oculus Quest family of headsets. Doesn't include PCVR Headsets such as the CV1.
        /// </summary>
        Quest,
        /// <summary>
        /// HTC Vive.
        /// </summary>
        Vive,
        /// <summary>
        /// HTC Vive Pro.
        /// </summary>
        VivePro,
        /// <summary>
        /// The ThrillSeeker headset.
        /// </summary>
        Index,
        /// <summary>
        /// Hi Brad.
        /// </summary>
        Deckard,
        /// <summary>
        /// Windows Mixed Reality.
        /// </summary>
        WMR,
        /// <summary>
        /// RGB VR!
        /// </summary>
        PSVR,
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
