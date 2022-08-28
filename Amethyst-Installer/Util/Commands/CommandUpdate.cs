using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace amethyst_installer_gui.Commands {
    public class CommandUpdate : ICommand {

        public string Command { get => "update"; set { } }
        public string Description { get => "Attempts to update Amethyst"; set { } }
        public string[] Aliases { get => new string[] { "u" }; set { } }

        public bool Execute(string parameters) {

            return true;
        }
    }
}
