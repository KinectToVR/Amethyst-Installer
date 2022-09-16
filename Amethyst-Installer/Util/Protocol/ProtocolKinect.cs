using amethyst_installer_gui.Installer;
using System.Diagnostics;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolNotPowered : IProtocolCommand {
        public string Command { get => "notpowered"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"notpowered\"!");
            Logger.Info("Attempting to fix E_NUI_NOTPOWERED...");
            if ( KinectUtil.FixNotPowered() ) {
                Logger.Info("Fixed E_NUI_NOTPOWERED successfully!");
                Util.ShowMessageBox("Fixed E_NUI_NOTPOWERED succesfully!", "Success");
            } else {
                Logger.Info("No devices with E_NUI_NOTPOWERED were found!");
                Util.ShowMessageBox("No applicable devices with E_NUI_NOTPOWERED were found!\nEither the Kinect isn't connected, or you have a different error", "Success");
            }
            Logger.Info("Done!");
            return true;
        }
    }
    public class ProtocolNotPoweredSilent : IProtocolCommand {
        public string Command { get => "notpowered/silent"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"notpowered\"!");
            Logger.Info("Attempting to fix E_NUI_NOTPOWERED...");
            if ( KinectUtil.FixNotPowered() ) {
                Logger.Info("Fixed E_NUI_NOTPOWERED successfully!");
            } else {
                Logger.Info("No devices with E_NUI_NOTPOWERED were found!");
            }
            Logger.Info("Done!");
            return true;
        }
    }

    public class ProtocolFixMicrophone : IProtocolCommand {
        public string Command { get => "fixmicrophone"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"fixmicrophone\"!");
            Logger.Info("Attempting to check for a Kinect microphone...");
            
            if ( KinectUtil.KinectMicrophonePresent() ) {
                Logger.Info("Checking microphone state...");
                if ( KinectUtil.KinectMicrophoneDisabled() ) {
                    Logger.Info("Microphone was found to be disabled");
                    Util.ShowMessageBox("The Kinect microphone is currently disabled, and must be enabled for the Kinect SDK to function properly!\nPlease enable it in this window.", "Microphone disabled");
                    Process.Start("rundll32.exe", "shell32.dll,Control_RunDLL mmsys.cpl,,1");
                } else {
                    Logger.Info("Microphone was found to be enabled");
                    Util.ShowMessageBox("No issues were detected! Your Kinect will function properly!", "Microphone already enabled");
                }
            } else {
                Logger.Info("Kinect not detected!");
                Util.ShowMessageBox("No Kinect was detected! Please connect it to your computer, and try again.", "Kinect not detected");
            }
            Logger.Info("Done!");
            return true;
        }
    }
}
