using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {


        /// <summary>
        /// Sets a tracker's role
        /// </summary>
        /// <returns>Whether a tracker role was successfully set</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetTrackerRole(string trackerName, TrackerRole role) {

            try {
                // Try loading steam vr settings in case
                LoadSteamVRSettings();
                if ( s_steamvrSettings == null ) {
                    if ( s_steamvrSettings == null ) {
                        Logger.Warn("SteamVR Settings doesn't exist! Has SteamVR been run at least once?");
                        return false;
                    }
                }

                // Now try force enabling the driver
                if ( s_steamvrSettings["trackers"] == null )
                    s_steamvrSettings["trackers"] = new JObject();

                s_steamvrSettings["trackers"][trackerName] = role.ToString();
                SaveSteamVrSettings();
                return true;

            } catch ( Exception ex ) {
                // Oh no, the user has an antivirus probably
                // This isn't a critical exception, so we catch it
                Logger.Error(Util.FormatException(ex));
                Logger.Warn("The user might have to set the tracker roles manually in SteamVR.");
                return false;
            }
        }

        /// <summary>
        /// Removes a tracker's role
        /// </summary>
        /// <returns>Whether a tracker role was removed. Also returns false if an error was encountered.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveTrackerRole(string trackerName) {

            try {
                // Try loading steam vr settings in case
                LoadSteamVRSettings();
                if ( s_steamvrSettings == null ) {
                    if ( s_steamvrSettings == null ) {
                        Logger.Warn("SteamVR Settings doesn't exist! Has SteamVR been run at least once?");
                        return false;
                    }
                }

                // Now try force enabling the driver
                if ( s_steamvrSettings["trackers"] == null )
                    return false;

                if (s_steamvrSettings["trackers"][trackerName] != null) {
                    ((JObject)s_steamvrSettings["trackers"]).Remove(trackerName);
                }
                SaveSteamVrSettings();
                return true;

            } catch ( Exception ex ) {
                // Oh no, the user has an antivirus probably
                // This isn't a critical exception, so we catch it
                Logger.Error(Util.FormatException(ex));
                Logger.Warn("The user might have to set the tracker roles manually in SteamVR.");
                return false;
            }
        }

    }

    public enum TrackerRole {
        TrackerRole_None,
        TrackerRole_Handed,
        TrackerRole_LeftFoot,
        TrackerRole_RightFoot,
        TrackerRole_LeftShoulder,
        TrackerRole_RightShoulder,
        TrackerRole_LeftElbow,
        TrackerRole_RightElbow,
        TrackerRole_LeftKnee,
        TrackerRole_RightKnee,
        TrackerRole_Waist,
        TrackerRole_Chest,
        TrackerRole_Camera,
        TrackerRole_Keyboard
    }
}
