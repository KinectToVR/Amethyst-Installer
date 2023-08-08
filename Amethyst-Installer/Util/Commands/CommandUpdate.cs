using amethyst_installer_gui.Installer;
using amethyst_installer_gui.Pages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using AmethystModule = amethyst_installer_gui.Installer.Module;

namespace amethyst_installer_gui.Commands {
    public class CommandUpdate : ICommand {
        public string Command { get => "update"; set { } }
        public string Description { get => "Attempts to update Amethyst"; set { } }
        public string[] Aliases { get => new string[] { "u" }; set { } }

        public bool Execute(ref string[] args) {
            Process.Start(
                new ProcessStartInfo("ms-windows-store://pdp/?productid=9P7R8FGDDGDH") { UseShellExecute = true });

            // We have a valid config!
            string[] a = Array.Empty<string>();
            return new CommandUninstall().Execute(ref a);
        }
    }
}