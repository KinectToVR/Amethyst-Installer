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
}
