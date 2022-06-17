using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace amethyst_installer_gui.Installer {
    /// <summary>
    /// Helper class responsible for interfacing with OpenVR, and changing OpenVR related settings, etc...
    /// </summary>
    public static partial class OpenVRUtil {

        /// <summary>
        /// Registers an OpenVR driver
        /// </summary>
        /// <param name="driverPath">The path to the driver</param>
        public static void RegisterSteamVrDriver(string driverPath) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            string driverDirectory = Path.GetDirectoryName(driverPath);

            if ( !s_failedToInit ) {
                string vrpathregPath = Path.GetFullPath(Path.Combine(Valve.VR.OpenVR.RuntimePath(), "bin", "win64", "vrpathreg.exe"));
                if ( File.Exists(vrpathregPath) ) {
                    // TODO: vrpathreg now returns error codes! use it for driver handling
                    var args = "";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = vrpathregPath,
                        Arguments = args,
                        WorkingDirectory = driverDirectory,
                    });
                    vrpathregProc.WaitForExit();
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                        case 2: // Driver installed more than once
                            return;
                        case 1: // Driver not present
                            break;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: -2\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            // TODO: Fallback to openvrpaths

            throw new NotImplementedException();
        }


        /// <summary>
        /// Returns the path of an OpenVR driver
        /// </summary>
        /// <param name="drivername">The driver's name</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetDriverPath(string drivername) {
            if ( !s_initialized )
                throw new InvalidOperationException("Tried to execute an OpenVR method before initialization!");

            if ( !s_failedToInit ) {
                string vrpathregPath = Path.GetFullPath(Path.Combine(Valve.VR.OpenVR.RuntimePath(), "bin", "win64", "vrpathreg.exe"));
                if ( File.Exists(vrpathregPath) ) {
                    var args = $"finddriver {drivername}";
                    var vrpathregProc = Process.Start(new ProcessStartInfo() {
                        FileName = vrpathregPath,
                        Arguments = args,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true
                    });
                    var output = vrpathregProc.StandardOutput.ReadToEnd();
                    vrpathregProc.WaitForExit();
                    switch ( vrpathregProc.ExitCode ) {
                        case 0: // Success
                            return output;
                        case 1: // Driver not present
                            return "";
                        case 2: // Driver installed more than once
                            return output;
                        case -1: // Configuration or permission problem
                        case -2: // Argument problem (wtf??)
                            Logger.Fatal($"vrpathreg failed:\n\tCode: -2\n\tArgs: \"{args}\"");
                            break;
                    }
                }
            }

            // TODO: Fallback to openvrpaths
            var openvrPaths = s_openvrpaths;
            if ( openvrPaths.external_drivers.Count > 0 ) {

            }

            return "";
        }
    }
}
