using System.Diagnostics;
using System.IO;

namespace AmethystUtils.Protocol {
    public class ProtocolRegister : IProtocolCommand {
        public string Command { get => "register"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolRemoveLegacyAddons : IProtocolCommand {
        public string Command { get => "removelegacyaddons"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolDisableOwotrack : IProtocolCommand {
        public string Command { get => "disableowotrack"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolOpenVr : IProtocolCommand {
        public string Command { get => "openvrpaths"; set { } }

        public bool Execute(string parameters) {
            string openvrpaths = Path.GetFullPath(Path.Combine(Constants.Userprofile, "AppData", "Local", "openvr", "openvrpaths.vrpath"));
            Shell.OpenFolderAndSelectItem(Path.GetDirectoryName(openvrpaths));
            return true;
        }
    }

    public class ProtocolLogs : IProtocolCommand {
        public string Command { get => "logs"; set { } }

        public bool Execute(string parameters) {
            Shell.OpenFolderAndSelectItem(Constants.AmethystLogsDirectory);
            return true;
        }
    }

    public class ProtocolCloseSteamVr : IProtocolCommand {
        public string Command { get => "closeconflictingapps"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolOcusus : IProtocolCommand {
        public string Command { get => "ocusus"; set { } }

        public bool Execute(string parameters) {
            Process.Start("https://ocusus.com");
            return true;
        }
    }
}
