using amethyst_installer_gui.Installer;
using System;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolNotPowered : IProtocolCommand {
        public string Command { get => "notpowered"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"notpowered\"!");
            Logger.Info("Attempting to fix E_NUI_NOTPOWERED...");
            bool status = KinectUtil.FixNotPowered();
            if ( status ) {
                Logger.Info("Fixed E_NUI_NOTPOWERED successfully!");
                Util.ShowMessageBox("Fixed E_NUI_NOTPOWERED succesfully!", "Success");
            } else {
                Logger.Info("No devices with E_NUI_NOTPOWERED were found!");
                Util.ShowMessageBox("No devices with E_NUI_NOTPOWERED were found!", "Success");
            }
            Logger.Info("Done!");
            return true;
        }
    }
}
