using amethyst_installer_gui.Controls;
using amethyst_installer_gui.PInvoke;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Installer.Modules {
    public class AmethystModule : ModuleBase {
        public AmethystModule() {}

        /*
         
if upgrade yes
   delete old files in folder according to funny filelist
   extract new ame zip
   update uninstall entry version number
if upgrade no
   extract ame zip to new folder
   register uninstall entry
   verify that there are no other entries in vrpaths named amethyst, if so remove them
   register ame driver entry in steamvr
   register tracker roles in vrsettings
         
         */

        public override bool Install(string sourceFile, string path, ref InstallModuleProgress control, out TaskState state) {

            // TODO: Check for previous install of Amethyst, and if present, soft-uninstall (soft means only get rid of the install itself, don't touch configs or SteamVR)

            if (ExtractAmethyst(sourceFile, path, ref control) ) {

                bool overallSuccess = HandleDrivers(path, ref control);

                overallSuccess = overallSuccess && CreateUninstallEntry(path, ref control);

                // TODO: Tracker roles
                overallSuccess = overallSuccess && AssignTrackerRoles(ref control);

                // TODO: Shortcuts
                overallSuccess = overallSuccess && CreateShortcuts(path, ref control);

                // TODO: If this is an upgrade change the message to a different one
                Logger.Info(LogStrings.InstalledAmethystSuccess);
                control.LogInfo(LogStrings.InstalledAmethystSuccess);
                state = TaskState.Checkmark;
                return overallSuccess;
            }

            state = TaskState.Error;
            return false;
        }

        private bool ExtractAmethyst(string zip, string target, ref InstallModuleProgress control) {
            try {

                Logger.Info(string.Format(LogStrings.ExtractingAmethyst, target));
                control.LogInfo(string.Format(LogStrings.ExtractingAmethyst, target));
                
                string ameZip = Path.GetFullPath(Path.Combine(Constants.AmethystTempDirectory, zip));
                if ( File.Exists(ameZip) ) {

                    if ( !Directory.Exists(target) )
                        Directory.CreateDirectory(target);

                    ZipFile.ExtractToDirectory(ameZip, target);
                    return true;
                }

            } catch ( Exception e ) {
                Logger.Fatal($"{LogStrings.ExtractAmethystFailed}:\n{Util.FormatException(e)})");
                control.LogError($"{LogStrings.ExtractAmethystFailed}! {LogStrings.ViewLogs}");
            }
            return false;
        }

        private bool HandleDrivers(string path, ref InstallModuleProgress control) {

            string driverPath = Path.Combine(path, "Amethyst");


            // TODO: Skip this step during an upgrade
            Logger.Info(LogStrings.CheckingAmethystDriverConflicts);
            control.LogInfo(LogStrings.CheckingAmethystDriverConflicts);

            // Check for existing Amethyst driver entries
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("Amethyst")) ) {
                OpenVRUtil.RemoveDriversWithName("Amethyst");
            }

            // Check for K2EX driver
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("KinectToVR")) ) {
                OpenVRUtil.ForceDisableDriver("KinectToVR");
                // OpenVRUtil.RemoveDriversWithName("KinectToVR");
            }

            // TODO: Skip this step during an upgrade
            Logger.Info(LogStrings.RegisteringAmethystDriver);
            control.LogInfo(LogStrings.RegisteringAmethystDriver);

            OpenVRUtil.RegisterSteamVrDriver(driverPath);
            OpenVRUtil.ForceEnableDriver("Amethyst");

            // Shell.OpenFolderAndSelectItem(Path.GetFullPath(Path.Combine(Constants.Userprofile, "AppData", "Local", "openvr", "openvrpaths.vrpath")));

            return true;
        }

        private bool AssignTrackerRoles(ref InstallModuleProgress control) {

            return false;
        }

        private bool CreateShortcuts(string path, ref InstallModuleProgress control) {

            return false;
        }

        private bool CreateUninstallEntry(string target, ref InstallModuleProgress control) {

            var amethystExecutable = Path.GetFullPath(Path.Combine(target, "Amethyst.exe"));
            var amethystInstallerExecutable = Path.GetFullPath(Path.Combine(target, "Amethyst-Installer.exe"));

            control.LogInfo(LogStrings.CreatingUninstallEntry);
            Logger.Info(LogStrings.CreatingUninstallEntry);

            UninstallEntry entry = new UninstallEntry(){
                DisplayName = "Amethyst",
                Publisher = "K2VR Team",
                DisplayIcon = $"{amethystExecutable},0", // something.exe,0 ; number is icon index in the binary's resources
                
                URLInfoAbout = "https://k2vr.tech/",
                Contact = Constants.DiscordInvite,

                DisplayVersion = Module.DisplayVersion,
                ApplicationVersion = Module.DisplayVersion,

                InstallLocation = target,
                UninstallString = amethystInstallerExecutable + " --uninstall",
                ModifyPath = amethystInstallerExecutable + " --modify"
            };

            try {
                UninstallUtil.RegisterUninstallEntry(entry, "Amethyst");
                return true;
            } catch (Exception e) {
                control.LogError($"{LogStrings.CreateUninstallEntryFailed}! {LogStrings.ViewLogs}");
                Logger.Fatal($"{LogStrings.CreateUninstallEntryFailed}:\n{Util.FormatException(e)})");
            }

            return false;
        }
    }
}
