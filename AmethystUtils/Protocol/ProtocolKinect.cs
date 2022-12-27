using System.Diagnostics;

namespace AmethystUtils.Protocol {
    public class ProtocolNotPowered : IProtocolCommand {
        public string Command { get => "notpowered"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }
    public class ProtocolNotPoweredSilent : IProtocolCommand {
        public string Command { get => "notpowered/silent"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolFixMicrophone : IProtocolCommand {
        public string Command { get => "fixmicrophone"; set { } }

        public bool Execute(string parameters) {
            Util.PassToInstaller($"amethyst://{Command}");
            return true;
        }
    }

    public class ProtocolToolkit : IProtocolCommand {
        public string Command { get => "kinect/toolkit"; set { } }

        public bool Execute(string parameters) {
            Process.Start(@"C:\Program Files\Microsoft SDKs\Kinect\Developer Toolkit v1.8.0\Tools\ToolkitBrowser\ToolkitBrowser.exe");
            return true;
        }
    }

    public class ProtocolToolkitExplorer : IProtocolCommand {
        public string Command { get => "kinect/toolkit/explorer"; set { } }

        public bool Execute(string parameters) {
            Process.Start(@"C:\Program Files\Microsoft SDKs\Kinect\Developer Toolkit v1.8.0\bin\KinectExplorer-D2D.exe");
            return true;
        }
    }
}
