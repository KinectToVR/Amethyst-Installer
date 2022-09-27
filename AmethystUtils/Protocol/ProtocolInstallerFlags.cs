using System.IO;

namespace AmethystUtils.Protocol {
    public class ProtocolUninstall : IProtocolCommand {
        public string Command { get => "uninstall"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    /*
    
    // @TODO: Implement modify

    public class ProtocolModify: IProtocolCommand {
        public string Command { get => "modify"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }
    */

    public class ProtocolInstallLocation : IProtocolCommand {
        public string Command { get => "installfolder"; set { } }

        public bool Execute(string parameters) {
            string installDirectory = InstallUtil.LocateAmethystInstall();
            Shell.OpenFolderAndSelectItem(installDirectory + Path.DirectorySeparatorChar);
            return true;
        }
    }
}
