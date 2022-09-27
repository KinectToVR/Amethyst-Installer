using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmethystUtils.Protocol {

    public class ProtocolLogs : IProtocolCommand {
        public string Command { get => "logs"; set { } }

        public bool Execute(string parameters) {
            Shell.OpenFolderAndSelectItem(Constants.AmethystLogsDirectory + Path.DirectorySeparatorChar);
            return true;
        }
    }

    public class ProtocolLogsInstaller : IProtocolCommand {
        public string Command { get => "logs/installer"; set { } }

        public bool Execute(string parameters) {
            Shell.OpenFolderAndSelectItem(Constants.AmethystLogsDirectory + Path.DirectorySeparatorChar + "installer" + Path.DirectorySeparatorChar);
            return true;
        }
    }

    public class ProtocolLogsAmethystApp : IProtocolCommand {
        public string Command { get => "logs/amethyst"; set { } }

        public bool Execute(string parameters) {
            Shell.OpenFolderAndSelectItem(Constants.AmethystLogsDirectory + Path.DirectorySeparatorChar + "amethyst" + Path.DirectorySeparatorChar);
            return true;
        }
    }
}
