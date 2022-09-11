using amethyst_installer_gui.Commands;
using amethyst_installer_gui.Installer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolUninstall : IProtocolCommand {
        public string Command { get => "uninstall"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"uninstall\"!");
            return new CommandUninstall().Execute(string.Empty);
        }
    }

    /*
    
    // @TODO: Implement modify

    public class ProtocolModify: IProtocolCommand {
        public string Command { get => "modify"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"modify\"!");
            Logger.Info("Attempting to fix E_NUI_NOTPOWERED...");
            KinectUtil.FixNotPowered();
            Logger.Info("Done!");
            return true;
        }
    }
    */
}
