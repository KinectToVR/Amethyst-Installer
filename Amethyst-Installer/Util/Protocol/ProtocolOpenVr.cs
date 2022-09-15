using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using System.Diagnostics;
using System.IO;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolRegister : IProtocolCommand {
        public string Command { get => "register"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"register\"!");

            string ameDir = InstallUtil.LocateAmethystInstall();
            if (ameDir.Length == 0) {
                Logger.Info("Failed to locate Amethyst install! Aborting...");
                Util.ShowMessageBox("Failed to locate Amethyst install!", "Oops");
                return true;
            } else {
                Logger.Info($"Amethyst install found at {ameDir}");
            }

            // Check for existing Amethyst add-on entries
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("Amethyst")) ) {
                Logger.Info("Found multiple Amethyst add-ons! Removing...");
                OpenVRUtil.RemoveDriversWithName("Amethyst");
            }

            // Check for K2EX add-on, because of conflicts
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("KinectToVR")) ) {
                Logger.Info("K2EX add-on found! Removing...");
                OpenVRUtil.ForceDisableDriver("KinectToVR");
                OpenVRUtil.RemoveDriversWithName("KinectToVR");
            }

            Logger.Info(LogStrings.RegisteringAmethystDriver);

            string driverPath = Path.Combine(ameDir, "Amethyst");

            OpenVRUtil.RegisterSteamVrDriver(driverPath);
            OpenVRUtil.ForceEnableDriver("Amethyst");

            InstallUtil.TryKillingConflictingProcesses();

            Util.ShowMessageBox("Successfully re-registered Amethyst SteamVR add-on!", "Success");

            return true;
        }
    }

    public class ProtocolRemoveLegacyAddons : IProtocolCommand {
        public string Command { get => "removelegacyaddons"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"removelegacyaddons\"!");

            // Check for K2EX add-on
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("KinectToVR")) ) {
                Logger.Info("Found K2EX add-on! Removing it...");
                InstallUtil.TryKillingConflictingProcesses();

                OpenVRUtil.ForceDisableDriver("KinectToVR");
                OpenVRUtil.RemoveDriversWithName("KinectToVR");

                InstallUtil.TryKillingConflictingProcesses();
                Logger.Info("Successfully removed K2EX add-on!");
                Util.ShowMessageBox("Successfully removed K2EX SteamVR add-on!", "Success");
            } else {
                Logger.Info("Couldn't find K2EX add-on!");
                Util.ShowMessageBox("No conflicting SteamVR add-ons were found!\nAmethyst can work properly.", "Success");
            }

            return true;
        }
    }

    public class ProtocolDisableOwotrack : IProtocolCommand {
        public string Command { get => "disableowotrack"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"disableowotrack\"!");

            // Check for owoTrack add-on
            if ( Directory.Exists(OpenVRUtil.GetDriverPath("owoTrack")) ) {
                Logger.Info("Found owoTrack add-on! Disabling it...");
                InstallUtil.TryKillingConflictingProcesses();

                OpenVRUtil.ForceDisableDriver("owoTrack");

                Logger.Info("Successfully disabled owoTrack add-on!");
                Util.ShowMessageBox("Successfully disabled owoTrack add-on!", "Success");
            } else {
                Logger.Info("Couldn't find owoTrack add-on!");
                Util.ShowMessageBox("No conflicting SteamVR add-ons were found!\nAmethyst can work properly.", "Success");
            }

            return true;
        }
    }

    public class ProtocolOpenVr : IProtocolCommand {
        public string Command { get => "openvrpaths"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"openvrpaths\"!");
            Shell.OpenFolderAndSelectItem(Path.GetDirectoryName(OpenVRUtil.OpenVrPathsPath));
            return true;
        }
    }

    public class ProtocolLogs : IProtocolCommand {
        public string Command { get => "logs"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"logs\"!");
            Shell.OpenFolderAndSelectItem(Constants.AmethystLogsDirectory);
            return true;
        }
    }
    
    public class ProtocolCloseSteamVr : IProtocolCommand {
        public string Command { get => "closeconflictingapps"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"closeconflictingapps\"!");
            InstallUtil.TryKillingConflictingProcesses();
            return true;
        }
    }

    public class ProtocolOcusus : IProtocolCommand {
        public string Command { get => "ocusus"; set { } }

        public bool Execute(string parameters) {
            Process.Start("https://ocusus.com");
            return true;
        }
    }
}
