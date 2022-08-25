using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenUninstallList.Commands {
    public class CommandChecksum : ICommand {

        public string Command { get => "checksum"; set { } }
        public string Description { get => "Calculates a given file's checksum"; set { } }
        public string[] Aliases { get => new string[] { "k" }; set { } }

        public void Execute(params string[] parameters) {
            // @TODO: Rework whenever we have a better upgrade workflow

            return;
        }
    }
}
