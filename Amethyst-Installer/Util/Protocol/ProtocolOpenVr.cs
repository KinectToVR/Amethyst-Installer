using amethyst_installer_gui.Commands;
using amethyst_installer_gui.Installer;
using amethyst_installer_gui.PInvoke;
using amethyst_installer_gui.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolRegister : IProtocolCommand {
        public string Command { get => "register"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"register\"!");

            string ameDir = InstallUtil.LocateAmethystInstall();
            if (ameDir.Length == 0) {
                Logger.Info("Failed to locate Amethyst install! Aborting...");
                Util.ShowMessageBox("Failed to locate Amethyst install!");
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

            // TODO: Skip this step during an upgrade
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
                Logger.Info("Successfully removed K2EX add-on!");
                Util.ShowMessageBox("Successfully removed K2EX add-on!", "Success");
            } else {
                Logger.Info("Couldn't find K2EX add-on!");
                Util.ShowMessageBox("Couldn't find K2EX add-on! Your Amethyst install can work properly.", "Success");
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

    public class ProtocolOcusus : IProtocolCommand {
        public string Command { get => "ocusus"; set { } }

        public bool Execute(string parameters) {
            Process.Start("https://ocusus.com");
            return true;
        }
    }
}
