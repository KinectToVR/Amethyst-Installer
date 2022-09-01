using amethyst_installer_gui.Installer;
using System;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolNotPowered : IProtocolCommand {
        public string Command { get => "notpowered"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"notpowered\"!");
            Logger.Info("Attempting to fix E_NUI_NOTPOWERED...");
            KinectUtil.FixNotPowered();
            return true;
        }
    }
}
