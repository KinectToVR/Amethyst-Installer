using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace amethyst_installer_gui.Commands {
    public class CommandParser {

        private List<ICommand> m_commandList;

        public CommandParser() {
            m_commandList = new List<ICommand>();
            m_commandList.Add(new CommandUninstall());
        }
    }
}
