﻿using amethyst_installer_gui.Commands;
using System;

namespace amethyst_installer_gui.Protocol {
    public class ProtocolUninstall : IProtocolCommand {
        public string Command { get => "uninstall"; set { } }

        public bool Execute(string parameters) {
            App.Init();
            Logger.Info("Received protocol command \"uninstall\"!");
            string[] a = Array.Empty<string>();
            return new CommandUninstall().Execute(ref a);
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
